namespace MODELS.BASE
{
    public class BaseRequest
    {
        public string FolderUpload { get; set; } = Guid.NewGuid().ToString();
        public bool IsActived { get; set; } = true;
        public bool IsEdit { get; set; } = false;
        public int? Sort { get; set; } = 0;
    }
}
