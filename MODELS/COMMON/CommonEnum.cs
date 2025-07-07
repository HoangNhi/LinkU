namespace MODELS.COMMON
{
    public enum HttpAction
    {
        Post,
        Put,
        Delete,
        Get
    }

    /// <summary>
    /// Enum: 0 - ProfilePicture, 1 -  CoverPicture, 2 - ChatImage, 3 - ChatFile, 4 - Avartar Group, 5 - ChatVideo
    /// </summary>
    public enum MediaFileType
    {
        ProfilePicture = 0,
        CoverPicture = 1,
        ChatImage = 2,
        ChatFile = 3,
        GroupAvatar = 4,
        ChatVideo = 5
    }
    /// <summary>
    /// 0 - tin nhắn thông thường, 1 - Tin nhắn chào mừng( sử dụng khi tạo group), 2 - Tin nhắn thông báo các thay đổi của nhóm(đổi tên nhóm, thêm thành viên, chuyển nhóm trưởng), 3 - Tin nhắn là File, 4 - Tin nhắn vừa text và file, 5 - Tin nhắn là 1 cuộc gọi điện
    /// </summary>
    public enum MessageType
    {
        Text = 0,
        Welcome = 1, // Tin nhắn chào mừng (sử dụng khi tạo group)
        Notification = 2, // Tin nhắn thông báo (sử dụng khi có sự kiện mới trong group)
        File = 3, // Tin nhắn là File
        TextAndFile = 4, // Tin nhắn vừa text và file
        Call = 5 // Tin nhắn là một cuộc gọi điện
    }

    /// <summary>
    /// 0 - Invalid, 1 - Hình vuông, 2 - Hình chữ nhật ngang, 3 - Hình chữ nhật dọc
    /// </summary>
    public enum ShapeType
    {
        Invalid = 0,
        Square = 1, // Hình vuông
        Landscape = 2, // Hình chữ nhật ngang
        Portrait = 3 // Hình chữ nhật dọc
    }

    public enum CaseHandleMessage
    {
        MessageGetListPaging = 0,
        SendPrivateMessage = 1,
        SendMessageWithFile = 2,
        SendGroupMessage = 3,
        UpdateMessageReaction = 4,
    }
}
