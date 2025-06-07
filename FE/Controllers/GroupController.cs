using FE.Constant;
using FE.Services;
using Microsoft.AspNetCore.Mvc;
using MODELS.BASE;
using MODELS.COMMON;
using MODELS.GROUP.Dtos;
using MODELS.GROUPMEMBER.Requests;
using MODELS.MEDIAFILE.Dtos;
using Newtonsoft.Json;

namespace FE.Controllers
{
    public class GroupController : BaseController<GroupController>
    {
        public GroupController(ICONSUMEAPIService consumeAPI) : base(consumeAPI)
        {
        }

        public IActionResult ShowModalCreateGroup()
        {
            return PartialView("~/Views/Home/Contact/Group/_PopupCreateGroupPartial.cshtml");
        }

        public IActionResult GetListMemberCreateGroup()
        {
            try
            {
                ApiResponse response = _consumeAPI.ExcuteAPI(URL_API.GROUP_GET_LIST_MEMBER_CREATE_GROUP, null, HttpAction.Get);
                if (response.Success)
                {
                    var result = JsonConvert.DeserializeObject<List<MODELMemberCreateGroup>>(response.Data.ToString());
                    return Json(new { IsSuccess = true, Message = "", Data = result });
                }
                else
                {
                    throw new Exception(response.Message);
                }
            }
            catch (Exception ex)
            {
                string message = "Lỗi hệ thống: " + ex.Message;
                return Json(new { IsSuccess = false, Message = message, Data = "" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateGroup(IFormCollection request)
        {
            try
            {
                // Kiểm tra request
                ValidateRequestCreateGroup(request);

                // Tạo Form
                var multiForm = new System.Net.Http.MultipartFormDataContent();

                // Thêm các tham số vào multiForm
                multiForm.Add(new StringContent(request["GroupName"]), "GroupName");
                
                var test = request["MemberIds"];
                var MemberIds = JsonConvert.DeserializeObject<List<Guid>>(request["MemberIds"]);
                if (MemberIds.Count < 2)
                {
                    throw new Exception("Vui lòng chọn ít nhất 2 thành viên");
                }
                else
                {
                    // Thêm chính mình vào danh sách thành viên
                    var Members = MemberIds.Select(
                        id => new POSTGroupMemberRequest 
                        {
                            UserId = id,
                            Role = 1 // Mặc định là Member
                        }
                    ).ToList();

                    Members.Add(new POSTGroupMemberRequest
                    {
                        UserId = Guid.Parse(_consumeAPI.GetUserId()), // Thêm chính mình vào danh sách thành viên
                        Role = 2 // Mặc định là Admin
                    });

                    multiForm.Add(new StringContent(JsonConvert.SerializeObject(Members)), "Members");
                }

                // Thêm file nếu có
                foreach (var file in request.Files)
                {
                    var streamContent = new StreamContent(file.OpenReadStream());

                    // Gán Content-Type cho từng file
                    streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);

                    multiForm.Add(streamContent, "Avatar", file.FileName);
                }

                ApiResponse response = await _consumeAPI.PostFormDataAPI(URL_API.GROUP_CREATE_GROUP_WITH_MEMBER, multiForm);

                if (response.Success)
                {
                    return Json(new { IsSuccess = true, Message = "", Data = "" });
                }
                else
                {
                    throw new Exception(response.Message);
                }
            }
            catch (Exception ex)
            {
                string message = "Lỗi hệ thống: " + ex.Message;
                return Json(new { IsSuccess = false, Message = message, Data = "" });
            }
        }

        void ValidateRequestCreateGroup(IFormCollection request)
        {
            // Kiểm tra xem có file không
            if (!(request.Files == null || request.Files.Count == 0))
            {
                // Kiểm tra từng file
                long maxFileSize = 2 * 1024 * 1024; // 2MB

                foreach (var file in request.Files)
                {
                    if (!MODELS.COMMON.CommonConst.AllowedPictureTypes.Contains(file.ContentType))
                        throw new Exception($"File '{file.FileName}' không hợp lệ. Chỉ cho phép .jpg, .jpeg, .jpe, .jfif và .png");

                    if (file.Length > maxFileSize)
                        throw new Exception($"File '{file.FileName}' vượt quá kích thước cho phép (2MB)");
                }
            }

            if (string.IsNullOrEmpty(request["GroupName"]))
            {
                throw new Exception("Tên nhóm không được để trống");
            }

            if (string.IsNullOrEmpty(request["MemberIds"]))
            {
                throw new Exception("Vui lòng chọn ít nhất 2 thành viên");
            }
        }
    }
}
