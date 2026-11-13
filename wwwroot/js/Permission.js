$(function () {
    const config = window.permisosConfig || {};
    const modal = new bootstrap.Modal("#permisoModal");
    let isEdit = false;

    const permisoValidador = new JustValidate("#permisoForm", {
        validateBeforeSubmitting: true,
        focusInvalidField: true,
        errorFieldCssClass: 'is-invalid',
        successFieldCssClass: 'is-valid'
    });

    permisoValidador
        .addField("#permisoNombre", [
            { rule: 'required', errorMessage: "El nombre es obligatorio." },
            { rule: 'maxLength', value: 100, errorMessage: "Máximo es de 100 caracteres" },
            {
                rule: 'customRegexp',
                value: /^[a-zA-Z0-9_]+\.[a-zA-Z0-9_]+$/,
                errorMessage: "Use formato: recurso.accion (ej: estudiante.crear)"
            }
        ])
        .addField("#permisoDescripcion", [
            { rule: 'maxLength', value: 250, errorMessage: "Máximo es de 250 caracteres" }
        ]);

    const table = $("#permisoTable").DataTable({
        processing: true,
        serverSide: true,
        ajax: {
            url: '/Permiso/GetData',
            type: 'POST'
        },
        columns: [
            { data: 'id' },
            {
                data: 'nombre',
                render: d => `<code>${d}</code>`
            },
            { data: 'descripcion' },
            {
                data: null,
                orderable: false,
                searchable: false,
                render: function (data) {
                    let buttons = '';
                    if (config.canUpdate) {
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
            url: 'https://cdn.datatables.net/plug-ins/1.13.8/i18n/es-ES.json'
        },
        order: [[1, 'asc']]
    });

    function clearForm() {
        $('#permisoId').val('');
        $('#permisoNombre').val('');
        $('#permisoDescripcion').val('');
        permisoValidador.refresh();
    }

    $('#btnNuevoPermiso').on('click', function () {
        isEdit = false;
        clearForm();
        $('#permisoModalTitle').text('Nuevo Permiso');
        modal.show();
    });

    $('#permisoTable').on('click', '.btn-edit', async function () {
        const id = $(this).data('id');
        try {
            const response = await axios.get(`/Admin/GetPermissionById/${id}`);
            isEdit = true;
            $('#permisoId').val(response.data.id);
            $('#permisoNombre').val(response.data.nombre);
            $('#permisoDescripcion').val(response.data.descripcion || '');
            $('#permisoModalTitle').text('Editar Permiso');
            permisoValidador.refresh();
            modal.show();
        } catch (error) {
            Swal.fire('Error', 'No se pudo cargar el permiso', 'error');
        }
    });

    $('#permisoTable').on('click', '.btn-delete', async function () {
        const id = $(this).data('id');
        const nombre = $(this).data('nombre');

        const confirm = await Swal.fire({
            title: '¿Eliminar Permiso?',
            html: `¿Seguro que deseas eliminar el permiso <strong>${nombre}</strong>?`,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Sí, eliminar',
            cancelButtonText: 'Cancelar',
            confirmButtonColor: '#d33',
            cancelButtonColor: '#6c757d'
        });

        if (!confirm.isConfirmed) return;

        try {
            const response = await axios.delete(`/Admin/DeletePermission?id=${id}`);
            if (response.data.success) {
                table.ajax.reload();
                Swal.fire('Eliminado', response.data.message, 'success');
            }
        } catch (error) {
            const message = error.response?.data?.message || 'No se pudo eliminar';
            Swal.fire('Error', message, 'error');
        }
    });

    permisoValidador.onSuccess(async (event) => {
        event.preventDefault();

        const payload = {
            id: $('#permisoId').val() ? parseInt($('#permisoId').val()) : null,
            nombre: $('#permisoNombre').val().trim(),
            descripcion: $('#permisoDescripcion').val().trim() || null
        };

        try {
            const response = isEdit
                ? await axios.put('/Admin/UpdatePermission', payload)
                : await axios.post('/Admin/CreatePermission', payload);

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