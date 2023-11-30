using Microsoft.Extensions.Localization;
using semigura.Commons;
using semigura.DBContext.Entities;
using semigura.DBContext.Models;
using semigura.Models;



namespace semigura.DBContext.Repositories
{
    public class NotificationDao
    {
        private readonly DBEntities context;
        private readonly IStringLocalizer localizer;

        public NotificationDao(DBEntities context, IStringLocalizer localizer)
        {
            this.context = context;
            this.localizer = localizer;
        }

        public List<Notification> GetListNotificationNotCompleted()
        {
            return context.Notification.Where(s => s.Status == semigura.Commons.Properties.NOTIFICATION_STATUS_OPEN || s.Status == semigura.Commons.Properties.NOTIFICATION_STATUS_PROCESSING).OrderByDescending(s => s.CreatedOn).Take(5).ToList();
        }


        public S03005ViewModel GetListNotification(S03005ViewModel model)
        {
            var result = new S03005ViewModel();

            var query = from notification in context.Notification

                        join lotContainer in context.LotContainer on notification.ParentId equals lotContainer.Id into lotContainerJoin
                        from lotContainerLResult in lotContainerJoin.DefaultIfEmpty()

                        join lot in context.Lot on lotContainerLResult.LotId equals lot.Id into lotJoin
                        from lotLResult in lotJoin.DefaultIfEmpty()

                        join container in context.Container on lotContainerLResult.ContainerId equals container.Id into containerJoin
                        from containerLResult in containerJoin.DefaultIfEmpty()

                        join locationOfContainer in context.Location on containerLResult.LocationId equals locationOfContainer.Id into locationOfContainerJoin
                        from locationOfContainerJoinLResult in locationOfContainerJoin.DefaultIfEmpty()

                        join location in context.Location on lotContainerLResult.ContainerId equals location.Id into locationJoin
                        from locationLResult in locationJoin.DefaultIfEmpty()

                        join factory1 in context.Factory on locationLResult.FactotyId equals factory1.Id into factory1Join
                        from factory1LResult in factory1Join.DefaultIfEmpty()

                        join factory2 in context.Factory on locationOfContainerJoinLResult.FactotyId equals factory2.Id into factory2Join
                        from factory2LResult in factory2Join.DefaultIfEmpty()

                        select new NotificationModel(localizer)
                        {
                            Id = notification.Id,
                            LotId = lotLResult.Id,
                            LotCode = lotLResult.Code,
                            ParentId = !string.IsNullOrEmpty(locationLResult.Id) ? locationLResult.Id.Trim() : (!string.IsNullOrEmpty(containerLResult.Id.Trim()) ? containerLResult.Id : null),
                            ParentName = !string.IsNullOrEmpty(locationLResult.Id) ? locationLResult.Name : (!string.IsNullOrEmpty(containerLResult.Id.Trim()) ? containerLResult.Code : null),
                            FactoryId = !string.IsNullOrEmpty(factory1LResult.Id) ? factory1LResult.Id.Trim() : (!string.IsNullOrEmpty(factory2LResult.Id.Trim()) ? factory2LResult.Id : null),
                            Factory = !string.IsNullOrEmpty(factory1LResult.Id) ? factory1LResult.Name : (!string.IsNullOrEmpty(factory2LResult.Id.Trim()) ? factory2LResult.Name : null),
                            Location = notification.Type == 1 ? locationOfContainerJoinLResult.Name : locationLResult.Name,
                            LocationId = notification.Type == 1 ? locationOfContainerJoinLResult.Id : locationLResult.Id,
                            Container = notification.Type == 1 ? containerLResult.Code : string.Empty,
                            ContainerType = containerLResult.Type,
                            TypeId = notification.Type == 1 ? containerLResult.Type : 0,
                            Title = notification.Title,
                            Content = notification.Content,
                            LevelId = notification.Level ?? 0,
                            StatusVal = notification.Status ?? 0,
                            PersonInCharge = notification.PersonInCharge,
                            CreatedOn = notification.CreatedOn,
                            Note = notification.Note
                        };

            if (!string.IsNullOrEmpty(model.FactoryId))
            {
                query = query.Where(s => s.FactoryId == model.FactoryId);
            }

            if (!string.IsNullOrEmpty(model.LocationId))
            {
                query = query.Where(s => s.LocationId == model.LocationId);
            }

            if (!string.IsNullOrEmpty(model.Container))
            {
                query = query.Where(s => s.TypeId == semigura.Commons.Properties.CONTAINER_TYPE_TANK && s.Container.Contains(model.Container));
            }

            if (model.SearchStatus != null && model.SearchStatus.Any())
            {
                query = query.Where(s => model.SearchStatus.Contains(s.StatusVal));
            }

            result.DataNotificationList = query.OrderByDescending(s => s.CreatedOn).Skip(model.IDisplayStart).Take(model.IDisplayLength).ToList();
            result.TotalRecords = query.Count();


            return result;
        }

        public string CloseNotification(Notification entity)
        {
            var systemDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time);
            var item = context.Notification.Where(s => s.Id == entity.Id).FirstOrDefault();
            if (item != null)
            {
                item.Status = entity.Status;
                item.ModifiedOn = entity.ModifiedOn;
                item.ModifiedById = entity.ModifiedById;

                context.SaveChanges();

                // クライアントへ最新データを更新
                //NotificationHub.RefreshDataNotification().ConfigureAwait(false);

                return string.Empty;
            }
            else
            {
                return localizer["C01004"];
            }
        }

        public NotificationModel GetNotification(S03005ViewModel model)
        {
            var result = new NotificationModel(localizer);

            var query = from notification in context.Notification

                        join lotContainer in context.LotContainer on notification.ParentId equals lotContainer.Id into lotContainerJoin
                        from lotContainerLResult in lotContainerJoin.DefaultIfEmpty()

                        join lot in context.Lot on lotContainerLResult.LotId equals lot.Id into lotJoin
                        from lotLResult in lotJoin.DefaultIfEmpty()

                        join container in context.Container on lotContainerLResult.ContainerId equals container.Id into containerJoin
                        from containerLResult in containerJoin.DefaultIfEmpty()

                        join locationOfContainer in context.Location on containerLResult.LocationId equals locationOfContainer.Id into locationOfContainerJoin
                        from locationOfContainerJoinLResult in locationOfContainerJoin.DefaultIfEmpty()

                        join location in context.Location on lotContainerLResult.ContainerId equals location.Id into locationJoin
                        from locationLResult in locationJoin.DefaultIfEmpty()

                        join factory1 in context.Factory on locationLResult.FactotyId equals factory1.Id into factory1Join
                        from factory1LResult in factory1Join.DefaultIfEmpty()

                        join factory2 in context.Factory on locationOfContainerJoinLResult.FactotyId equals factory2.Id into factory2Join
                        from factory2LResult in factory2Join.DefaultIfEmpty()

                        where notification.Id == model.Id
                        select new NotificationModel(localizer)
                        {
                            Id = notification.Id,
                            LotId = lotLResult.Id,
                            LotCode = lotLResult.Code,
                            ParentId = !string.IsNullOrEmpty(locationLResult.Id) ? locationLResult.Id : (!string.IsNullOrEmpty(containerLResult.Id) ? containerLResult.Id : null),
                            ParentName = !string.IsNullOrEmpty(locationLResult.Id) ? locationLResult.Name : (!string.IsNullOrEmpty(containerLResult.Id) ? containerLResult.Code : null),
                            FactoryId = !string.IsNullOrEmpty(factory1LResult.Id) ? factory1LResult.Id : (!string.IsNullOrEmpty(factory2LResult.Id) ? factory2LResult.Id : null),
                            Factory = !string.IsNullOrEmpty(factory1LResult.Id) ? factory1LResult.Name : (!string.IsNullOrEmpty(factory2LResult.Id) ? factory2LResult.Name : null),
                            Location = notification.Type == 1 ? locationOfContainerJoinLResult.Name : locationLResult.Name,
                            LocationId = notification.Type == 1 ? locationOfContainerJoinLResult.Id : locationLResult.Id,
                            ContainerId = notification.Type == 1 ? containerLResult.Id : containerLResult.Id,
                            Container = notification.Type == 1 ? containerLResult.Code : string.Empty,
                            ContainerType = containerLResult.Type,
                            TypeId = notification.Type == 1 ? containerLResult.Type : 0,
                            Type_ParentId = notification.Type,
                            Title = notification.Title,
                            Content = notification.Content,
                            LevelId = notification.Level ?? 0,
                            StatusVal = notification.Status ?? 0,
                            PersonInCharge = notification.PersonInCharge,
                            CreatedOn = notification.CreatedOn,
                            Note = notification.Note
                        };


            return query.FirstOrDefault();
        }


        public string Update(Notification entity)
        {
            var systemDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time);
            var item = context.Notification.Where(s => s.Id == entity.Id).FirstOrDefault();
            if (item != null)
            {
                item.Status = entity.Status;
                item.Note = entity.Note;
                item.ModifiedOn = entity.ModifiedOn;
                item.ModifiedById = entity.ModifiedById;

                context.SaveChanges();

                // クライアントへ最新データを更新
                //NotificationHub.RefreshDataNotification().ConfigureAwait(false);
            }
            else
            {
                return localizer["C01004"];
            }

            return string.Empty;
        }

        public string UpdateContent(Notification entity)
        {
            var systemDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time);
            var item = context.Notification.Where(s => s.Id == entity.Id).FirstOrDefault();
            if (item != null)
            {
                item.Content = entity.Content;
                item.ModifiedOn = entity.ModifiedOn;
                item.ModifiedById = entity.ModifiedById;

                context.SaveChanges();
            }
            else
            {
                return localizer["C01004"];
            }

            return string.Empty;
        }
        public string Add(Notification entity)
        {
            entity.Id = Utils.GenerateId(context);
            context.Notification.Add(entity);
            context.SaveChanges();

            return string.Empty;
        }

        public string Delete(string id)
        {
            var systemDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time);
            var item = context.Notification.Where(s => s.Id == id).FirstOrDefault();
            if (item != null)
            {
                context.Notification.Remove(item);

                context.SaveChanges();

                // クライアントへ最新データを更新
                //NotificationHub.RefreshDataNotification().ConfigureAwait(false); // TODO
            }

            return string.Empty;
        }

        public string DeleteByLotContainerId(string lotcontainerId)
        {
            var item = context.Notification.Where(s => s.ParentId == lotcontainerId);
            if (item.Any())
            {
                //item.ForEach(s => context.Notification.Remove(s));
                context.Notification.RemoveRange(item);
                context.SaveChanges();
            }
            else
            {
                return localizer["C01004"];
            }

            return string.Empty;
        }

        public Notification GetNotificationNotClosed(string lotContainerId)
        {
            return context.Notification.Where(s => s.ParentId == lotContainerId && s.Status != semigura.Commons.Properties.NOTIFICATION_STATUS_CLOSED).OrderByDescending(s => s.ModifiedOn).ThenByDescending(s => s.CreatedOn).FirstOrDefault();
        }
    }
}