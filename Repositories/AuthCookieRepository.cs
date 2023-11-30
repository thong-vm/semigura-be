using Logger;
using semigura.DBContext.Models;
using System.Security.Claims;
using System.Text.Json;

namespace semigura.Repositories
{
    public class AuthCookieRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly LogFormater _logger;

        public AuthCookieRepository(IHttpContextAccessor httpContextAccessor, ILogger<AuthCookieRepository> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = new LogFormater(logger);
        }

        private HttpContext HttpContext => _httpContextAccessor.HttpContext;

        public string Username => _httpContextAccessor.HttpContext.User.Identity.Name;
        public UserInfoModel UserInfo
        {
            get
            {
                string userData = HttpContext.User.FindFirstValue(ClaimTypes.UserData);
                List<AuthorInfoModel> authorInfoModelList = null;
                UserInfoModel userInfo = null;
                try
                {
                    UserDataModel userDataModel = JsonSerializer.Deserialize<UserDataModel>(userData);
                    authorInfoModelList = userDataModel.authorInfoModelList;
                    userInfo = userDataModel.userInfoModel;
                }
                catch (Exception e)
                {
                    _logger.Error(e.Message);
                }
                return userInfo;
            }
        }

        public List<AuthorInfoModel> AuthorInfoModelList
        {
            get
            {
                string userData = HttpContext.User.FindFirstValue(ClaimTypes.UserData);
                List<AuthorInfoModel> authorInfoModelList = null;
                UserInfoModel userInfo = null;
                try
                {
                    UserDataModel userDataModel = JsonSerializer.Deserialize<UserDataModel>(userData);
                    authorInfoModelList = userDataModel.authorInfoModelList;
                    userInfo = userDataModel.userInfoModel;
                }
                catch (Exception e)
                {
                    _logger.Error(e.Message);
                }
                return authorInfoModelList;
            }
        }

    }
}
