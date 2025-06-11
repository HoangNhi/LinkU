using MODELS.BASE;
using MODELS.GROUP.Dtos;
using MODELS.GROUP.Requests;
using MODELS.GROUPMEMBER.Dtos;

namespace BE.Services.Group
{
    public interface IGROUPService
    {
        BaseResponse<GetListPagingResponse> GetListPaging(GetListPagingRequest request);
        BaseResponse<MODELGroup> GetById(GetByIdRequest request);
        BaseResponse<POSTGroupRequest> GetByPost(GetByIdRequest request);
        BaseResponse<MODELGroup> Insert(POSTGroupRequest request);
        BaseResponse<MODELGroup> Update(POSTGroupRequest request);
        BaseResponse<string> DeleteList(DeleteListRequest request);

        /// <summary>
        /// Tạo 1 nhóm, thêm thành viên vào nhóm và Upload ảnh đại diện nhóm (nếu có)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<BaseResponse<MODELGroup>> CreateGroupWithMember(POSTCreateGroupRequest request);

        
        BaseResponse<List<MODELMemberCreateGroup>> GetListMemberCreateGroup();
        BaseResponse<GetListPagingResponse> GetListSuggestMember(POSTGetListSuggestMemberRequest request);
    }
}
