using MODELS.BASE;
using MODELS.REACTIONTYPE.Dtos;

namespace BE.Services.ReactionType
{
    public interface IREACTIONTYPEService
    {
        BaseResponse<GetListPagingResponse> GetListPaging(GetListPagingRequest request);
        BaseResponse<ModelReactionType> GetById(GetByIdRequest request);
    }
}
