using Microsoft.Extensions.Localization;
using semigura.DBContext.Entities;
using semigura.DBContext.Models;
using semigura.DBContext.Repositories;

namespace semigura.DBContext.Business
{
    public class S01002Business : BaseBusiness
    {
        IStringLocalizer localizer;
        public S01002Business(DBEntities dbContext,
            ILogger<S01002Business> logger,
            IStringLocalizer<S01002Business> localizer)
            : base(dbContext, logger, localizer)
        {
            this.localizer = localizer;
        }
        public List<Factory> GetListFactory()
        {
            try
            {
                var factoryDao = new FactoryDao(context);

                return factoryDao.GetListFactory();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
        }
        public List<FactoryInfoModel> GetFactoryInfoById(string factoryId)
        {
            try
            {
                var factoryDao = new FactoryDao(context);

                return factoryDao.GetFactoryInfoById(factoryId);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
        }

    }
}