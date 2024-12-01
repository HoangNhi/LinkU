// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// Thông báo
function closeToast(event) {
    let toastClose = event.target;
    toastClose.closest('.toast').remove();
}

function ShowThongBaoThanhCong(Message) {
    const thongBaoContainer = document.getElementById('ThongBao-container');

    const toast = document.createElement('div');
    toast.className = 'toast success';
    toast.innerHTML = `
        <div class="toast-status-icon">
            <div style="background-color: hsl(120, 70%, 40%); width: 30px; height: 30px; border-radius: 50%; display: flex; align-items: center; justify-content: center; color: white;">
                <i class="fas fa-check"></i>
            </div>
        </div>
        <div class="toast-content">
            <span>Thông báo</span>
            <p style="margin-bottom: 0">${Message}</p>
        </div>
        <button class="toast-close" onclick="closeToast(event)">
            <i class="fas fa-times"></i>
        </button>
        <div class="toast-duration"></div>
    `;

    thongBaoContainer.appendChild(toast);

    // Tự động đóng sau 4 giây
    setTimeout(() => {
        toast.remove();
    }, 2000);
}

function ShowThongBaoThatBai(Message) {
    const thongBaoContainer = document.getElementById('ThongBao-container');

    const toast = document.createElement('div');
    toast.className = 'toast error';
    toast.innerHTML = `
        <div class="toast-status-icon">
            <div style="background-color: hsl(5, 85%, 50%); width: 30px; height: 30px; border-radius: 50%; display: flex; align-items: center; justify-content: center; color: white;">
                <i class="fas fa-times"></i>
            </div>
        </div>
        <div class="toast-content">
            <span>Thông báo</span>
            <p style="margin-bottom: 0">${Message}</p>
        </div>
        <button class="toast-close" onclick="closeToast(event)">
            <i class="fas fa-times"></i>
        </button>
        <div class="toast-duration"></div>
    `;

    thongBaoContainer.appendChild(toast);

    // Tự động đóng sau 4 giây
    setTimeout(() => {
        toast.remove();
    }, 2000);
}

// Loading
function showLoading(value) {
    if (value) {
        $.blockUI({
            message:
                '<div class="loader-demo-box">' +
                '<img style="width: 120px;" src="/asset/Loading.gif" />' +
                '<div class="bar-loader">' +
                '<span></span>' +
                '<span></span>' +
                '<span></span>' +
                '<span></span>' +
                '</div>' +
                '</div>'
        });
    }
    else {
        $.unblockUI();
    }
}

function showLoadingElement(value, id) {
    if (value) {
        $('#' + id).block({
            message:
                '<div class="loader-demo-box">' +
                '<img style="width: 120px;" src="/asset/Loading.gif" />' +
                '<div class="bar-loader">' +
                '<span></span>' +
                '<span></span>' +
                '<span></span>' +
                '<span></span>' +
                '</div>' +
                '</div>'
        });
    }
    else {
        $('#' + id).unblock();
    }
}
