using MODELS.BASE;
using MODELS.REACTIONTYPE.Dtos;

namespace BE.Services.ReactionType
{
    public interface IREACTIONTYPEService
    {
        BaseResponse<GetListPagingResponse> GetListPaging(GetListPagingRequest request);
        Task<BaseResponse<ModelReactionType>> GetById(GetByIdRequest request);
        Task<List<ModelReactionType>> GetByIds(List<Guid> ids);
        Task<List<ModelReactionType>> GetByIdsAsync(List<Guid> ids);
    }
}
