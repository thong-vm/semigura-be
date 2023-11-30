using Microsoft.Extensions.Localization;
using semigura.DBContext.Entities;
using semigura.DBContext.Repositories;
using semigura.Models;

namespace semigura.DBContext.Business
{
    public class S03002Business : BaseBusiness
    {
        IStringLocalizer localizer;
        public S03002Business(
            DBEntities dbContext,
            ILogger<S03002Business> logger,
            IStringLocalizer<S03002Business> localizer
            ) : base(dbContext, logger, localizer)
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

        public S03002ViewModel GetDataByFactoryId(string factoryId)
        {
            try
            {
                var lotDao = new LotDao(context, localizer);
                var locationDao = new LocationDao(context);
                S03002ViewModel result = new S03002ViewModel();
                result.ListLot = lotDao.GetListLotByFactory(factoryId);
                result.ListLocation = locationDao.GetListLocation(factoryId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
        }

        public S03002ViewModel GetListLotContainer(S03002ViewModel model)
        {
            try
            {
                var dao = new LotContainerDao(context, localizer);

                return dao.GetLisLotContainerForTank(model);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
        }

        public List<CapacityRefer> GetListCapacityRefer(string containerId)
        {
            try
            {
                var dao = new CapacityReferDao(context);

                return dao.GetListCapacityRefer(containerId);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
        }
    }
}