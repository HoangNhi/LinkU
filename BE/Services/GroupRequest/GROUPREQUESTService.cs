﻿using AutoMapper;
using BE.Helpers;
using BE.Hubs;
using BE.Services.Conversation;
using BE.Services.Message;
using ENTITIES.DbContent;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using MODELS.BASE;
using MODELS.COMMON;
using MODELS.GROUP.Dtos;
using MODELS.GROUPREQUEST.Dtos;
using MODELS.GROUPREQUEST.Requests;
using MODELS.MESSAGE.Dtos;
using MODELS.MESSAGE.Requests;
using MODELS.USER.Dtos;
using Newtonsoft.Json;

namespace BE.Services.GroupRequest
{
    public class GROUPREQUESTService : IGROUPREQUESTService
    {
        private readonly LINKUContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ICONVERSATIONService _conversationService;
        private readonly IMESSAGEService _messageService;

        public GROUPREQUESTService(LINKUContext context, IMapper mapper, IHttpContextAccessor contextAccessor, ICONVERSATIONService conversationService, IMESSAGEService messageService)
        {
            _context = context;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
            _conversationService = conversationService;
            _messageService = messageService;
        }

        public BaseResponse<GetListPagingResponse> GetListPaging(POSTGroupRequestGetListPagingRequest request)
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
                    iTotalRow
                };

                // Lấy dữ liệu
                var result = _context.ExcuteStoredProcedure<MODELGroupRequestGetListPaging>("sp_GROUPREQUEST_GetListPaging", parameters).ToList();

                // Xử lý dữ liệu
                foreach (var item in result)
                {
                    // Deserialize JSON
                    item.Group = JsonConvert.DeserializeObject<MODELGroup>(item.GroupJson);
                    item.Senders = JsonConvert.DeserializeObject<List<MODELUser>>(item.SendersJson);

                    // Lấy hình ảnh đại diện của nhóm
                    if (string.IsNullOrEmpty(item.Group.AvartarUrl))
                    {
                        var avatar = _conversationService.GetGroupAvartar(new GetByIdRequest { Id = item.Group.Id });
                        if (avatar.Error)
                        {
                            throw new Exception(avatar.Message);
                        }
                        item.Group.Avartar = avatar.Data;
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

        public BaseResponse<MODELGroupRequest> Update(POSTGroupInvitationRequest request)
        {
            var response = new BaseResponse<MODELGroupRequest>();
            try
            {
                var update = _context.GroupRequests.FirstOrDefault(g => g.Id == request.Id && !g.IsDeleted);

                if (update == null)
                {
                    throw new Exception("Không tìm thấy dữ liệu");
                }

                // Update dữ liệu
                update.State = request.State;
                update.NgaySua = DateTime.Now;
                update.NguoiSua = _contextAccessor.HttpContext.User.Identity.Name;
                _context.GroupRequests.Update(update);

                if (update.State == 1)
                {
                    // Thêm dữ liệu vào bảng GroupMembers
                    var add = new ENTITIES.DbContent.GroupMember
                    {
                        Id = Guid.NewGuid(),
                        GroupId = update.GroupId,
                        UserId = update.ReceiverId,
                        NgayTao = DateTime.Now,
                        NguoiTao = _contextAccessor.HttpContext.User.Identity.Name,
                        NgaySua = DateTime.Now,
                        NguoiSua = _contextAccessor.HttpContext.User.Identity.Name
                    };

                    _context.GroupMembers.Add(add);


                    // Tạo tin nhắn thông báo trong nhóm
                    MODELMessageContent content = new MODELMessageContent()
                    {
                        UserId = update.ReceiverId
                    };

                    string textMessage = JsonConvert.SerializeObject(content);

                    var message = _messageService.Insert(new PostMessageRequest
                    {
                        Content = textMessage,
                        SenderId = update.ReceiverId,
                        TargetId = update.GroupId,
                        MessageType = (int)MessageType.Notification,
                        IsSaveChange = false
                    });

                    if (message.Error)
                    {
                        throw new Exception(message.Message);
                    }

                    // Tạo Conversation
                    var conversation = _conversationService.Insert(new MODELS.MESSAGESTATUS.Requests.POSTConversationRequest
                    {
                        UserId = update.ReceiverId,
                        TargetId = update.GroupId,
                        TypeOfConversation = 1,
                        LastReadMessageId = message.Data.Id,
                        IsSaveChange = false
                    });

                    if (conversation.Error)
                    {
                        throw new Exception(conversation.Message);
                    }
                }

                _context.SaveChanges();

                response.Data = _mapper.Map<MODELGroupRequest>(update);
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
