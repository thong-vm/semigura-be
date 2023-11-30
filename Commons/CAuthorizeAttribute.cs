using Microsoft.AspNetCore.Authorization;
using semigura.DBContext.Models;

namespace semigura.Commons
{
    public class CAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly string[] allowedroles;
        public CAuthorizeAttribute(params string[] roles)
        {
            this.allowedroles = roles;
        }
        //protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
        //{
        //    bool authorize = false;

        //    var rd = httpContext.Request.RequestContext.RouteData;
        //    string currentAction = rd.GetRequiredString("action");
        //    string currentController = rd.GetRequiredString("controller");
        //    string currentArea = rd.Values["area"] as string;

        //    //~~~~~~~~~~~~~~~~~~~~~~~
        //    UserInfoModel userInfo = null;
        //    List<AuthorInfoModel> authorInfoList = null;
        //    if (httpContext.Session != null && httpContext.Session[Properties.AUTHOR_INFO] != null)
        //    {
        //        authorInfoList = (List<AuthorInfoModel>)httpContext.Session[Properties.AUTHOR_INFO];
        //    }
        //    else if (httpContext.Request.Cookies != null
        //        && httpContext.Request.Cookies[Properties.JWT_TOKEN] != null)
        //    {
        //        var jwtToken = HttpContext.Current.Request.Cookies[Properties.JWT_TOKEN].Value;
        //        userInfo = TokenManager.ValidateToken(jwtToken);

        //        if (userInfo != null)
        //        {
        //            authorInfoList = new S01001Business().GetAuthorInfo(userInfo);
        //        }
        //    }

        //    if (userInfo != null) httpContext.Session[Properties.USER_INFO] = userInfo;
        //    if (authorInfoList != null) httpContext.Session[Properties.AUTHOR_INFO] = authorInfoList;

        //    // 権限をチェック
        //    if (authorInfoList != null && authorInfoList.Any() && (authorInfoList[0].Role == 1 || CheckExistController(authorInfoList, currentController)))
        //    {
        //        return true;
        //    }

        //    return authorize;
        //}

        //protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        //{
        //    filterContext.Result = new RedirectResult("/S01001/UnAuthorized");
        //}

        private bool CheckExistController(List<AuthorInfoModel> authorInfoList, string controllerName)
        {
            bool result = false;
            for (int i = 0; i < authorInfoList.Count; i++)
            {
                if (authorInfoList[i].ControllerName == controllerName)
                {
                    result = true;
                }
                else if (authorInfoList[i].ChildList != null && authorInfoList[i].ChildList.Any())
                {
                    result = CheckExistController(authorInfoList[i].ChildList, controllerName);
                }

                if (result == true)
                {
                    break;
                }
            }

            return result;
        }
    }
}