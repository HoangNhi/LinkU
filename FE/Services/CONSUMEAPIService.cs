using FE.Constant;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using MODELS.BASE;
using MODELS.COMMON;
using MODELS.REFRESHTOKEN.Requests;
using MODELS.USER.Dtos;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Claims;
using System.Text;

namespace FE.Services
{
    public class CONSUMEAPIService : ICONSUMEAPIService
    {
        private IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        public CONSUMEAPIService(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }
        public ApiResponse ExcuteAPI(string action, object? model, HttpAction method)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                using (var client = new HttpClient())
                {
                    // Lấy ra địa chỉ BE
                    client.BaseAddress = new Uri(GetBEUrl());
                    // Thời gian chờ tối đa
                    client.Timeout = TimeSpan.FromMinutes(5);
                    // Set header cho request
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetToken("AccessToken"));

                    client.DefaultRequestHeaders.Accept.Clear();

                    Task<HttpResponseMessage> responseTask;
                    switch (method)
                    {
                        case HttpAction.Get:
                            responseTask = client.GetAsync(action);
                            break;
                        case HttpAction.Post:
                            responseTask = client.PostAsJsonAsync(action, model);
                            break;
                        case HttpAction.Put:
                            responseTask = client.PutAsJsonAsync(action, model);
                            break;
                        case HttpAction.Delete:
                            responseTask = client.DeleteAsync(action);
                            break;
                        default:
                            responseTask = client.PostAsJsonAsync(action, model);
                            break;
                    }

                    responseTask.Wait();
                    response = ExecuteAPIResponse(responseTask).Result;
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Lỗi hệ thống: " + ex.Message;
            }
            return response;
        }
        public ApiResponse ExcuteAPIWithoutToken(string action, object? model, HttpAction method)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                using (var client = new HttpClient())
                {
                    // Lấy ra địa chỉ BE
                    client.BaseAddress = new Uri(GetBEUrl());
                    // Thời gian chờ tối đa
                    client.Timeout = TimeSpan.FromMinutes(5);
                    // Set header cho request
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    client.DefaultRequestHeaders.Accept.Clear();

                    Task<HttpResponseMessage> responseTask;
                    switch (method)
                    {
                        case HttpAction.Get:
                            responseTask = client.GetAsync(action);
                            break;
                        case HttpAction.Post:
                            responseTask = client.PostAsJsonAsync(action, model);
                            break;
                        case HttpAction.Put:
                            responseTask = client.PutAsJsonAsync(action, model);
                            break;
                        case HttpAction.Delete:
                            responseTask = client.DeleteAsync(action);
                            break;
                        default:
                            responseTask = client.PostAsJsonAsync(action, model);
                            break;
                    }

                    responseTask.Wait();
                    response = ExecuteAPIResponse(responseTask).Result;
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Lỗi hệ thống: " + ex.Message;
            }
            return response;
        }
        private async Task<ApiResponse> ExecuteAPIResponse(Task<HttpResponseMessage> responseTask)
        {
            ApiResponse response = new ApiResponse();

            //To store result of web api response.   
            var result = responseTask.Result;

            if (result.IsSuccessStatusCode)
            {
                var readTask = result.Content.ReadAsStringAsync();
                readTask.Wait();

                if (readTask == null)
                {
                    response.Success = false;
                    response.Message = "Lỗi hệ thống";
                }
                else
                {
                    string json = readTask.Result;
                    var resultData = JsonConvert.DeserializeObject<ApiResponse>(json);

                    response.Message = resultData.Message;
                    if (!resultData.Success)
                    {
                        response.Success = false;
                    }
                    else
                    {
                        response.Success = true;
                        response.Data = resultData.Data;
                    }
                }
            }
            else if (result.StatusCode == HttpStatusCode.Unauthorized)
            {
                // Call Refresh Token
                ApiResponse responseAPI = ExcuteAPIWithoutToken(URL_API.USER_REFRESH_TOKEN, new RefreshTokenRequest { Token = GetToken("RefreshToken") }, HttpAction.Post);
                if (responseAPI.Success)
                {
                    // Lưu lại token mới
                    await UpdateToken(responseAPI);
                    // Re-call API
                    using (var client = new HttpClient())
                    {
                        var request = await CreateRequestMessage(result.RequestMessage);
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", GetToken("AccessToken"));
                        var RecallResponseTask = client.SendAsync(request);
                        response = await ExecuteAPIResponse(RecallResponseTask);
                    }
                }
                else
                {
                    await _httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    _httpContextAccessor.HttpContext.Response.Redirect("/Account/Login");
                }
            }
            else
            {
                response.Success = false;
                response.Message = "Lỗi hệ thống";
            }
            return response;
        }
        public async Task<IActionResult> ExecuteFileDownloadAPI(string action, string fileName)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(GetBEUrl());
                    client.Timeout = TimeSpan.FromMinutes(5);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetToken("AccessToken"));

                    var response = await client.GetAsync($"{action}?fileName={fileName}");

                    if (response.IsSuccessStatusCode)
                    {
                        var stream = await response.Content.ReadAsStreamAsync();
                        var contentType = response.Content.Headers.ContentType?.ToString() ?? "application/octet-stream";
                        var downloadFileName = response.Content.Headers.ContentDisposition?.FileNameStar ?? fileName;

                        return new FileStreamResult(stream, contentType)
                        {
                            FileDownloadName = downloadFileName,
                        };
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        // Call Refresh Token
                        ApiResponse responseAPI = ExcuteAPIWithoutToken(URL_API.USER_REFRESH_TOKEN, new RefreshTokenRequest { Token = GetToken("RefreshToken") }, HttpAction.Post);
                        if (responseAPI.Success)
                        {
                            // Lưu lại token mới
                            await UpdateToken(responseAPI);

                            // Re-call API
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetToken("AccessToken"));
                            response = await client.GetAsync($"{action}?fileName={fileName}");

                            if (response.IsSuccessStatusCode)
                            {
                                var stream = await response.Content.ReadAsStreamAsync();
                                var contentType = response.Content.Headers.ContentType?.ToString() ?? "application/octet-stream";
                                var downloadFileName = response.Content.Headers.ContentDisposition?.FileNameStar ?? fileName;

                                return new FileStreamResult(stream, contentType)
                                {
                                    FileDownloadName = downloadFileName
                                };
                            }
                        }
                        else
                        {
                            await _httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                            return new RedirectResult("/Account/Login");
                        }
                    }

                    return new BadRequestResult();
                }
            }
            catch (Exception ex)
            {
                return new ContentResult
                {
                    Content = $"Lỗi hệ thống: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        #region Helper Method
        public string GetBEUrl()
        {
            return _configuration["BeURL"];
        }
        public string GetImageURL()
        {
            return _configuration["ImageURL"];
        }
        public string GetToken(string nameToken)
        {
            return _httpContextAccessor.HttpContext.User.Claims.Where(x => x.Type == nameToken).FirstOrDefault().Value.ToString();
        }
        public string GetUserId()
        {
            return _httpContextAccessor.HttpContext.User.Claims.Where(x => x.Type == "UserId").FirstOrDefault().Value.ToString();
        }
        public string GetName()
        {
            return _httpContextAccessor.HttpContext.User.Claims.Where(x => x.Type == "Name").FirstOrDefault().Value.ToString();
        }
        private async Task UpdateToken(ApiResponse responseAPI)
        {
            // Lưu lại token mới
            var resultData = JsonConvert.DeserializeObject<MODELUser>(responseAPI.Data.ToString());
            var claims = new List<Claim>();

            claims.Add(new Claim("UserId", GetUserId()));
            claims.Add(new Claim("Name", GetName()));
            claims.Add(new Claim("AccessToken", resultData.AccessToken));
            claims.Add(new Claim("RefreshToken", resultData.RefreshToken));

            // Create the identity from the user info
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            // Create the principal
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            // Remove old token in cookie
            await _httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Save token in cookie
            await _httpContextAccessor.HttpContext.SignInAsync(claimsPrincipal, new AuthenticationProperties
            {
                IsPersistent = true, // Giữ cookie sau khi trình duyệt đóng
            });

            _httpContextAccessor.HttpContext.User = claimsPrincipal;
        }

        // Tạo 1 HTTP Request message từ request cũ
        // Error: The request message was already sent. Cannot send the same request message multiple times.
        private async Task<HttpRequestMessage> CreateRequestMessage(HttpRequestMessage originalRequest)
        {
            // Tạo một request mới từ request cũ
            var newRequest = new HttpRequestMessage(originalRequest.Method, originalRequest.RequestUri);

            // Sao chép Headers từ request cũ
            foreach (var header in originalRequest.Headers)
            {
                newRequest.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            // Cập nhật Authorization với token mới
            newRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", GetToken("AccessToken"));

            // Sao chép Content nếu có
            if (originalRequest.Content != null)
            {
                var originalContent = await originalRequest.Content.ReadAsStringAsync();
                newRequest.Content = new StringContent(originalContent, Encoding.UTF8, originalRequest.Content.Headers.ContentType?.MediaType);
            }

            return newRequest;
        }

        #endregion
    }
}
