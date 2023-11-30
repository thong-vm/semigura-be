using Microsoft.Extensions.Localization;
using semigura.Commons;
using semigura.DBContext.Entities;
using semigura.DBContext.Models;
using semigura.DBContext.Repositories;
using semigura.Models;

namespace semigura.DBContext.Business
{
    public class S03005Business : BaseBusiness
    {
        IStringLocalizer localizer;
        public S03005Business(DBEntities db,
            ILogger<S03005Business> logger,
            IStringLocalizer<S03005Business> localizer
            ) : base(db, logger, localizer)
        {
            this.localizer = localizer;
        }

        public List<Notification> GetListNotificationNotCompleted()
        {
            try
            {
                var dao = new NotificationDao(context, localizer);

                return dao.GetListNotificationNotCompleted();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
        }



        public S03005ViewModel GetListNotification(S03005ViewModel model)
        {
            try
            {
                var dao = new NotificationDao(context, localizer);

                return dao.GetListNotification(model);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
        }

        public S03005ViewModel GetNotification(S03005ViewModel model)
        {
            S03005ViewModel result = new S03005ViewModel();
            try
            {
                var dao = new NotificationDao(context, localizer);

                var notification = dao.GetNotification(model);
                if (notification != null)
                {
                    result = new S03005ViewModel();
                    result.Id = notification.Id;
                    result.LotId = notification.LotId;
                    result.LotCode = notification.LotCode;
                    result.FactoryId = notification.FactoryId;
                    result.LocationId = notification.LocationId;
                    result.Location = notification.Location;
                    result.TypeId = notification.TypeId;
                    result.Type = notification.TypeId;
                    result.Container = notification.Container;
                    result.ContainerId = notification.ContainerId;
                    result.Level = notification.LevelId;
                    result.Title = notification.Title;
                    result.Content = notification.Content;
                    result.Status = notification.StatusVal;
                    result.Note = notification.Note;
                    result.CreatedOn = notification.CreatedOn;
                    result.Type_ParentId = notification.Type_ParentId;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
            return result;
        }



        public string Save(S03005ViewModel model, UserInfoModel UserInfoModel)
        {
            string message = string.Empty;
            try
            {
                var dao = new NotificationDao(context, localizer);

                if (!string.IsNullOrEmpty(model.Id))
                {
                    //Notification entity = model.Map(new Notification());
                    Notification entity = new Notification();
                    entity.ModifiedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time);
                    entity.ModifiedById = UserInfoModel.Id;

                    message = dao.Update(entity);
                }

            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }

            return message;
        }

        public string Delete(S03005ViewModel model)
        {
            string message = string.Empty;
            try
            {
                var dao = new NotificationDao(context, localizer);
                message = dao.Delete(model.Id);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
            return message;
        }

        public string CloseNotification(S03005ViewModel model, UserInfoModel UserInfoModel)
        {

            string message = string.Empty;
            {
                var dao = new NotificationDao(context, localizer);
                //Notification entity = model.Map(new Notification());
                Notification entity = new Notification();

                entity.Status = semigura.Commons.Properties.NOTIFICATION_STATUS_CLOSED;
                entity.ModifiedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time);
                entity.ModifiedById = UserInfoModel.Id;

                message = dao.CloseNotification(entity);
            }

            return message;
        }


        public List<Factory> GetListFactory()
        {
            {
                var dao = new FactoryDao(context);
                return dao.GetListFactory();
            }
        }

        public List<Container> GetListContainerTank(string factoryId)
        {
            {
                var dao = new ContainerDao(context, localizer);
                return dao.GetListContainer(factoryId, semigura.Commons.Properties.CONTAINER_TYPE_TANK);
            }
        }

        public List<Container> GetListContainerSeiguiku(string factoryId)
        {
            {
                var dao = new ContainerDao(context, localizer);
                return dao.GetListContainer(factoryId, semigura.Commons.Properties.CONTAINER_TYPE_SEIGIKU);
            }
        }


        public List<semigura.DBContext.Entities.Location> GetListLocation(string factoryId)
        {
            {
                var dao = new LocationDao(context);
                return dao.GetListLocation(factoryId);
            }
        }

    }
}