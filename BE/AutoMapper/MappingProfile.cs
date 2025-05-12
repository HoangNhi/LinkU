using AutoMapper;
using ENTITIES.DbContent;
using MODELS.FRIENDREQUEST.Dtos;
using MODELS.FRIENDREQUEST.Requests;
using MODELS.FRIENDSHIP.Dtos;
using MODELS.FRIENDSHIP.Requests;
using MODELS.MEDIAFILE.Dtos;
using MODELS.MEDIAFILE.Requests;
using MODELS.MESSAGE.Dtos;
using MODELS.MESSAGE.Requests;
using MODELS.MESSAGESTATUS.Dtos;
using MODELS.MESSAGESTATUS.Requests;
using MODELS.OTP.Dtos;
using MODELS.REFRESHTOKEN.Dtos;
using MODELS.USER.Dtos;
using MODELS.USER.Requests;

namespace BE.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User
            CreateMap<User, MODELUser>().ReverseMap();
            CreateMap<User, PostUserRequest>().ReverseMap();
            CreateMap<User, RegisterRequest>().ReverseMap();
            CreateMap<User, PostUpdateUserInforRequest>().ReverseMap();

            // OTP
            CreateMap<OTP, MODELOTP>().ReverseMap();

            // RefreshToken
            CreateMap<RefreshToken, MODELRefreshToken>().ReverseMap();

            // FriendRequest
            CreateMap<FriendRequest, MODELFriendRequest>().ReverseMap();
            CreateMap<FriendRequest, POSTFriendRequest>().ReverseMap();

            // Friendship
            CreateMap<Friendship, MODELFriendship>().ReverseMap();
            CreateMap<Friendship, POSTFriendshipRequest>().ReverseMap();

            // Message
            CreateMap<Message, MODELMessage>().ReverseMap();
            CreateMap<Message, PostMessageRequest>().ReverseMap();

            // MediaFile
            CreateMap<MediaFile, MODELMediaFile>().ReverseMap();
            CreateMap<MediaFile, POSTMediaFileRequest>().ReverseMap();

            // Conversation
            CreateMap<Conversation, MODELConversation>().ReverseMap();
            CreateMap<Conversation, POSTConversationRequest>().ReverseMap();
        }
    }
}
