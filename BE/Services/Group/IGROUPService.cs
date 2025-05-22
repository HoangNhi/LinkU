using MODELS.BASE;
using MODELS.GROUP.Dtos;
using MODELS.GROUP.Requests;

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
    }
}
