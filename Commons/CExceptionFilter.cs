using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace semigura.Commons
{
    public class CExceptionFilter : ActionFilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            if (!filterContext.ExceptionHandled)
            {
                WriteExceptionLog(filterContext.Exception);
                filterContext.Result = new RedirectResult("/S01001/Error");
                filterContext.ExceptionHandled = true;
            }
        }

        private void WriteExceptionLog(Exception exception)
        {
            Console.Error.WriteLine(exception.Message, exception);
            try
            {
                //if (exception is DbEntityValidationException)
                //{
                //    foreach (var validationErrors in ((DbEntityValidationException)exception).EntityValidationErrors)
                //    {
                //        foreach (var validationError in validationErrors.ValidationErrors)
                //        {
                //            LogUtil.Error(string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage));
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message, ex);
            }

        }
    }
}