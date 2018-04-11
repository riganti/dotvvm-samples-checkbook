using System.Security.Principal;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;

namespace CheckBook.App.ApiControllers
{
    [AllowAnonymous]
    public class TestController : AppApiController
    {
        [HttpGet]
        public bool GetIsAuthenticated()
        {
            return RequestContext.Principal.Identity.IsAuthenticated;
        }

        [HttpGet]
        [Authorize]
        public string GetSecuredTest()
        {
            return JsonConvert.SerializeObject(new
            {
                RequestContextIdentity = RequestContext.Principal.Identity?.AsString(),
                HttpContext = HttpContext.Current?.User?.Identity?.AsString(),
                OwinContext = HttpContext.Current?.GetOwinContext()?.Authentication?.User?.Identity?.AsString()
            });
        }
    }

    internal static class TestExtension
    {
        public static string AsString(this IIdentity identity)
        {
            return $"Name: {identity?.GetUserName()}, Id: {identity?.GetUserId()}, Authenticated: {identity?.IsAuthenticated}, Type: {identity?.AuthenticationType}";
        }
    }
}