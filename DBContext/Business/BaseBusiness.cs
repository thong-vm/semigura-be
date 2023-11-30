using Logger;
using Microsoft.Extensions.Localization;
using semigura.DBContext.Entities;
using semigura.DBContext.Repositories;
namespace semigura.DBContext.Business
{
    public class BaseBusiness
    {
        public readonly DBEntities context = null!;
        public readonly LogFormater _logger;
        private readonly IStringLocalizer localizer;

        public BaseBusiness(DBEntities db, ILogger logger, IStringLocalizer localizer)
        {
            context = db;
            _logger = new LogFormater(logger);
            this.localizer = localizer;
        }

        public Media GetImage(string id)
        {
            {
                var dao = new MediaDao(context, localizer);

                return dao.GetMedia(id);
            }
        }
    }
}