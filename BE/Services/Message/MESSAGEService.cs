using AutoMapper;
using BE.Helpers;
using BE.Services.User;
using ENTITIES.DbContent;
using Microsoft.Data.SqlClient;
using MODELS.BASE;
using MODELS.MESSAGE.Dtos;
using MODELS.MESSAGE.Requests;
using Newtonsoft.Json;

namespace BE.Services.Message
{
    public class MESSAGEService : IMESSAGEService
    {
        private readonly LINKUContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUSERService _userService;

        public MESSAGEService(LINKUContext context, IMapper mapper, IHttpContextAccessor contextAccessor, IUSERService userService)
        {
            _context = context;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
            _userService = userService;
        }

        public BaseResponse<GetListPagingResponse> GetListPaging(PostMessageGetListPagingRequest request)
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
                    new SqlParameter("@iUserId", request.UserId),
                    new SqlParameter("@iTargetId", request.TargetId),
                    new SqlParameter("@iConversationType", request.ConversationType),
                    iTotalRow
                };

                var result = _context.ExcuteStoredProcedure<MODELMessage>("sp_MESSAGE_GetListPaging", parameters).ToList();

                if(request.ConversationType == 0)
                {
                    var CurrentUser = _userService.GetById(new GetByIdRequest() { Id = request.UserId }).Data;
                    var TargetId = _userService.GetById(new GetByIdRequest() { Id = request.TargetId }).Data;

                    foreach (var item in result)
                    {
                        item.Sender = item.SenderId == request.UserId ? CurrentUser : TargetId;
                    }
                }
                else
                {
                    // Lấy ra người dùng hiện tại từ HttpContext
                    var currentUserId = _contextAccessor.GetClaim("name");
                    var currentUser = _context.Users.Find(Guid.Parse(currentUserId));

                    // Duyệt qua từng tin nhắn
                    foreach (var item in result)
                    {
                        item.Sender = _userService.GetById(new GetByIdRequest() { Id = item.SenderId }).Data;

                        // Tin nhắn là Welcome hoặc Notification
                        if (item.MessageType == 1 || item.MessageType == 2)
                        {
                            MODELMessageContent content = JsonConvert.DeserializeObject<MODELMessageContent>(item.Content);

                            // Thay đổi nội dung của tin nhắn theo người dùng hiện tại
                            // Case 1: User hiện tại là người tạo ra message này
                            if(currentUser.Id == content.UserId)
                            {
                                // Nếu ta thêm ai đó vào nhóm
                                if (content.TargetId.Count > 0)
                                {
                                    List<string> usernames = new List<string>();
                                    foreach (var userid in content.TargetId)
                                    {
                                        var user = _context.Users.Find(userid);
                                        if (user != null)
                                        {
                                            usernames.Add(string.Concat(user.HoLot, " ", user.Ten));
                                        }
                                    }

                                    // Cập nhật nội dung tin nhắn
                                    item.Content = $"Bạn đã thêm {string.Join(", ", usernames)} vào nhóm";
                                }
                                // Nếu ta tham gia bằng Lời mời (GroupRequest)
                                else
                                {
                                    // Cập nhật nội dung tin nhắn
                                    item.Content = $"Bạn đã tham gia nhóm";
                                }
                            }
                            // Case 2: Bạn là 1 trong những người nhận tin nhắn
                            else if (content.TargetId.Contains(currentUser.Id))
                            {
                                var user = _context.Users.Find(content.UserId);
                                if(user != null)
                                {
                                    // Cập nhật nội dung tin nhắn
                                    item.Content = $"{string.Concat(user.HoLot," ", user.Ten)} đã thêm bạn vào nhóm";
                                }
                            }
                            // Case 3: Bạn không phải là người tạo và cũng không phải là người nhận tin nhắn
                            else
                            {
                                if (content.TargetId.Count > 0)
                                {
                                    var user = _context.Users.Find(content.UserId);
                                    var usernames = new List<string>();
                                    foreach (var userid in content.TargetId)
                                    {
                                        var targetUser = _context.Users.Find(userid);
                                        if (targetUser != null)
                                        {
                                            usernames.Add(string.Concat(targetUser.HoLot, " ", targetUser.Ten));
                                        }
                                    }

                                    if (user != null)
                                    {
                                        // Cập nhật nội dung tin nhắn
                                        item.Content = $"{string.Concat(user.HoLot, " ", user.Ten)} đã thêm {string.Join(", ", usernames)} vào nhóm";
                                    }
                                }
                                else
                                {
                                    var user = _context.Users.Find(content.UserId);
                                    if (user != null)
                                    {
                                        // Cập nhật nội dung tin nhắn
                                        item.Content = $"{string.Concat(user.HoLot, " ", user.Ten)} đã tham gia nhóm";
                                    }
                                }
                            }
                        }
                    }
                }

                GetListPagingResponse resposeData = new GetListPagingResponse();
                resposeData.PageIndex = request.PageIndex;
                resposeData.Data = result;
                resposeData.TotalRow = Convert.ToInt32(iTotalRow.Value);
                response.Data = resposeData;
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }
            return response;
        }
        public BaseResponse<MODELMessage> GetById(GetByIdRequest request)
        {
            var response = new BaseResponse<MODELMessage>();
            try
            {
                var result = new MODELMessage();
                var data = _context.Messages.FindAsync(request.Id);
                if (data == null)
                {
                    throw new Exception("Không tìm thấy dữ liệu");
                }
                else
                {
                    result = _mapper.Map<MODELMessage>(data);
                }
                response.Data = result;
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }
            return response;
        }
        public BaseResponse<PostMessageRequest> GetByPost(GetByIdRequest request)
        {
            var response = new BaseResponse<PostMessageRequest>();
            try
            {
                var result = new PostMessageRequest();
                var data = _context.Messages.FindAsync(request.Id);
                if (data == null)
                {
                    result.IsEdit = false;
                }
                else
                {
                    result = _mapper.Map<PostMessageRequest>(data);
                    result.IsEdit = true;
                }
                response.Data = result;
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }
            return response;
        }
        public BaseResponse<MODELMessage> Insert(PostMessageRequest request)
        {
            var response = new BaseResponse<MODELMessage>();
            try
            {
                if (request.Content == "")
                {
                    throw new Exception("Nội dung không được để trống");
                }

                var add = _mapper.Map<ENTITIES.DbContent.Message>(request);
                add.Id = request.Id == Guid.Empty ? Guid.NewGuid() : request.Id;

                var Sender = _context.Users.Find(request.SenderId);
                add.NguoiTao = Sender.Username;
                add.NgayTao = DateTime.Now;
                add.NguoiSua = Sender.Username;
                add.NgaySua = DateTime.Now;

                // Lưu dữ liệu
                _context.Messages.Add(add);
                if (request.IsSaveChange)
                {
                    _context.SaveChanges();
                }

                response.Data = _mapper.Map<MODELMessage>(add);
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }
            return response;
        }

        public BaseResponse<MODELMessage> Update(PostMessageRequest request)
        {
            var response = new BaseResponse<MODELMessage>();
            try
            {
                var update = _context.Messages.Find(request.Id);
                if (update == null)
                {
                    throw new Exception("Không tìm thấy dữ liệu");
                }
                else
                {
                    _mapper.Map(request, update);
                    update.NguoiSua = _contextAccessor.HttpContext.User.Identity.Name;
                    update.NgaySua = DateTime.Now;

                    // Lưu dữ liệu
                    _context.Messages.Update(update);
                    _context.SaveChanges();

                    response.Data = _mapper.Map<MODELMessage>(update);
                }
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
                    var delete = _context.Messages.Find(id);
                    if (delete != null)
                    {
                        delete.NguoiXoa = _contextAccessor.HttpContext.User.Identity.Name;
                        delete.NgayXoa = DateTime.Now;
                        _context.Messages.Remove(delete);
                    }
                    else
                    {
                        throw new Exception("Không tìm thấy dữ liệu");
                    }
                }

                _context.SaveChanges();
                response.Data = String.Join(',', request.Ids);
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }
            return response;
        }

        public BaseResponse<bool> RoolbackDelete(GetByIdRequest request)
        {
            var response = new BaseResponse<bool>();
            try
            {
                var delete = _context.Messages.Find(request.Id);
                if (delete != null)
                {
                    delete.IsDeleted = true;
                    delete.NguoiXoa = "roolback";
                    delete.NgayXoa = DateTime.Now;
                    // Lưu dữ liệu
                    _context.Messages.Update(delete);
                    _context.SaveChanges();
                }
                else
                {
                    throw new Exception("Không tìm thấy dữ liệu");
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
