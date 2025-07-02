using AutoMapper;
using BE.Helpers;
using ENTITIES.DbContent;
using Microsoft.Data.SqlClient;
using MODELS.BASE;
using MODELS.MESSAGE.Dtos;
using MODELS.MESSAGEREACTION.Dtos;
using MODELS.MESSAGEREACTION.Requests;
using MODELS.USER.Dtos;
using Newtonsoft.Json;

namespace BE.Services.MessageReaction
{
    public class MESSAGEREACTIONService : IMESSAGEREACTIONService
    {
        private readonly LINKUContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        public MESSAGEREACTIONService(LINKUContext context, IMapper mapper, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
        }

        public BaseResponse<MODELMessageReaction> Insert(POSTMessageReactionRequest request)
        {
            var response = new BaseResponse<MODELMessageReaction>();
            try
            {
                var checkExist = _context.MessageReactions.Any
                    (x => x.MessageId == request.MessageId 
                    && x.UserId == request.UserId 
                    && !x.IsDeleted);

                if (checkExist)
                {
                    throw new Exception("Phản ứng đã tồn tại cho tin nhắn này.");
                }

                var add = _mapper.Map<ENTITIES.DbContent.MessageReaction>(request);
                add.Id = Guid.NewGuid();
                add.NgayTao = DateTime.Now;
                add.NguoiTao = _contextAccessor.HttpContext.User.Identity.Name;
                add.NgaySua = DateTime.Now;
                add.NguoiSua = _contextAccessor.HttpContext.User.Identity.Name;

                _context.MessageReactions.Add(add);
                _context.SaveChanges();

                response.Data = _mapper.Map<MODELMessageReaction>(add);
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }
            return response;
        }
        public BaseResponse<MODELMessageReaction> Update(POSTMessageReactionRequest request)
        {
            var response = new BaseResponse<MODELMessageReaction>();
            try
            {
                var update = _context.MessageReactions.FirstOrDefault(x => x.Id == request.Id && !x.IsDeleted);
                if (update == null)
                {
                    throw new Exception("Phản ứng không tồn tại.");
                }

                _mapper.Map(request, update);

                update.NgaySua = DateTime.Now;
                update.NguoiSua = _contextAccessor.HttpContext.User.Identity.Name;
                _context.SaveChanges();

                response.Data = _mapper.Map<MODELMessageReaction>(update);
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }
            return response;
        }
        public BaseResponse<MODELMessageReaction> Delete(GetByIdRequest request)
        {
            var response = new BaseResponse<MODELMessageReaction>();
            try
            {
                var delete = _context.MessageReactions.FirstOrDefault(x => x.Id == request.Id && !x.IsDeleted);
                if (delete == null)
                {
                    throw new Exception("Phản ứng không tồn tại.");
                }
                delete.NgayXoa = DateTime.Now;
                delete.NguoiXoa = _contextAccessor.HttpContext.User.Identity.Name;
                delete.IsDeleted = true;
                _context.SaveChanges();
                response.Data = _mapper.Map<MODELMessageReaction>(delete);
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }
            return response;
        }
        
        public BaseResponse<MODELMessageReaction> HandleRequest(POSTMessageReactionRequest request)
        {
            var response = new BaseResponse<MODELMessageReaction>();
            try
            {
                var checkExist = _context.MessageReactions
                    .FirstOrDefault
                    (
                        x => x.MessageId == request.MessageId 
                        && x.UserId == request.UserId
                        && !x.IsDeleted
                    );

                // Chưa có dữ liệu phản ứng cho tin nhắn và người dùng
                if (checkExist == null)
                {
                    var result = Insert(request);
                    if (result.Error)
                    {
                        throw new Exception(result.Message);
                    }
                    response.Data = result.Data;
                }
                // Dữ liệu đã tồn tại
                else
                {
                    // Nếu trùng với phản ứng hiện tại thì xóa
                    if (checkExist.ReactionTypeId == request.ReactionTypeId)
                    {
                        var result = Delete(new GetByIdRequest { Id = checkExist.Id });
                        if (result.Error)
                        {
                            throw new Exception(result.Message);
                        }
                        response.Data = result.Data;
                    }
                    // Không trùng với phản ứng hiện tại thì cập nhật
                    else
                    {
                        request.Id = checkExist.Id;
                        var result = Update(request);
                        if (result.Error)
                        {
                            throw new Exception(result.Message);
                        }
                        response.Data = result.Data;
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

        public BaseResponse<GetListPagingResponse> GetListPaging(POSTMessageReactionGetListPagingRequest request)
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
                    new SqlParameter("@iMessageId", request.MessageId),
                    new SqlParameter("@iReactionTypeId", request.ReactionTypeId),
                    iTotalRow
                };

                var result = _context.ExcuteStoredProcedure<MODELMessageReaction>("sp_MESSAGEREACTION_GetListPaging", parameters).ToList();
                
                result.ForEach(x =>
                {
                    x.User = JsonConvert.DeserializeObject<MODELUser>(x.UserJSON);
                });

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
    }
}
