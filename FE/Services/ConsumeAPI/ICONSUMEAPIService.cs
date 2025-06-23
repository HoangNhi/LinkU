using Microsoft.AspNetCore.Mvc;
using MODELS.BASE;
using MODELS.COMMON;

namespace FE.Services.ConsumeAPI
{
    public interface ICONSUMEAPIService
    {
        ApiResponse ExcuteAPI(string action, object? model, HttpAction method);
        ApiResponse ExcuteAPIWithoutToken(string action, object? model, HttpAction method);
        Task<IActionResult> ExecuteFileDownloadAPI(string action, string fileName);
        Task<ApiResponse> PostFormDataAPI(string action, MultipartFormDataContent content);
        string GetBEUrl();
        string GetToken(string nameToken);
        string GetUserId();
        string GetImageURL();
    }
}
