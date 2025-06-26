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
    }, 4000);
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
    }, 4000);
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
                '</div>',
            css: { zIndex: 1055}
        });

        $(".blockUI.blockOverlay").css("zIndex", 1055)
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
function ShowModal(html, size, extenClass = []) {
    $('#modalContainer').removeClass().addClass("modal-dialog modal-dialog-centered");
    // Kích thước
    if (size != 'md')
    {
        $('#modalContainer').addClass('modal-' + size);
    }

    // Thêm class nếu có
    if (extenClass.length > 0) {
        extenClass.forEach(ec => $('#modalContainer').addClass(ec))
    }

    // Chèn html và hiển thị
    $('#modalContainer').html(html);
    $('#modal-default').modal('show');
}

function CloseModal() {
    $('#modal-default').modal('hide');
}

function ShowLightBox(src) {
    // Set Background
    const lightboxModal = $("#lightboxModal"),
        modalImg = $("#modal-image"),
        backgroudBlur = $("#background-blur"),
        modalDialog = $("#lightboxModal .modal-dialog"),
        downloadButton = $("#lightboxModal #modal-option #BtnDownload");

    // Set Background Blur
    backgroudBlur.css("background-image",  `url(${src})`)

    // Set content
    modalImg.css("background-image", `url(${src})`);

    // Set Width Modal
    const img = new Image();
    img.src = src;

    img.onload = function () {
        if (img.naturalWidth > img.naturalHeight) {
            modalDialog.addClass("modal-xl")
        } else {
            modalDialog.removeClass("modal-xl")
        }
    };

    var uriDownload = `/Home/DownloadFile?fileName=${src.split('/').pop()}`;
    downloadButton.attr("href", uriDownload)

    // Show LightBox
    lightboxModal.modal('show')
}

function CloseLightBox() {
    $("#lightboxModal").modal('hide')
}

var notFound = `
        <div class="container-fluid">
            <div class="">
                <h2 class="fs-4 fw-bold text-black mb-3 text-center" style="line-height: 1.5">Không Tìm Thấy Kết Quả</h2>

                <p class="mb-4 fs-6 text-center" style="line-height: 1.5; font-weight: 400">
                    Rất tiếc, chúng tôi không thể tìm thấy người dùng mà bạn đang tìm kiếm.
                    Hãy thử lại với từ khóa khác hoặc kiểm tra lại chính tả.
                </p>

                <div class="note note-primary">
                    <h6 class="text-primary mb-2 text-center"
                        style="font-weight: 600;">
                        <i class="fa-solid fa-lightbulb me-2"></i>
                        Gợi ý tìm kiếm:
                    </h6>
                    <ul class="m-0">
                        <li class="mb-1" style="line-height: 1.5; color: var(--mdb-gray-700); font-weight: 400">Kiểm tra lại chính tả và thử lại</li>
                        <li class="mb-1" style="line-height: 1.5; color: var(--mdb-gray-700); font-weight: 400">Sử dụng từ khóa ngắn gọn hơn</li>
                        <li class="mb-1" style="line-height: 1.5; color: var(--mdb-gray-700); font-weight: 400">Thử các từ đồng nghĩa khác</li>
                        <li class="mb-1" style="line-height: 1.5; color: var(--mdb-gray-700); font-weight: 400">Loại bỏ các ký tự đặc biệt</li>
                    </ul>
                </div>
            </div>
        </div>`


var noFriends = `
        <div class="container-fluid">
            <div class="">
                <h2 class="fs-4 fw-bold text-black mb-3 text-center" style="line-height: 1.5">Chưa Có Bạn Bè</h2>
                <p class="mb-4 fs-6 text-center" style="line-height: 1.5; font-weight: 400">
                    Bạn chưa kết bạn với ai cả. Hãy bắt đầu tìm kiếm và kết nối với những người bạn mới 
                    để có trải nghiệm tuyệt vời hơn!
                </p>
                <div class="note note-primary">
                    <h6 class="text-primary mb-2 text-center"
                        style="font-weight: 600;">
                        <i class="fa-solid fa-users me-2"></i>
                        Bắt đầu kết bạn:
                    </h6>
                    <ul class="m-0">
                        <li class="mb-1" style="line-height: 1.5; color: var(--mdb-gray-700); font-weight: 400">Tìm kiếm bạn bè qua tên hoặc email</li>
                        <li class="mb-1" style="line-height: 1.5; color: var(--mdb-gray-700); font-weight: 400">Khám phá danh sách gợi ý bạn bè</li>
                        <li class="mb-1" style="line-height: 1.5; color: var(--mdb-gray-700); font-weight: 400">Chia sẻ thông tin của bạn với người khác</li>
                        <li class="mb-1" style="line-height: 1.5; color: var(--mdb-gray-700); font-weight: 400">Tham gia các nhóm cộng đồng</li>
                    </ul>
                </div>
                <div class="text-center mt-4">
                    <button type="button" class="btn btn-primary me-2" onclick="SwitchToTabFunctions('tab-conversation');if($('#TargetId').val() === '' || $('#TargetId').val() === undefined){SwitchToTabChatArea('tab-wellcome')}else{SwitchToTabChatArea('tab-chat-message')};$('#Conversation_SearchInput').focus();">
                        <i class="fa-solid fa-user-plus me-1"></i>
                        Tìm Bạn Bè
                    </button>
                    <button type="button" class="btn btn-outline-primary">
                        <i class="fa-solid fa-share me-1"></i>
                        Chia Sẻ Hồ Sơ
                    </button>
                </div>
            </div>
        </div>`

var noGroupRequest = `
     <div class="container-fluid text-center">
        <img src="/asset/Group.png" height = "200" />
        <h4>Không có lời mời nào</h4>
        <p>Bạn chưa có lời mời tham gia nhóm nào.</p>
    </div>
`

var noGroup = `
     <div class="container-fluid text-center">
        <img src="/asset/Group.png" height = "200" />
        <h4>Khám phá các nhóm thú vị</h4>
        <p  class="text-break"
            style="width: 60%; justify-self: center;">
            Tham gia các cộng đồng để kết nối với những người có cùng sở thích và chia sẻ những trải nghiệm tuyệt vời
        </p>
    </div>
`
function playNotification() {
    const sound = document.getElementById("notificationSound");
    sound.currentTime = 0; // Reset về đầu
    sound.play();
}