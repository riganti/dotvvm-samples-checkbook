using CheckBook.DataAccess.Data.User;
using CheckBook.DataAccess.Services;
using Microsoft.AspNet.Identity;
using System.Web.Http;

namespace CheckBook.App.ApiControllers
{
    public class UserController : AppApiController
    {
        [HttpGet]
        public UserInfoData GetInfo()
        {
            var user = UserService.GetUserInfo(RequestContext.Principal.Identity.GetUserId<int>());
            return user;
        }

        [HttpGet]
        public UserStatsData GetStats()
        {
            var stats = UserService.GetUserStats(RequestContext.Principal.Identity.GetUserId<int>());
            return stats;
        }
    }
}