namespace MODELS.BASE
{
    public class BaseRequest
    {
        public string FolderUpload { get; set; } = Guid.NewGuid().ToString();
        public bool IsActived { get; set; } = true;
        public bool IsEdit { get; set; } = false;
        public int? Sort { get; set; } = 0;
        public bool IsSaveChange { get; set; } = true;
        public string? Username { get; set; } = string.Empty;
    }
}
