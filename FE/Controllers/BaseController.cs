using FE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FE.Controllers
{
    [Authorize]
    public class BaseController<T> : Controller
    {
        public readonly ICONSUMEAPIService _consumeAPI;
        public BaseController(ICONSUMEAPIService consumeAPI)
        {
            _consumeAPI = consumeAPI;
        }

        #region User Common
        public string GetProfilePicture(string profilePicture)
        {
            return String.IsNullOrEmpty(profilePicture) || String.IsNullOrEmpty(profilePicture) ? "https://linku.blob.core.windows.net/mediafiles/NoProfilePicture.jpg" : profilePicture;
        }
        public string GetCoverPicture(string coverPicture)
        {
            return String.IsNullOrEmpty(coverPicture) || String.IsNullOrEmpty(coverPicture) ? "https://linku.blob.core.windows.net/mediafiles/NoCoverPicture.jpg" : coverPicture;
        }
        #endregion
    }
}
