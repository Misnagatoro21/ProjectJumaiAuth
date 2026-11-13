$(function () {
    const config = window.cursoConfig || {};
    const modal = new bootstrap.Modal("#cursoModal");
    let isEdit = false;

    const cursoValidator = new JustValidate('#cursoForm', {
        validateBeforeSubmitting: true,
        focusInvalidField: true,
        errorFieldCssClass: 'is-invalid',
        successFieldCssClass: 'is-valid'
    });

    cursoValidator
        .addField("#cursoNombre", [
            { rule: 'required', errorMessage: "El nombre es obligatorio." },
            { rule: 'minLength', value: 2, errorMessage: "Mínimo 2 caracteres" },
            { rule: 'maxLength', value: 100, errorMessage: "Máximo es de 100 caracteres" },
        ])
        .addField("#cursoDescripcion", [
            { rule: 'required', errorMessage: "La descripción es obligatoria." },
            { rule: 'minLength', value: 2, errorMessage: "Mínimo 2 caracteres" },
            { rule: 'maxLength', value: 100, errorMessage: "Máximo es de 100 caracteres" },
        ]);

    const table = $("#cursoTable").DataTable({
        processing: true,
        serverSide: true,
        ajax: {
            url: '/Curso/GetData',
            type: 'POST'
        },
        columns: [
            { data: 'id' },
            { data: 'nombre' },
            { data: 'descripcion' },
            {
                data: null,
                orderable: false,
                searchable: false,
                render: function (data) {
                    let buttons = '';
                    if (config.canUpdate) { // Corregido 'canUpate' a 'canUpdate'
                        buttons += `<button class="btn btn-sm btn-outline-primary me-1 btn-edit" data-id="${data.id}">
                            <i class="fa-solid fa-pen"></i></button>`;
                    }
                    if (config.canDelete) {
                        buttons += `<button class="btn btn-sm btn-outline-danger btn-delete"
                            data-id="${data.id}" data-nombre="${data.nombre}">
                            <i class="fa-solid fa-trash"></i></button>`;
                    }
                    return buttons || '<span class="text-muted">-</span>';
                }
            }
        ],
        language: {
            url: 'https://cdn.datatables.net/plug-ins/1.13.8/i18n/es-ES.json' // Corregida la duplicidad
        },
        order: [[1, 'asc']]
    });

    function clearForm() {
        $("#cursoId").val('');
        $("#cursoNombre").val('');
        $("#cursoDescripcion").val('');
        cursoValidator.refresh();
    }

    $('#btnNuevoCurso').on('click', function () {
        isEdit = false;
        clearForm();
        $("#cursoModalTitle").text("Nuevo Curso");
        modal.show();
    });

    $("#cursoTable").on('click', '.btn-edit', async function () {
        const id = $(this).data('id');
        try {
            const response = await axios.get(`/Curso/GetById?id=${id}`); // Unificado a query string (cámbialo si tu backend usa /GetById/${id})
            isEdit = true;
            $('#cursoId').val(response.data.id);
            $('#cursoNombre').val(response.data.nombre);
            $('#cursoDescripcion').val(response.data.descripcion);
            $("#cursoModalTitle").text("Editar Curso");
            cursoValidator.refresh();
            modal.show();
        } catch (error) {
            Swal.fire('Error', 'No se pudo cargar el curso', 'error');
        }
    });

    $("#cursoTable").on('click', '.btn-delete', async function () {
        const id = $(this).data('id');
        const nombre = $(this).data('nombre');

        const confirm = await Swal.fire({
            title: '¿Eliminar Curso?',
            html: `¿Seguro que deseas eliminar el curso <strong>${nombre}</strong>?`,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Sí, eliminar',
            cancelButtonText: 'Cancelar',
            confirmButtonColor: '#d33',
            cancelButtonColor: '#6c757d'
        });

        if (!confirm.isConfirmed) return;

        try {
            const response = await axios.post(`/Curso/Delete?id=${id}`); // Cambiado a post para mantener consistencia con tus otros módulos (o usa axios.delete si tu backend lo espera así)
            if (response.data.success) {
                table.ajax.reload();
                Swal.fire('Eliminado', response.data.message, 'success');
            }
        } catch (error) {
            const message = error.response?.data?.message || 'No se pudo eliminar';
            Swal.fire('Error', message, 'error');
        }
    });

    cursoValidator.onSuccess(async (event) => {
        event.preventDefault();
        const payload = {
            id: $("#cursoId").val() ? parseInt($("#cursoId").val()) : null,
            nombre: $("#cursoNombre").val().trim(),
            descripcion: $("#cursoDescripcion").val().trim() || null
        };

        try {
            const response = isEdit
                ? await axios.post('/Curso/Update', payload) // Ajustado a /Curso/Update (coherente con Estudiante)
                : await axios.post('/Curso/Create', payload);

            if (response.data.success) {
                modal.hide();
                table.ajax.reload();
                Swal.fire({
                    title: 'Listo',
                    text: response.data.message,
                    icon: 'success',
                    timer: 2000,
                    showConfirmButton: false
                });
            }
        } catch (error) {
            const message = error.response?.data?.message || 'No se pudo guardar.';
            Swal.fire('Error', message, 'error');
        }
    });
});