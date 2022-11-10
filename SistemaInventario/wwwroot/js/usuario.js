var datatable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    datatable = $('#tblDatos').DataTable({
        "ajax": {
            "url": "/Admin/Usuario/ObtenerTodos"
        },
        "columns": [
            { "data": "userName", "width": "20%" },
            { "data": "nombres", "width": "20%" },
            { "data": "apellidos", "width": "20%" },
            { "data": "email", "width": "20%" },
            { "data": "phoneNumber", "width": "20%" },
            { "data": "role", "width": "20%" },


            
            //{
            //    "data": "id",
            //    "render": function (data) {
            //        return `
            //            <div class="text-center">
            //                <a href="/Admin/Categoria/Upsert/${data}" class="btn btn-success text-white" style="cursor:pointer">
            //                    <i class="fas fa-edit"></i>
            //                </a>
            //                <a onclick=Delete("/Admin/Categoria/Delete/${data}") class="btn btn-danger text-white" style="cursor:pointer">
            //                    <i class="fas fa-trash"></i>
            //                </a>
            //            </div>
            //            `;
            //    }, "width": "20%"
            //}
        ]
    });
}


function Delete(url) {
    swal({
        title: "Esta seguro que quiere Eliminar la Categoria?",
        text: "Este registro no se podra recuperar",
        icon: "warning",
        buttons: true,
        dangerMode: true
    }).then((borrar) => {
        if (borrar) {
            $.ajax({
                type: "DELETE",
                url: url,
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        datatable.ajax.reload();
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            });
        }
    });
}