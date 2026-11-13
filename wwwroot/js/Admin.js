document.addEventListener('DOMContentLoaded', () => {
    async function refreshSessionIfNeeded() {
        if (typeof refreshSession === 'function') {
            await refreshSession(false);
        }
    }

    async function toggleRolePermission(roleId, permissionId, action) {
        const url = action === 'assign'
            ? `/Admin/AssignPermissionToRole?roleId=${roleId}&permissionId=${permissionId}`
            : `/Admin/RemovePermissionFromRole?roleId=${roleId}&permissionId=${permissionId}`;

        const response = await axios.post(url);
        return response.data;
    }

    document.querySelectorAll('.btn-toggle-permission').forEach(button => {
        button.addEventListener('click', async () => {
            try {
                const result = await toggleRolePermission(
                    button.dataset.roleId,
                    button.dataset.permissionId,
                    button.dataset.action);

                if (result.success) {
                    await refreshSessionIfNeeded();
                    await Swal.fire({ icon: 'success', title: 'Listo', text: result.message, timer: 1500, showConfirmButton: false });
                    location.reload();
                } else {
                    Swal.fire({ icon: 'error', title: 'Error', text: result.message });
                }
            } catch {
                Swal.fire({ icon: 'error', title: 'Error', text: 'No se pudo completar la operación.' });
            }
        });
    });

    const assignBtn = document.getElementById('btnAssignPermission');
    const permissionSelect = document.getElementById('permissionToAssign');
    if (assignBtn && permissionSelect) {
        assignBtn.addEventListener('click', async () => {
            try {
                const result = await toggleRolePermission(
                    assignBtn.dataset.roleId,
                    permissionSelect.value,
                    'assign');

                if (result.success) {
                    await refreshSessionIfNeeded();
                    await Swal.fire({ icon: 'success', title: 'Permiso asignado', text: result.message, timer: 1500, showConfirmButton: false });
                    location.reload();
                } else {
                    Swal.fire({ icon: 'error', title: 'Error', text: result.message });
                }
            } catch {
                Swal.fire({ icon: 'error', title: 'Error', text: 'No se pudo asignar el permiso.' });
            }
        });
    }

    const updateRoleBtn = document.getElementById('btnUpdateRole');
    if (updateRoleBtn) {
        updateRoleBtn.addEventListener('click', async () => {
            const userId = updateRoleBtn.dataset.userId;
            const roleId = document.getElementById('userRoleSelect').value;

            try {
                const response = await axios.post(`/Admin/UpdateUserRole?userId=${userId}`, { roleId: parseInt(roleId) });
                if (response.data.success) {
                    await refreshSessionIfNeeded();
                    await Swal.fire({ icon: 'success', title: 'Rol actualizado', text: response.data.message, timer: 1500, showConfirmButton: false });
                    location.reload();
                } else {
                    Swal.fire({ icon: 'error', title: 'Error', text: response.data.message });
                }
            } catch {
                Swal.fire({ icon: 'error', title: 'Error', text: 'No se pudo actualizar el rol.' });
            }
        });
    }

    async function assignUserPermission(userId, permissionId, isGranted) {
        const response = await axios.post(`/Admin/AssignPermissionToUser?userId=${userId}`, {
            permissionId: parseInt(permissionId),
            isGranted
        });
        return response.data;
    }

    const userPermissionsTable = document.getElementById('userPermissionsTable');
    if (userPermissionsTable) {
        userPermissionsTable.addEventListener('click', async (event) => {
            const grantBtn = event.target.closest('.btn-user-grant');
            const denyBtn = event.target.closest('.btn-user-deny');
            const removeBtn = event.target.closest('.btn-user-remove');
            const button = grantBtn || denyBtn || removeBtn;
            if (!button) return;

            try {
                if (grantBtn || denyBtn) {
                    const result = await assignUserPermission(
                        button.dataset.userId,
                        button.dataset.permissionId,
                        !!grantBtn);

                    if (result.success) {
                        await refreshSessionIfNeeded();
                        await Swal.fire({ icon: 'success', title: 'Listo', text: result.message, timer: 1500, showConfirmButton: false });
                        location.reload();
                    } else {
                        Swal.fire({ icon: 'error', title: 'Error', text: result.message });
                    }
                    return;
                }

                const response = await axios.post(`/Admin/RemoveUserPermission?userId=${button.dataset.userId}&permissionId=${button.dataset.permissionId}`);
                if (response.data.success) {
                    await refreshSessionIfNeeded();
                    await Swal.fire({ icon: 'success', title: 'Listo', text: response.data.message, timer: 1500, showConfirmButton: false });
                    location.reload();
                } else {
                    Swal.fire({ icon: 'error', title: 'Error', text: response.data.message });
                }
            } catch {
                Swal.fire({ icon: 'error', title: 'Error', text: 'No se pudo completar la operación.' });
            }
        });
    }
});