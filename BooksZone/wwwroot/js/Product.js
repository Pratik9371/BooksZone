var dataTable;

$(document).ready(function () {
    loadDataTable();
})

function loadDataTable() {
    dataTable = $("#tableData").DataTable({
        "ajax": {
            "url": "/Admin/Product/GetAll",
        },
        "columns": [
            { "data": "title" },
            { "data": "isbn" },
            { "data": "price" },
            { "data": "author" },
            { "data": "category.name" },
            {
                "data": "id",
                "render": function (data) {
                    return `<div class="text-center">
                                    <a href="/Admin/Product/Upsert/${data}" class="btn btn-success text-white"><i class="fa fa-edit"></i></a>
                                    <a onclick=Delete("/Admin/Product/Delete/${data}") class="btn btn-danger text-white" style="cursor:pointer"><i class="fa fa-trash"></i></a>
                                </div>`;
                }
            }
        ]
    })
}


function Delete(url) {
    swal({
        title: "Are you sure you want to delete?",
        text: "You will not be able to restore the data!",
        icon: "warning",
        buttons: true,
        dangerMode: true
    }).then((res) => {
        if (res) {
            $.ajax({
                type: "DELETE",
                url: url,
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        dataTable.ajax.reload();
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            });
        }
    })
}