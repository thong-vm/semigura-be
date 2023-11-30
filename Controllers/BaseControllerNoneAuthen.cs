using Microsoft.AspNetCore.Mvc;
using semigura.Commons;

namespace semigura.Controllers
{
    [CExceptionFilter]
    public class BaseControllerNoneAuthen : Controller
    {
        //private readonly DBEntities _db;

        //public BaseControllerNoneAuthen(DBEntities db)
        //{
        //    _db = db;
        //}
        //public override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    //base.OnActionExecuting(filterContext);

        //    //if (filterContext != null)
        //    //{
        //    //    this.Context = filterContext.HttpContext;

        //    //    // Authen
        //    //    if (Context != null && Context.Session != null && Context.Session[semigura.Commons.Properties.USER_INFO] != null)
        //    //    {
        //    //        UserInfo = (UserInfoModel)Context.Session[semigura.Commons.Properties.USER_INFO];
        //    //    }
        //    //    else if (Context != null && Context.Request.Cookies != null && Context.Request.Cookies[semigura.Commons.Properties.JWT_TOKEN] != null)
        //    //    {
        //    //        UserInfo = TokenManager.ValidateToken(Context.Request.Cookies[semigura.Commons.Properties.JWT_TOKEN].Value);
        //    //        filterContext.HttpContext.Session.SetString(semigura.Commons.Properties.USER_INFO, JsonSerializer.Serialize(UserInfo));
        //    //    }

        //    //    // Author
        //    //    if (Context != null && Context.Session != null && Context.Session[semigura.Commons.Properties.AUTHOR_INFO] != null)
        //    //    {
        //    //        AuthorInfoModelList = (List<AuthorInfoModel>)Context.Session[semigura.Commons.Properties.AUTHOR_INFO];
        //    //    }
        //    //    else if (UserInfo != null)
        //    //    {
        //    //        AuthorInfoModelList = new S01001Business(_db).GetAuthorInfo(UserInfo);
        //    //        filterContext.HttpContext.Session.SetString(semigura.Commons.Properties.AUTHOR_INFO, JsonSerializer.Serialize(AuthorInfoModelList));
        //    //    }
        //    //}
        //}

        //public System.Web.HttpContextBase Context { get; set; }

        //public UserInfoModel UserInfo { get; set; }

        //public List<AuthorInfoModel> AuthorInfoModelList { get; set; }
    }
}