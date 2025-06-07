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
        File = 2, // Tin nhắn là File
        TextAndFile = 3, // Tin nhắn vừa text và file
        Call = 4 // Tin nhắn là một cuộc gọi điện
    }
}
