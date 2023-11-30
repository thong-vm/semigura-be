//using Microsoft.AspNetCore.Mvc;

//namespace semigura.Commons
//{
//    public class CAuthenticationAttribute : ActionFilterAttribute, IAuthenticationFilter
//    {
//        private bool _authen;

//        public void OnAuthentication(AuthenticationContext filterContext)
//        {
//            //Logic for authenticating a user    
//            _authen = (filterContext.ActionDescriptor.GetCustomAttributes(typeof(OverrideAuthenticationAttribute), true).Length == 0);
//        }

//        //Runs after the OnAuthentication method    
//        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
//        {
//            string jwtToken = string.Empty;
//            if (HttpContext.Current.Request.Cookies != null
//                && HttpContext.Current.Request.Cookies[Properties.JWT_TOKEN] != null)
//            {
//                jwtToken = HttpContext.Current.Request.Cookies[Properties.JWT_TOKEN].Value;
//            }

//            if (string.IsNullOrEmpty(jwtToken) || TokenManager.ValidateToken(jwtToken) == null)
//            {
//                filterContext.Result = new UnauthorizedResult(); //new RedirectResult("/S01001/Index");
//            }
//        }
//    }
//}