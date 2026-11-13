using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ProjectJumaiAuth.Authorization;
using ProjectJumaiAuth.Data;
using ProjectJumaiAuth.Extensions;
using ProjectJumaiAuth.Mapping;
using ProjectJumaiAuth.Middleware;
using ProjectJumaiAuth.Models.Settings;
using ProjectJumaiAuth.Repositories;
using ProjectJumaiAuth.Repository;
using ProjectJumaiAuth.Servicces;
using ProjectJumaiAuth.Services;
using System.Text;

using static ProyectJumaiAuth.Extenciones.JWTPermisionPolicies.permissionAuthorizationHandler;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("Smtp"));

var jwtSettings = builder.Configuration
    .GetSection("Jwt")
    .Get<JwtSettings>()
    ?? throw new InvalidOperationException("No está presente la configuración del JWT");

builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(UserProfile).Assembly));

builder.Services.AddScoped<IAuthServices, AuthServices>();
builder.Services.AddScoped<ITokenServices, TokenService>();
builder.Services.AddScoped<IRolPermissionServices, RolPermissionService>();
builder.Services.AddScoped<IEstudianteService, EstudianteService>();
builder.Services.AddScoped<ICursoService, CursoService>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IRoleRepository, RolRepository>();
builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
builder.Services.AddScoped<IEstudianteRepository, EstudianteRepository>();
builder.Services.AddScoped<ICursoRepository, CursoRepository>();
builder.Services.AddScoped<IEmailService, EmailService>();

// ==================== DBContext ====================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ProyectJumaiAuth")));

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (context.Request.Cookies.ContainsKey("access_token"))
                {
                    context.Token = context.Request.Cookies["access_token"];
                }
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                context.HandleResponse();
                if (!context.Response.HasStarted)
                {
                    context.Response.Redirect("/Auth/Login");
                }
                return Task.CompletedTask;
            },
            OnForbidden = context =>
            {
                if (!context.Response.HasStarted)
                {
                    context.Response.Redirect("/Auth/AccessDenied");
                }
                return Task.CompletedTask;
            }
        };

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,

            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,

            ValidateLifetime = true,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),

            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("admin.panel", policy =>
        policy.RequireAssertion(context =>
            context.User.TienePermiso("rol.ver") ||
            context.User.ObtenerPermisos().Contains("permiso.ver") ||
            context.User.ObtenerPermisos().Contains("usuario.ver")));

    options.AddPolicy("permiso.actualizar", policy =>
        policy.RequireAssertion(context =>
            context.User.TienePermiso("permiso.actualizar") || context.User.TienePermiso("permiso.crear")));

    options.AddPolicy("permiso.borrar", policy =>
        policy.RequireAssertion(context =>
            context.User.TienePermiso("permiso.borrar") || context.User.TienePermiso("permiso.crear")));
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStatusCodePages(async context =>
{
    var response = context.HttpContext.Response;

    if (response.StatusCode == 404 && !response.HasStarted)
    {
        response.Redirect("/Home/NotFoundPage");
    }

    await Task.CompletedTask;
});

app.UseRouting();
app.UseSession();
app.UseMiddleware<JwtRefreshMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();