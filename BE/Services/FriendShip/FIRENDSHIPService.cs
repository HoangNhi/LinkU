using AutoMapper;
using ENTITIES.DbContent;
using MODELS.BASE;
using MODELS.FRIENDREQUEST.Dtos;
using MODELS.FRIENDSHIP.Dtos;
using MODELS.FRIENDSHIP.Requests;

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
    }
}
