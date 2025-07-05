using AutoMapper;
using BE.Helpers;
using ENTITIES.DbContent;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MODELS.BASE;
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
                var reactionType = _context.ReactionTypes
                    .AsNoTracking()
                    .FirstOrDefault(x => x.Id == request.Id && !x.IsDeleted);

                if (reactionType == null)
                {
                    response.Error = true;
                    response.Message = "Dữ liệu không tồn tại";
                    return response;
                }

                var model = _mapper.Map<ModelReactionType>(reactionType);

                // Chỉ cần map MediaFile nếu chưa có navigation
                if (model.MediaFile == null)
                {
                    var media = _context.MediaFiles
                        .AsNoTracking()
                        .FirstOrDefault(x => x.ReactionTypeId == model.Id && !x.IsDeleted);

                    if (media != null)
                    {
                        model.MediaFile = _mapper.Map<MODELMediaFile>(media);
                    }
                }

                response.Data = model;
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }

            return response;
        }

        public List<ModelReactionType> GetByIds(List<Guid> ids)
        {
            // Bước 1: Load MediaFiles liên quan theo ReactionTypeId
            var mediaFiles = _context.MediaFiles
                .AsNoTracking()
                .Where(mf => !mf.IsDeleted && (mf.ReactionTypeId.HasValue && ids.Contains(mf.ReactionTypeId.Value)))
                .ToList()
                .ToDictionary(mf => mf.ReactionTypeId, mf => mf); // ánh xạ nhanh theo ReactionTypeId

            // Bước 2: Truy vấn ReactionTypes và ánh xạ
            var reactionTypes = _context.ReactionTypes
                .AsNoTracking()
                .Where(x => ids.Contains(x.Id))
                .Select(x => new ModelReactionType
                {
                    Id = x.Id,
                    TenGoi = x.TenGoi,
                    SapXep = x.SapXep,
                    // gán media nếu tồn tại
                    MediaFile = mediaFiles.ContainsKey(x.Id)
                        ? _mapper.Map<MODELMediaFile>(mediaFiles[x.Id])
                        : null
                })
                .ToList();

            return reactionTypes;
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
