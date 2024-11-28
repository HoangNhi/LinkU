using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODELS.BASE
{
    public class ApiResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public object? Data { get; set; }

        public ApiResponse()
        {
            Success = false;
            Message = null;
            Data = null;
        }

        public ApiResponse(bool success, object? data, string message = null)
        {
            Success = success;
            Message = message;
            Data = data;
        }

        public ApiResponse(object data)
        {
            Success = true;
            Data = data;
            Message = null;
        }
    }
}
