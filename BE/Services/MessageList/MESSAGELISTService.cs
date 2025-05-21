using AutoMapper;
using BE.Helpers;
using ENTITIES.DbContent;
using Microsoft.Data.SqlClient;
using MODELS.BASE;
using MODELS.MESSAGE.Dtos;
using MODELS.MESSAGELIST.Dtos;
using MODELS.MESSAGELIST.Requests;
using MODELS.USER.Dtos;

namespace BE.Services.MessageList
{
    public class MESSAGELISTService : IMESSAGELISTService
    {
        private readonly LINKUContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;

        public MESSAGELISTService(LINKUContext context, IMapper mapper, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
        }

        public BaseResponse<MODELMessageList_Search> Search(MessageList_SearchRequest request)
        {
            var response = new BaseResponse<MODELMessageList_Search>();
            try
            {
                var UserId = Guid.Parse(_contextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "name").Value);
                // Tìm kiếm người dùng theo số điện thoại hoặc email
                var users = _context.Users.Where(u => u.IsDeleted == false
                                                && (u.SoDienThoai.Equals(request.TextSearch) || u.Email.Equals(request.TextSearch))
                                                && u.Id != UserId) // Không trả về User hiện tại
                                          .Select(x => new MODELUser
                                          {
                                              Id = x.Id,
                                              HoLot = x.HoLot,
                                              Ten = x.Ten,
                                              Email = x.Email,
                                              SoDienThoai = x.SoDienThoai,
                                          })
                                          .OrderBy(x => string.Concat(x.HoLot, " ", x.Ten))
                                          .ToList();
                // Lấy ảnh đại diện của người dùng
                foreach (var user in users)
                {
                    var ProfilePicture = _context.MediaFiles
                                                 .FirstOrDefault(m => m.OwnerId == user.Id
                                                                && m.FileType == (int)MODELS.COMMON.MediaFileType.ProfilePicture
                                                                && !m.IsDeleted && m.IsActived);
                    if (ProfilePicture != null)
                    {
                        user.ProfilePicture = ProfilePicture.Url;
                    }
                }

                response.Data = new MODELMessageList_Search
                {
                    Users = users
                };
            }
            catch (System.Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }
            return response;
        }


        public BaseResponse<GetListPagingResponse> GetListMessageLatest(POSTGetListMessageLatestRequest request)
        {
            var response = new BaseResponse<GetListPagingResponse>();
            try
            {
                SqlParameter iTotalRow = new SqlParameter()
                {
                    ParameterName = "@oTotalRow",
                    SqlDbType = System.Data.SqlDbType.BigInt,
                    Direction = System.Data.ParameterDirection.Output
                };

                var parameters = new[]
                {
                    new SqlParameter("@iTextSearch", request.TextSearch),
                    new SqlParameter("@iPageIndex", request.PageIndex - 1),
                    new SqlParameter("@iRowsPerPage", request.RowPerPage),
                    new SqlParameter("@iCurrentUserId", request.CurrentUserId),
                    iTotalRow
                };

                var result = _context.ExcuteStoredProcedure<MODELMessageList_GetListMessageLatest>("sp_MESSAGELIST_GetListMessageLastes", parameters).ToList();

                GetListPagingResponse resposeData = new GetListPagingResponse();
                resposeData.PageIndex = request.PageIndex;
                resposeData.Data = result;
                resposeData.TotalRow = Convert.ToInt32(iTotalRow.Value);
                response.Data = resposeData;
            }
            catch (System.Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }
            return response;
        }
    }
}
