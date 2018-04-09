using System.Web.Http;

namespace CheckBook.App.ApiControllers
{
    [AllowAnonymous]
    public class TestApiController : AppApiController
    {
        [Route("is-authenticated")]
        [HttpGet]
        public bool IsAuthenticated()
        {
            return RequestContext.Principal.Identity.IsAuthenticated;
        }
    }
}