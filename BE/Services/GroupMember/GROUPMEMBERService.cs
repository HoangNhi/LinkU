using AutoMapper;
using BE.Services.Group;
using ENTITIES.DbContent;
using MODELS.BASE;
using MODELS.GROUPMEMBER.Dtos;
using MODELS.GROUPMEMBER.Requests;

namespace BE.Services.GroupMember
{
    public class GROUPMEMBERService : IGROUPMEMBERService
    {
        private readonly LINKUContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;

        public GROUPMEMBERService(LINKUContext context, IMapper mapper, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
        }

        public BaseResponse<GetListPagingResponse> GetListPaging(GetListPagingRequest request)
        {
            throw new NotImplementedException();
        }

        public BaseResponse<MODELGroupMember> GetById(GetByIdRequest request)
        {
            var response = new BaseResponse<MODELGroupMember>();
            try
            {
                var group = _context.GroupMembers.FirstOrDefault(gm => gm.Id == request.Id && !gm.IsDeleted);
                if (group == null)
                {
                    throw new Exception("Thành viên không tồn tại");
                }
                response.Data = _mapper.Map<MODELGroupMember>(group);
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }
            return response;
        }

        public BaseResponse<POSTGroupMemberRequest> GetByPost(GetByIdRequest request)
        {
            var response = new BaseResponse<POSTGroupMemberRequest>();
            try
            {
                var group = _context.GroupMembers.FirstOrDefault(gm => gm.Id == request.Id && !gm.IsDeleted);
                if (group == null)
                {
                    var newGroupMember = new POSTGroupMemberRequest
                    {
                        Id = Guid.NewGuid(),
                        IsEdit = false
                    };
                    response.Data = newGroupMember;
                }
                else
                {
                    response.Data = _mapper.Map<POSTGroupMemberRequest>(group);
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

        public BaseResponse<MODELGroupMember> Insert(POSTGroupMemberRequest request)
        {
            var response = new BaseResponse<MODELGroupMember>();
            try
            {
                // Kiểm tra nhóm có tồn tại không
                var group = _context.Groups.Find(request.GroupId);
                if (group == null)
                {
                    throw new Exception("Nhóm không tồn tại");
                }

                // Kiểm tra người dùng có tồn tại trong nhóm không
                var checkExistUserInGroup = _context.GroupMembers.Any(
                    gm => gm.GroupId == request.GroupId 
                    && gm.UserId == request.UserId
                    && !gm.IsDeleted
                );

                if (checkExistUserInGroup)
                {
                    throw new Exception("Người dùng đã tồn tại trong nhóm này");
                }

                // Thêm dữ liệu vào bảng GroupMembers
                var add = _mapper.Map<ENTITIES.DbContent.GroupMember>(request);

                add.Id = request.Id == Guid.Empty ? Guid.NewGuid() : request.Id;
                add.NgayTao = DateTime.Now;
                add.NguoiTao = _contextAccessor.HttpContext.User.Identity.Name;
                add.NgaySua = DateTime.Now;
                add.NguoiSua = _contextAccessor.HttpContext.User.Identity.Name;

                _context.GroupMembers.Add(add);
                _context.SaveChanges();

                response.Data = _mapper.Map<MODELGroupMember>(add);
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }
            return response;
        }

        public BaseResponse<MODELGroupMember> Update(POSTGroupMemberRequest request)
        {
            var response = new BaseResponse<MODELGroupMember>();
            try
            {
                var update = _context.GroupMembers.FirstOrDefault(gm => gm.Id == request.Id);

                if (update == null)
                {
                    throw new Exception("Nhóm không tồn tại");
                }

                _mapper.Map(request, update);

                update.NgaySua = DateTime.Now;
                update.NguoiSua = _contextAccessor.HttpContext.User.Identity.Name;

                _context.GroupMembers.Update(update);
                _context.SaveChanges();

                response.Data = _mapper.Map<MODELGroupMember>(update);
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
                    var delete = _context.GroupMembers.Find(id);
                    if (delete != null)
                    {
                        delete.IsDeleted = true;
                        delete.NguoiXoa = _contextAccessor.HttpContext.User.Identity.Name;
                        delete.NgayXoa = DateTime.Now;
                        // Lưu dữ liệu
                        _context.GroupMembers.Update(delete);
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
