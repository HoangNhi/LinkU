using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FE.Controllers
{
    [Authorize]
    public class BaseController<T> : Controller
    {
        
    }
}
