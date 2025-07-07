using MODELS.BASE;
using MODELS.GROUPMEMBER.Dtos;
using MODELS.GROUPMEMBER.Requests;

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
        Task<BaseResponse<MODELResponseAddMemberToGroup>> AddMemberToGroup(POSTAddMemberToGroupRequest request);
        Task<BaseResponse<List<MODELGroupMember>>> GetListByGroupId(GetByIdRequest request);
    }
}
