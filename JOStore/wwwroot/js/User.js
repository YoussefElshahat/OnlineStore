$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    let dataTable = $('#tblData').DataTable({
        "ajax": { url: '/Admin/User/GetAll' },
        "columns": [
            { data: 'name', "width": "15%", "defaultContent": "N/A" },
            { data: 'email', "width": "10%" },
            { data: 'phoneNumber', "width": "10%", "defaultContent": "N/A" },
            { data: 'companyName', "width": "10%", "defaultContent": "N/A" },
            { data: 'role', "width": "10%", "defaultContent": "Unknown" },
            {
                data: { id: "id", lockoutEnd: "lockoutEnd" },
                "render": function (data) {
                    let today = new Date().getTime();
                    let lockout = data.lockoutEnd ? new Date(data.lockoutEnd).getTime() : 0;
                    let buttonClass = lockout > today ? "btn-success" : "btn-danger";
                    let buttonText = lockout > today ? "Unlock" : "Lock";

                    return `
                        <div class="text-center">
                            <a onclick="toggleLock('${data.id}')" class="btn ${buttonClass} text-white" style="cursor:pointer; width:100px;">
                                <i class="fas fa-lock"></i> ${buttonText}
                            </a>
                            <a href="/admin/user/roleManagment?userId=${data.id}" class="btn btn-primary text-white" style="cursor:pointer; width:150px;">
                                <i class="bi bi-pencil-square"></i> Permissions
                            </a>
                        </div>`;

                },
                "width": "20%"
            }
        ]
    });
}

function toggleLock(userId) {
    $.ajax({
        type: "POST",
        url: "/Admin/User/LockUnlock",
        contentType: "application/json",
        data: JSON.stringify(userId),
        success: function (response) {
            if (response.success) {
                toastr.success(response.message);
                $('#tblData').DataTable().ajax.reload();
            } else {
                toastr.error(response.message);
            }
        }
    });
}
