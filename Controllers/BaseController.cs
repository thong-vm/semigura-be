using Logger;
using Microsoft.AspNetCore.Mvc;
using semigura.DBContext.Entities;

namespace semigura.Controllers
{
    //[CAuthenticationAttribute]
    //[CAuthorizeAttribute]
    //[CExceptionFilter]
    public class BaseController : Controller
    {
        public readonly DBEntities _db;
        public readonly LogFormater _logger;

        public BaseController(DBEntities db, ILogger logger)
        {
            _db = db;
            _logger = new LogFormater(logger);
        }
    }
}