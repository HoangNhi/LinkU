namespace MODELS.GROUP.Requests
{
    public class WSRequestRerenderTab
    {
        public List<Guid> UserIds { get; set; } = new List<Guid>();
        public string TabName { get; set; } = string.Empty;
    }
}
