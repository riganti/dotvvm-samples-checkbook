using Microsoft.AspNet.Identity;
using System.Security.Principal;
using System.Web.Http;

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
        public string GetIdentity()
        {
            return RequestContext.Principal.Identity.AsString();
        }
    }

    internal static class TestExtension
    {
        public static string AsString(this IIdentity identity)
        {
            return $"Name: {identity?.GetUserName()}\nId: {identity?.GetUserId()}\nAuthenticated: {identity?.IsAuthenticated}\nType: {identity?.AuthenticationType}";
        }
    }
}