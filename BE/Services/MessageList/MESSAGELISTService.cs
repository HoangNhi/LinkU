using AutoMapper;
using BE.Helpers;
using ENTITIES.DbContent;
using Microsoft.Data.SqlClient;
using MODELS.BASE;
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

        /// <summary>
        /// Tìm kiếm danh sách tin nhắn: Người dùng, tin nhắn, file đính kèm,...
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
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
                                                                && !m.IsDeleted && m.IsActive);
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
    }
}
