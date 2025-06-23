namespace FE.Services.ViewRender
{
    public interface IVIEWRENDERService
    {
        Task<string> RenderToStringAsync(string viewPath, object model, Dictionary<string, object>? viewBagValues = null);
    }
}
