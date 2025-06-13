using AutoMapper;
using BE.Helpers;
using ENTITIES.DbContent;
using Microsoft.Data.SqlClient;
using MODELS.BASE;
using MODELS.FRIENDREQUEST.Dtos;
using MODELS.FRIENDREQUEST.Requests;
using MODELS.FRIENDSHIP.Dtos;
using MODELS.FRIENDSHIP.Requests;
using MODELS.USER.Dtos;

namespace BE.Services.FriendShip
{
    public class FIRENDSHIPService : IFRIENDSHIPService
    {
        private readonly LINKUContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;

        public FIRENDSHIPService(LINKUContext context, IMapper mapper, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
        }


        public BaseResponse<MODELFriendship> Insert(POSTFriendshipRequest request)
        {
            var response = new BaseResponse<MODELFriendship>();
            try
            {
                var checkExist = _context.Friendships
                    .Any(x => ((x.UserId1 == request.UserId1 && x.UserId2 == request.UserId2) ||
                                         (x.UserId1 == request.UserId2 && x.UserId2 == request.UserId1))
                                        && !x.IsDeleted);
                if (checkExist)
                {
                    throw new Exception("Dữ liệu đã tồn tại");
                }

                var add = _mapper.Map<Friendship>(request);
                add.Id = request.Id == Guid.Empty ? Guid.NewGuid() : request.Id;
                add.NgayTao = DateTime.Now;
                add.NguoiTao = _contextAccessor.HttpContext.User.Identity.Name;
                add.NgaySua = DateTime.Now;
                add.NguoiSua = _contextAccessor.HttpContext.User.Identity.Name;

                // Lưu dữ liệu
                _context.Friendships.Add(add);
                _context.SaveChanges();

                response.Data = _mapper.Map<MODELFriendship>(add);
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }
            return response;
        }
        
        public BaseResponse<GetListPagingResponse> GetListPaging(POSTFriendshipGetListPagingRequest request)
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

                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@iUserId", request.UserId),
                    new SqlParameter("@iTextSearch", request.TextSearch),
                    new SqlParameter("@iPageIndex", request.PageIndex - 1),
                    new SqlParameter("@iRowsPerPage", request.RowPerPage),
                    iTotalRow
                };

                // Lấy dữ liệu
                var result = _context.ExcuteStoredProcedure<MODELUser>("sp_FRIENDSHIP_GetListPaging", parameters).ToList();

                GetListPagingResponse responseData = new GetListPagingResponse();
                responseData.PageIndex = request.PageIndex;
                responseData.Data = result;
                responseData.TotalRow = Convert.ToInt32(iTotalRow.Value);
                response.Data = responseData;
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }
            return response;
        }
    }
}
