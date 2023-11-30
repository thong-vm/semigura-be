using Microsoft.Extensions.Localization;
using semigura.Commons;
using semigura.DBContext.Entities;
using semigura.DBContext.Models;
using semigura.DBContext.Repositories;
using semigura.Models;

namespace semigura.DBContext.Business
{
    public class S09003Business : BaseBusiness
    {
        IStringLocalizer localizer;
        public S09003Business(DBEntities db,
            ILogger<S09003Business> logger,
            IStringLocalizer<S09003Business> localizer) : base(db, logger, localizer)
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

        public S09003ViewModel GetListContainer(S09003ViewModel model)
        {
            try
            {
                var dao = new ContainerDao(context, localizer);

                return dao.GetListContainer(model);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
        }

        public S09003ViewModel? GetContainer(string id)
        {
            try
            {
                var dao = new ContainerDao(context, localizer);

                var result = dao.GetContainer(id);
                if (result != null)
                {
                    return result.Map<S09003ViewModel>();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
            return null;
        }

        public List<semigura.DBContext.Entities.Location> GetListLocationByFactoryId(string factoryId)
        {
            try
            {
                var locationDao = new LocationDao(context);
                var result = locationDao.GetListLocation(factoryId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
        }

        public string Save(S09003ViewModel model, UserInfoModel UserInfoModel)
        {
            string message = string.Empty;
            try
            {
                var containerDao = new ContainerDao(context, localizer);
                var lotContainerdao = new LotContainerDao(context, localizer);


                if (!string.IsNullOrEmpty(model.Id))
                {
                    Container entity = model.Map<Container>();
                    entity.ModifiedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time);
                    entity.ModifiedById = UserInfoModel.Id;

                    message = containerDao.Update(entity);

                    if (string.IsNullOrEmpty(message))
                    {
                        lotContainerdao.UpdateLocationByContainer(entity);
                    }
                }
                else
                {
                    Container entity = model.Map<Container>();
                    entity.Type = semigura.Commons.Properties.CONTAINER_TYPE_TANK;
                    entity.CreatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time);
                    entity.CreatedById = UserInfoModel.Id;

                    message = containerDao.Add(entity);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
            return message;
        }

        public string Delete(S09003ViewModel model)
        {
            string message = string.Empty;
            try
            {
                var dao = new ContainerDao(context, localizer);

                message = dao.Delete(model.Id);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
            return message;
        }

    }
}