using MODELS.COMMON;
using System.Net.Http.Headers;
using System.Net;
using MODELS.BASE;
using Newtonsoft.Json;
using FE.Constant;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using MODELS.USER.Dtos;

namespace FE.Services
{
    public class CONSUMEAPIService : ICONSUMEAPIService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
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
                    response = ExecuteAPIResponse(responseTask);
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
                    response = ExecuteAPIResponse(responseTask);
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Lỗi hệ thống: " + ex.Message;
            }
            return response;
        }
        public ApiResponse ExecuteAPIResponse(Task<HttpResponseMessage> responseTask)
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
            }else if(result.StatusCode == HttpStatusCode.Unauthorized)
            {
                // Call Refresh Token
                ApiResponse responseAPI = ExcuteAPIWithoutToken(URL_API.USER_REFRESH_TOKEN, GetToken("RefreshToken"), HttpAction.Post);
                if (responseAPI.Success)
                {
                    // Lưu lại token mới
                    var resultData = JsonConvert.DeserializeObject<MODELUser>(responseAPI.Data.ToString());
                    var claims = new List<Claim>();

                    claims.Add(new Claim("UserId", GetUserId()));
                    claims.Add(new Claim("AccessToken", resultData.AccessToken));
                    claims.Add(new Claim("RefreshToken", resultData.RefreshToken));

                    // Create the identity from the user info
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    // Create the principal
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                    // Remove old token in cookie
                    _httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).Wait();

                    // Save token in cookie
                    _httpContextAccessor.HttpContext.SignInAsync(claimsPrincipal, new AuthenticationProperties
                    {
                        IsPersistent = true, // Giữ cookie sau khi trình duyệt đóng
                    });

                    response = ExcuteAPI(responseTask.Result.RequestMessage.RequestUri.AbsolutePath, null, HttpAction.Post);
                }
                else
                {
                    _httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                }
            }
            else
            {
                response.Success = false;
                response.Message = "Lỗi hệ thống";
            }
            return response;
        }

        #region Helper Method
        public string GetBEUrl()
        {
            return _configuration["BeURL"];
        }
        public string GetToken(string nameToken)
        {
            return _httpContextAccessor.HttpContext.User.Claims.Where(x => x.Type == nameToken).FirstOrDefault().Value.ToString();
        }
        public string GetUserId()
        {
            return _httpContextAccessor.HttpContext.User.Claims.Where(x => x.Type == "UserId").FirstOrDefault().Value.ToString();
        }

        #endregion
    }
}
