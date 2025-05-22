using AutoMapper;
using ENTITIES.DbContent;
using MODELS.BASE;
using MODELS.GROUP.Dtos;
using MODELS.GROUP.Requests;

namespace BE.Services.Group
{
    public class GROUPService : IGROUPService
    {
        private readonly LINKUContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;

        public GROUPService(LINKUContext context, IMapper mapper, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
        }

        public BaseResponse<GetListPagingResponse> GetListPaging(GetListPagingRequest request)
        {
            throw new NotImplementedException();
        }

        public BaseResponse<MODELGroup> GetById(GetByIdRequest request)
        {
            var response = new BaseResponse<MODELGroup>();
            try
            {
                var group = _context.Groups.FirstOrDefault(g => g.Id == request.Id && !g.IsDeleted);
                if (group == null)
                {
                    throw new Exception("Nhóm không tồn tại");
                }
                response.Data = _mapper.Map<MODELGroup>(group);
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }
            return response;
        }

        public BaseResponse<POSTGroupRequest> GetByPost(GetByIdRequest request)
        {
            var response = new BaseResponse<POSTGroupRequest>();
            try
            {
                var group = _context.Groups.FirstOrDefault(g => g.Id == request.Id && !g.IsDeleted);
                if (group == null)
                {
                    var newGroup = new POSTGroupRequest
                    {
                        Id = Guid.NewGuid(),
                        IsEdit = false
                    };
                    response.Data = newGroup;
                }
                else
                {
                    response.Data = _mapper.Map<POSTGroupRequest>(group);
                    response.Data.IsEdit = true;
                }
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }
            return response;
        }

        public BaseResponse<MODELGroup> Insert(POSTGroupRequest request)
        {
            var response = new BaseResponse<MODELGroup>();
            try
            {
                var add = _mapper.Map<ENTITIES.DbContent.Group>(request);
                
                add.Id = request.Id == Guid.Empty ? Guid.NewGuid() : request.Id;
                add.NgayTao = DateTime.Now;
                add.NguoiTao = _contextAccessor.HttpContext.User.Identity.Name;
                add.NgaySua = DateTime.Now;
                add.NguoiSua = _contextAccessor.HttpContext.User.Identity.Name;

                _context.Groups.Add(add);
                _context.SaveChanges();

                response.Data = _mapper.Map<MODELGroup>(add);
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }
            return response;
        }

        public BaseResponse<MODELGroup> Update(POSTGroupRequest request)
        {
            var response = new BaseResponse<MODELGroup>();
            try
            {
                var update = _context.Groups.FirstOrDefault(g => g.Id == request.Id);

                if (update == null)
                {
                    throw new Exception("Nhóm không tồn tại");
                }

                _mapper.Map(request, update);

                update.NgaySua = DateTime.Now;
                update.NguoiSua = _contextAccessor.HttpContext.User.Identity.Name;

                _context.Groups.Update(update);
                _context.SaveChanges();

                response.Data = _mapper.Map<MODELGroup>(update);
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }
            return response;
        }

        public BaseResponse<string> DeleteList(DeleteListRequest request)
        {
            var response = new BaseResponse<string>();
            try
            {
                foreach (var id in request.Ids)
                {
                    var delete = _context.Groups.Find(id);
                    if (delete != null)
                    {
                        delete.IsDeleted = true;
                        delete.NguoiXoa = _contextAccessor.HttpContext.User.Identity.Name;
                        delete.NgayXoa = DateTime.Now;
                        // Lưu dữ liệu
                        _context.Groups.Update(delete);
                        _context.SaveChanges();
                    }
                    else
                    {
                        throw new Exception($"Không tìm thấy dữ liệu: {id.ToString()}");
                    }
                }
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
