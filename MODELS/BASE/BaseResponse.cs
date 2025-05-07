namespace MODELS.BASE
{
    public class BaseResponse<T>
    {
        public T Data { get; set; }
        public bool Error { get; set; } = false;
        public string? Message { get; set; }
    }

    public class BaseResponse
    {
        public bool Error { get; set; } = false;
        public string? Message { get; set; }
    }
}
