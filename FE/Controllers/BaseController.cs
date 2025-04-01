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
            return String.IsNullOrEmpty(profilePicture) || String.IsNullOrEmpty(profilePicture) ? _consumeAPI.GetImageURL() + "/Files/Common/NoPicture.png" : _consumeAPI.GetImageURL() + "/" + profilePicture;
        }
        public string GetCoverPicture(string coverPicture)
        {
            return String.IsNullOrEmpty(coverPicture) || String.IsNullOrEmpty(coverPicture) ? _consumeAPI.GetImageURL() + "/Files/Common/CoverPicture.jpg" : _consumeAPI.GetImageURL() + "/" + coverPicture;
        }
        #endregion
    }
}
