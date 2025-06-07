using MODELS.BASE;
using MODELS.GROUPMEMBER.Dtos;
using MODELS.GROUPMEMBER.Requests;
using MODELS.USER.Dtos;

namespace BE.Services.GroupMember
{
    public interface IGROUPMEMBERService
    {
        BaseResponse<GetListPagingResponse> GetListPaging(GetListPagingRequest request);
        BaseResponse<MODELGroupMember> GetById(GetByIdRequest request);
        BaseResponse<POSTGroupMemberRequest> GetByPost(GetByIdRequest request);
        BaseResponse<MODELGroupMember> Insert(POSTGroupMemberRequest request);
        BaseResponse<MODELGroupMember> Update(POSTGroupMemberRequest request);
        BaseResponse<string> DeleteList(DeleteListRequest request);
    }
}
