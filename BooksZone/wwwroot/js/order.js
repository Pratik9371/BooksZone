var dataTable;

$(document).ready(function () {

    var url = window.location.search;
    if (url.includes("inprocess")) {
        loadDataTable("GetOrdersList?status=inprocess");
    } else if (url.includes("pending")) {
        loadDataTable("GetOrdersList?status=pending");
    } else if (url.includes("completed")) {
        loadDataTable("GetOrdersList?status=completed");
    } else if (url.includes("rejected")) {
        loadDataTable("GetOrdersList?status=rejected");
    } else {
        loadDataTable("GetOrdersList?status=all");
    }
})

function loadDataTable(url) {
    dataTable = $("#tableData").DataTable({
        "ajax": {
            "url": "/Admin/Order/" + url ,
        },
        "columns": [
            { "data": "id" },
            { "data": "name" },
            { "data": "phoneNumber" },
            { "data": "applicationUser.email" },
            { "data": "orderStatus" },
            { "data": "orderTotal" },
            {
                "data": "id",
                "render": function (data) {
                    return `<div class="text-center">
                                 <a href="/Admin/Order/Details/${data}" class="btn btn-success text-white"><i class="fa fa-edit"></i></a>
                            </div>`;
                }
            },
        ]
    })
}

