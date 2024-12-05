using AutoMapper;
using BE.Helpers;
using ENTITIES.DbContent;
using Microsoft.Data.SqlClient;
using MODELS.BASE;
using MODELS.COMMON;
using MODELS.MESSAGE.Dtos;
using MODELS.MESSAGE.Requests;

namespace BE.Services.Message
{
    public class MESSAGEService : IMESSAGEService
    {
        private readonly LINKUContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;

        public MESSAGEService(LINKUContext context, IMapper mapper, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
        }

        public BaseResponse<GetListPagingResponse> GetListPaging(GetListPagingRequest request)
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
                    iTotalRow
                };

                var result = _context.ExcuteStoredProcedure<MODELMessage>("sp_MESSAGE_GetListPaging", parameters).ToList();
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
                var add = _mapper.Map<ENTITIES.DbContent.Message>(request);
                add.Id = request.Id == Guid.Empty ? Guid.NewGuid() : request.Id;
                add.NguoiTao = _contextAccessor.HttpContext.User.Identity.Name;
                add.NgayTao = DateTime.Now;
                add.NguoiSua = _contextAccessor.HttpContext.User.Identity.Name;
                add.NgaySua = DateTime.Now;

                // Lưu dữ liệu
                _context.Messages.Add(add);
                _context.SaveChanges();

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
    }
}
