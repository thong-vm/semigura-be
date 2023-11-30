using Microsoft.Extensions.Localization;
using semigura.DBContext.Entities;
using semigura.DBContext.Repositories;
using semigura.Models;

namespace semigura.DBContext.Business
{
    public class S02003Business : BaseBusiness
    {
        public S02003Business(DBEntities db,
            ILogger<S02003Business> logger,
            IStringLocalizer<S02003Business> localizer) : base(db, logger, localizer) { }

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

        public List<semigura.DBContext.Entities.Location> GetLocationByFactoryId(string factoryId)
        {
            try
            {
                var locationDao = new LocationDao(context);

                return locationDao.GetListLocation(factoryId);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
        }

        public S02003ViewModel GetDataByLocationId(S02003ViewModel model)
        {
            try
            {
                var locationDao = new LocationDao(context);
                S02003ViewModel result = new S02003ViewModel();
                result = locationDao.GetDataByLocationId(model);

                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
        }
        public S02003ViewModel GetDataByRoomName(S02003ViewModel model)
        {
            try
            {
                var locationDao = new LocationDao(context);
                S02003ViewModel result = new S02003ViewModel();
                result = locationDao.GetDataByRoomName(model);

                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
        }
    }
}