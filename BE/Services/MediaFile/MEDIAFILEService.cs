﻿using AutoMapper;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ENTITIES.DbContent;
using Microsoft.AspNetCore.Mvc;
using MODELS.BASE;
using MODELS.MEDIAFILE.Dtos;
using MODELS.MEDIAFILE.Requests;

namespace BE.Services.MediaFile
{
    public class MEDIAFILEService : IMEDIAFILEService
    {
        private readonly BlobContainerClient _containerClient;
        private readonly LINKUContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;

        public MEDIAFILEService(BlobServiceClient blobServiceClient, IConfiguration config, LINKUContext context, IMapper mapper, IHttpContextAccessor contextAccessor)
        {
            var containerName = config["AzureBlobStorage:ContainerName"];

            _containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            _containerClient.CreateIfNotExists(PublicAccessType.Blob);
            _context = context;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
        }


        public BaseResponse<POSTMediaFileRequest> GetByPost(GetByIdRequest request)
        {
            var response = new BaseResponse<POSTMediaFileRequest>();
            try
            {
                var result = new POSTMediaFileRequest();
                var data = _context.MediaFiles.Find(request.Id);
                if (data == null)
                {
                    result.Id = Guid.NewGuid();
                    result.IsEdit = false;
                }
                else
                {
                    result = _mapper.Map<POSTMediaFileRequest>(data);
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

        public BaseResponse<MODELMediaFile> Insert(POSTMediaFileRequest request)
        {
            var response = new BaseResponse<MODELMediaFile>();
            try
            {
                var add = _mapper.Map<ENTITIES.DbContent.MediaFile>(request);
                add.Id = request.Id == Guid.Empty ? Guid.NewGuid() : request.Id;
                add.NgayTao = DateTime.Now;
                add.NguoiTao = _contextAccessor.HttpContext.User.Identity.Name;
                add.NgaySua = DateTime.Now;
                add.NguoiSua = _contextAccessor.HttpContext.User.Identity.Name;

                // Thêm vào bảng MediaFiles
                _context.MediaFiles.Add(add);
                _context.SaveChanges();
                response.Data = _mapper.Map<MODELMediaFile>(add);
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }
            return response;
        }

        public BaseResponse<MODELMediaFile> Update(POSTMediaFileRequest request)
        {
            var response = new BaseResponse<MODELMediaFile>();
            try
            {
                var update = _context.MediaFiles.Find(request.Id);
                if (update == null)
                {
                    throw new Exception("Không tìm thấy dữ liệu");
                }
                else
                {
                    _mapper.Map(request, update);
                    update.NgaySua = DateTime.Now;
                    update.NguoiSua = _contextAccessor.HttpContext.User.Identity.Name;

                    // Thêm vào bảng MediaFiles
                    _context.MediaFiles.Update(update);
                    _context.SaveChanges();
                    response.Data = _mapper.Map<MODELMediaFile>(update);
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
                    var delete = _context.MediaFiles.Find(id);
                    if (delete == null)
                    {
                        throw new Exception("Không tìm thấy dữ liệu");
                    }
                    else
                    {
                        if (delete.IsActived)
                        {
                            throw new Exception($"Không thể xóa ảnh '{delete.FileName}' vì đang sử dụng");
                        }

                        delete.IsDeleted = true;
                        delete.NguoiXoa = _contextAccessor.HttpContext.User.Identity.Name;
                        delete.NgayXoa = DateTime.Now;

                        _context.MediaFiles.Update(delete);
                    }
                }
                _context.SaveChanges();

                response.Data = $"Xóa thành công '{string.Join(", ", request.Ids)}'";
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<BaseResponse<MODELMediaFile>> UpdatePictureUser(IFormFile file, Guid OwnerId, int FileType)
        {
            var response = new BaseResponse<MODELMediaFile>();
            try
            {
                // Upload file
                var resultUpload = await UploadFileAsync(new List<IFormFile> { file });
                if (resultUpload.Error)
                {
                    throw new Exception(resultUpload.Message);
                }

                // Chuyển các Picture về IsActived = false
                var currentPictures = _context.MediaFiles
                    .Where(x => x.OwnerId == OwnerId && x.FileType == FileType && x.IsActived && !x.IsDeleted);

                foreach (var picture in currentPictures)
                {
                    picture.IsActived = false;
                    picture.NgaySua = DateTime.Now;
                    picture.NguoiSua = _contextAccessor.HttpContext.User.Identity.Name;
                    _context.MediaFiles.Update(picture);
                }

                // Tạo dữ liệu hình ảnh mới
                var add = new ENTITIES.DbContent.MediaFile
                {
                    Id = Guid.NewGuid(),
                    FileName = resultUpload.Data.First().FileName,
                    Url = resultUpload.Data.First().Url,
                    OwnerId = OwnerId,
                    FileType = FileType,
                    IsActived = true,
                    NgayTao = DateTime.Now,
                    NguoiTao = _contextAccessor.HttpContext.User.Identity.Name,
                    NgaySua = DateTime.Now,
                    NguoiSua = _contextAccessor.HttpContext.User.Identity.Name,
                };

                // Thêm vào bảng MediaFiles
                _context.MediaFiles.Add(add);

                // Lưu thay đổi
                _context.SaveChanges();

                // Trả về dữ liệu đã thêm
                response.Data = _mapper.Map<MODELMediaFile>(add);
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }
            return response;
        }

        public BaseResponse<MODELMediaFile> UpdatePictureUser(POSTMediaFileRequest request)
        {
            var response = new BaseResponse<MODELMediaFile>();
            try
            {
                var update = _context.MediaFiles.Where(x => x.Id == request.Id
                                                       && !x.IsDeleted && !x.IsActived)
                                                .FirstOrDefault();
                if (update == null)
                {
                    throw new Exception("Không tìm thấy dữ liệu");
                }
                else
                {
                    // Chuyển các Picture về IsActived = false
                    var currentPictures = _context.MediaFiles
                        .Where(x => x.OwnerId == request.OwnerId && x.FileType == request.FileType && x.IsActived && !x.IsDeleted);
                    foreach (var picture in currentPictures)
                    {
                        picture.IsActived = false;
                        picture.NgaySua = DateTime.Now;
                        picture.NguoiSua = _contextAccessor.HttpContext.User.Identity.Name;
                        _context.MediaFiles.Update(picture);
                    }

                    // Update dữ liệu hình ảnh hiện tại
                    _mapper.Map(request, update);
                    update.IsActived = true;
                    update.NgaySua = DateTime.Now;
                    update.NguoiSua = _contextAccessor.HttpContext.User.Identity.Name;

                    // Cập nhật dữ liệu
                    _context.MediaFiles.Update(update);
                    _context.SaveChanges();

                    // Trả về dữ liệu đã cập nhật
                    response.Data = _mapper.Map<MODELMediaFile>(update);
                }
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<BaseResponse<FileStreamResult>> DownloadFileAsync(string fileName)
        {
            var response = new BaseResponse<FileStreamResult>();
            try
            {
                var blobClient = _containerClient.GetBlobClient(fileName);
                if (await blobClient.ExistsAsync())
                {
                    var downloadInfo = await blobClient.DownloadAsync();
                    var contentType = downloadInfo.Value.ContentType;
                    var fileStream = downloadInfo.Value.Content;

                    response.Data = new FileStreamResult(fileStream, contentType)
                    {
                        FileDownloadName = fileName
                    };

                }
                else
                {
                    throw new Exception("File không tồn tại");
                }
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<BaseResponse<List<MODELMediaFile>>> UploadFileAsync(List<IFormFile> files)
        {
            var response = new BaseResponse<List<MODELMediaFile>>();
            try
            {
                var result = new List<MODELMediaFile>();
                foreach (var file in files)
                {
                    string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    var blobClient = _containerClient.GetBlobClient(fileName);

                    var metadata = new Dictionary<string, string>
                    {
                        {"OriginalFileName", file.FileName }
                    };

                    using (var stream = file.OpenReadStream())
                    {
                        await blobClient.UploadAsync(stream, new BlobUploadOptions
                        {
                            HttpHeaders = new BlobHttpHeaders
                            {
                                ContentType = file.ContentType,
                                ContentDisposition = "attachment"
                            },
                            Metadata = metadata
                        });
                    }

                    result.Add(new MODELMediaFile
                    {
                        FileName = file.FileName,
                        Url = blobClient.Uri.ToString(),
                    });
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
    }
}
