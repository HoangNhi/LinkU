using AutoMapper;
using BE.Helpers;
using BE.Services.MediaFile;
using BE.Services.User;
using ENTITIES.DbContent;
using Microsoft.Data.SqlClient;
using MODELS.BASE;
using MODELS.CONVERSATION.Dtos;
using MODELS.MEDIAFILE.Dtos;
using MODELS.REACTIONTYPE.Dtos;
using Newtonsoft.Json;

namespace BE.Services.ReactionType
{
    public class REACTIONTYPEService : IREACTIONTYPEService
    {
        private readonly LINKUContext _context;
        private readonly IMapper _mapper;

        public REACTIONTYPEService(LINKUContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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

                var result = _context.ExcuteStoredProcedure<ModelReactionType>("sp_REACTIONTYPE_GetListPaging", parameters)
                    .Select(x => new ModelReactionType
                    {
                        Id = x.Id,
                        TenGoi = x.TenGoi,
                        SapXep = x.SapXep,
                        MediaFile = JsonConvert.DeserializeObject<MODELMediaFile>(x.MediaFileJson)
                    })
                    .ToList();

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

        public BaseResponse<ModelReactionType> GetById(GetByIdRequest request)
        {
            var response = new BaseResponse<ModelReactionType>();
            try
            {
                var reactionType = _context.ReactionTypes.FirstOrDefault(x => x.Id == request.Id && !x.IsDeleted);
                if (reactionType == null)
                {
                    throw new Exception("Dữ liệu không tồn tại");
                }

                var modelReactionType = _mapper.Map<ModelReactionType>(reactionType);
                var mediaFile = _context.MediaFiles.FirstOrDefault(x => x.ReactionTypeId == modelReactionType.Id && !x.IsDeleted);
                if (mediaFile != null)
                {
                    modelReactionType.MediaFile = _mapper.Map<MODELMediaFile>(mediaFile);
                }

                response.Data = modelReactionType;
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
