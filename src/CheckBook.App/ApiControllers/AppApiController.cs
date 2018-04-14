using System.Web.Http;

namespace CheckBook.App.ApiControllers
{
    [Authorize]
    public abstract class AppApiController : ApiController
    {
    }
}