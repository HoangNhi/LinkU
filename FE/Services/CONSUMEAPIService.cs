using MODELS.COMMON;
using System.Net.Http.Headers;
using System.Net;
using MODELS.BASE;
using Newtonsoft.Json;

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
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _httpContextAccessor.HttpContext.User.Claims.Where(x => x.Type == "Token").FirstOrDefault().Value.ToString());

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
        public string GetBEUrl()
        {
            return _configuration["BeURL"];
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
            }
            else
            {
                response.Success = false;
                response.Message = "Lỗi hệ thống";
            }
            return response;
        }
    }
}
