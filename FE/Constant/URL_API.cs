namespace FE.Constant
{
    public static class URL_API
    {
        // User
        public const string USER_GET_LIST_PAGING = "/User/GetListPaging";
        public const string USER_GET_LIST_MEDIA_FILES = "/User/GetListMediaFiles";
        public const string USER_GET_BY_ID = "/User/GetById";
        public const string USER_GET_BY_POST = "/User/GetByPost";
        public const string USER_INSERT = "/User/Insert";
        public const string USER_UPDATE_INFOR = "/User/UpdateInfor";
        public const string USER_UPDATE_PICTURE = "/User/UpdatePicture";
        public const string USER_DELETE_LIST = "/User/DeleteList";
        public const string USER_LOGIN = "/User/Login";
        public const string USER_REGISTER = "/User/Register";
        public const string USER_REFRESH_TOKEN = "/User/RefreshToken";
        public const string USER_LOGOUT = "/User/Logout";
        public const string USER_CHECKUSERNAMEEXIST = "/User/CheckUsernameExist";
        public const string USER_LOGINGOOGLE = "/User/LoginGoogle";
        public const string USER_SENDOTP = "/User/SendOTP";
        public const string USER_VERIFYOTP = "/User/VerifyOTP";
        public const string USER_CHANGEPASSWORD = "/User/ChangePassword";

        // Message
        public const string MESSAGE_GET_LIST_PAGING = "/Message/GetListPaging";
        public const string MESSAGE_GET_BY_ID = "/Message/GetById";
        public const string MESSAGE_GET_BY_POST = "/Message/GetByPost";
        public const string MESSAGE_INSERT = "/Message/Insert";
        public const string MESSAGE_UPDATE = "/Message/Update";
        public const string MESSAGE_DELETE_LIST = "/Message/DeleteList";

        // MessageList
        public const string MESSAGELIST_SEARCH = "/MessageList/Search";
        public const string MESSAGELIST_GETLISTMESSAGELATEST = "/MessageList/GetListMessageLatest";

        // FriendRequest
        public const string FRIENDREQUEST_GET_LIST_PAGING = "/FriendRequest/GetListPaging";
        public const string FRIENDREQUEST_GET_BY_ID = "/FriendRequest/GetById";
        public const string FRIENDREQUEST_GET_BY_POST = "/FriendRequest/GetByPost";
        public const string FRIENDREQUEST_INSERT = "/FriendRequest/Insert";
        public const string FRIENDREQUEST_UPDATE = "/FriendRequest/Update";
        public const string FRIENDREQUEST_DELETE = "/FriendRequest/Delete";
        public const string FRIENDREQUEST_GETFRIENDREQUESTSTATUS = "/FriendRequest/GetFriendRequestStatus";

        // MEDIAFILE
        /// <summary>
        /// Cho phép upload file ở tất cả định dạng
        /// </summary>
        public const string MEDIAFILE_UPLOAD_FILE = "/MediaFile/UploadFile";
        /// <summary>
        /// Cho phép upload file ở định dạng ảnh (image/jpeg, image/png)
        /// </summary>
        public const string MEDIAFILE_UPLOAD_PICTURE = "/MediaFile/UploadFile";
        public const string MEDIAFILE_DOWNLOAD = "/MediaFile/Download";
        public const string MEDIAFILE_GET_BY_POST = "/MediaFile/GetByPost";
        public const string MEDIAFILE_DELETE_LIST = "/MediaFile/DeleteList";
        public const string MEDIAFILE_UPDATE_PICTURE_USER_WITH_UPLOAD_PICTURE = "/MediaFile/UpdatePictureUserWithUploadPicture";
        public const string MEDIAFILE_UPDATE_PICTURE_USER_WITHOUT_UPLOAD_PICTURE = "/MediaFile/UpdatePictureUserWithoutUploadPicture";


    }
}
