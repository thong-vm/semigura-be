using Microsoft.Extensions.Localization;
using semigura.DBContext.Entities;
using semigura.DBContext.Repositories;
using semigura.Models;

namespace semigura.DBContext.Business
{
    public class S02002Business : BaseBusiness
    {
        IStringLocalizer localizer;
        public S02002Business(DBEntities db,
            ILogger<S02002Business> logger,
            IStringLocalizer<S02001Business> localizer
            ) : base(db, logger, localizer)
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

        public List<Lot> GetLotByFactoryId(string factoryId)
        {
            try
            {
                var lotDao = new LotDao(context, localizer);

                return lotDao.GetListLotByFactory(factoryId);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
        }
        public List<Lot> GetLotByFactoryIdAndStatus(string factoryId, bool isInUse)
        {
            try
            {
                var lotDao = new LotDao(context, localizer);

                return lotDao.GetListLotByFactoryAndStatus(factoryId, isInUse);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
        }
        public S02002ViewModel GetDataByLotId(string lotId)
        {
            try
            {
                var lotDao = new LotDao(context, localizer);
                S02002ViewModel result = new S02002ViewModel();
                result = lotDao.GetDataByLotId(lotId);
                result.AllDataByLotId = lotDao.GetAllSensorByLotId(lotId).AllDataByLotId;

                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
        }
        public S02002ViewModel GetDataByLastDay(string lotId)
        {
            try
            {
                var lotDao = new LotDao(context, localizer);
                S02002ViewModel result = new S02002ViewModel();
                result.AllDataByLotId = lotDao.GetDataByLastDay(lotId).AllDataByLotId;

                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
        }
        public S02002ViewModel GetDataBySearchDay(S02002ViewModel model)
        {
            try
            {
                var lotDao = new LotDao(context, localizer);
                S02002ViewModel result = new S02002ViewModel();
                result.AllDataByLotId = lotDao.GetDataBySearchDay(model).AllDataByLotId;

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