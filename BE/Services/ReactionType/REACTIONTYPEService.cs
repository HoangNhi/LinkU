using AutoMapper;
using BE.Helpers;
using BE.Services.Redis;
using ENTITIES.DbContent;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MODELS.BASE;
using MODELS.COMMON;
using MODELS.MEDIAFILE.Dtos;
using MODELS.REACTIONTYPE.Dtos;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace BE.Services.ReactionType
{
    public class REACTIONTYPEService : IREACTIONTYPEService
    {
        private readonly LINKUContext _context;
        private readonly IMapper _mapper;
        private readonly IREDISService _redisService;

        public REACTIONTYPEService(LINKUContext context, IMapper mapper, IREDISService redisService)
        {
            _context = context;
            _mapper = mapper;
            _redisService = redisService;
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

        public async Task<BaseResponse<ModelReactionType>> GetById(GetByIdRequest request)
        {
            var response = new BaseResponse<ModelReactionType>();

            try
            {
                var value = await _redisService.GetAsync<ModelReactionType>(RedisKeyHelper.ReactionTypeById(request.Id.Value));
                if(value == null)
                {
                    var reactionType = await _context.ReactionTypes.FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted);

                    if (reactionType == null)
                    {
                        throw new Exception("Loại biểu cảm không tồn tại");
                    }

                    var result = _mapper.Map<ModelReactionType>(reactionType);
                    var media = await _context.MediaFiles
                            .FirstOrDefaultAsync(x => x.ReactionTypeId == result.Id && !x.IsDeleted);

                    result.MediaFile = _mapper.Map<MODELMediaFile>(media);

                    // Trả về 
                    response.Data = result;

                    // Lưu vào Redis
                    _redisService.SetAsync(
                        RedisKeyHelper.ReactionTypeById(request.Id.Value),
                        JsonConvert.SerializeObject(result)
                    );
                }
                else
                {
                    response.Data = value;
                }
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<List<ModelReactionType>> GetByIds(List<Guid> ids)
        {
            var response = new List<ModelReactionType>();
            foreach (var id in ids)
            {
                var reactionType = await GetById(new GetByIdRequest { Id = id });
                if (reactionType.Data != null)
                {
                    response.Add(reactionType.Data);
                }
            }
            return response;
        }

        public async Task<List<ModelReactionType>> GetByIdsAsync(List<Guid> ids)
        {
            if (ids == null || !ids.Any())
                return new List<ModelReactionType>();

            // Bước 1: Load MediaFiles liên quan theo ReactionTypeId
            var mediaFiles = await _context.MediaFiles
                .AsNoTracking()
                .Where(mf => !mf.IsDeleted && mf.ReactionTypeId.HasValue && ids.Contains(mf.ReactionTypeId.Value))
                .ToDictionaryAsync(mf => mf.ReactionTypeId.Value, mf => mf); // ánh xạ nhanh theo ReactionTypeId

            // Bước 2: Truy vấn ReactionTypes và ánh xạ
            var reactionTypes = await _context.ReactionTypes
                .AsNoTracking()
                .Where(x => ids.Contains(x.Id))
                .Select(x => new ModelReactionType
                {
                    Id = x.Id,
                    TenGoi = x.TenGoi,
                    SapXep = x.SapXep,
                    MediaFile = mediaFiles.ContainsKey(x.Id)
                        ? _mapper.Map<MODELMediaFile>(mediaFiles[x.Id])
                        : null
                })
                .ToListAsync();

            return reactionTypes;
        }

    }
}
