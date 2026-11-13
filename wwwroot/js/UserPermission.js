$(function () {
    const tableEl = $('#userPermissionsTable');
    if (!tableEl.length) return;

    tableEl.DataTable({
        language: {
            url: 'https://cdn.datatables.net/plug-ins/1.13.8/i18n/es-ES.json'
        },
        order: [[0, 'asc']],
        columnDefs: [
            { orderable: false, targets: 3 }
        ],
        pageLength: 10
    });
});
