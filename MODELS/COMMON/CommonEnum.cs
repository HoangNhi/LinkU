namespace MODELS.COMMON
{
    public enum HttpAction
    {
        Post,
        Put,
        Delete,
        Get
    }

    public enum MediaFileType
    {
        ProfilePicture,
        CoverPicture,
        ChatImage,
        ChatFile,
        GroupAvatar,
    }

    public enum MessageType
    {
        Text = 0,
        Welcome = 1, // Tin nhắn chào mừng (sử dụng khi tạo group)
        Notification = 2, // Tin nhắn thông báo (sử dụng khi có sự kiện mới trong group)
        File = 3, // Tin nhắn là File
        TextAndFile = 4, // Tin nhắn vừa text và file
        Call = 5 // Tin nhắn là một cuộc gọi điện
    }
}
