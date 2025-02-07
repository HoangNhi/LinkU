using AutoMapper;
using BE.Helpers;
using ENTITIES.DbContent;
using Microsoft.Data.SqlClient;
using MODELS.BASE;
using MODELS.USER.Dtos;

namespace BE.Services.MessageList
{
    public class MESSAGELISTService : IMESSAGELISTService
    {
        private readonly LINKUContext _context;
        private readonly IMapper _mapper;

        public MESSAGELISTService(LINKUContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        public BaseResponse<GetListPagingResponse> Search(GetListPagingRequest request)
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

                var result = _context.ExcuteStoredProcedure<MODELUser>("sp_MESSAGELIST_Search", parameters).ToList();
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
