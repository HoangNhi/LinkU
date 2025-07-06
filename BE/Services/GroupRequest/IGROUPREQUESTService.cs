using MODELS.BASE;
using MODELS.GROUPREQUEST.Dtos;
using MODELS.GROUPREQUEST.Requests;

namespace BE.Services.GroupRequest
{
    public interface IGROUPREQUESTService
    {
        BaseResponse<GetListPagingResponse> GetListPaging(POSTGroupRequestGetListPagingRequest request);
        Task<BaseResponse<MODELGroupRequest>> Update(POSTGroupInvitationRequest request);
    }
}
