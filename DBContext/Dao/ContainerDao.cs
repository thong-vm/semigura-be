using Microsoft.Extensions.Localization;
using semigura.Commons;
using semigura.DBContext.Entities;
using semigura.DBContext.Models;
using semigura.Models;

namespace semigura.DBContext.Repositories
{
    public class ContainerDao
    {
        private readonly DBEntities context;
        private readonly IStringLocalizer localizer;

        public ContainerDao(DBEntities context, IStringLocalizer localizer)
        {
            this.context = context;
            this.localizer = localizer;
        }

        public List<Container> GetListContainer()
        {
            return context.Container.OrderBy(s => s.Code).ToList();
        }

        public List<ContainerModel> GetListContainerByLotId(string lotId)
        {
            var result = new S02001ViewModel();
            var query = from container in context.Container
                        join lotContainer in context.LotContainer on container.Id equals lotContainer.ContainerId
                        where container.Type == semigura.Commons.Properties.CONTAINER_TYPE_TANK && lotContainer.LotId == lotId
                        select new ContainerModel
                        {
                            Id = container.Id,
                            Code = container.Code,
                            LotContainerId = lotContainer.Id,
                        };

            return query.ToList();
        }

        public S03006ViewModel GetListContainer(S03006ViewModel model)
        {
            var result = new S03006ViewModel();

            var query = from container in context.Container
                        join location in context.Location on container.LocationId equals location.Id
                        join factory in context.Factory on location.FactotyId equals factory.Id
                        where container.Type == semigura.Commons.Properties.CONTAINER_TYPE_TANK && container.DeleteFlg != true
                        select new ContainerModel
                        {
                            Id = container.Id,
                            Code = container.Code,
                            FactoryId = factory.Id,
                            FactoryName = factory.Name,
                            LocationId = location.Id,
                            LocationName = location.Name,
                            LocationCode = location.Code,
                            Type = container.Type,
                            Capacity = container.Capacity,
                            Height = container.Height,
                            CreatedOn = container.CreatedOn
                        };

            result.ListContainer = query.OrderBy(s => s.Code).ToList();
            return result;
        }

        public S09003ViewModel GetListContainer(S09003ViewModel model)
        {
            var result = new S09003ViewModel();

            var query = from container in context.Container
                        join location in context.Location on container.LocationId equals location.Id
                        join factory in context.Factory on location.FactotyId equals factory.Id
                        where container.Type == semigura.Commons.Properties.CONTAINER_TYPE_TANK && container.DeleteFlg != true
                        select new ContainerModel
                        {
                            Id = container.Id,
                            Code = container.Code,
                            FactoryId = factory.Id,
                            FactoryName = factory.Name,
                            LocationId = location.Id,
                            LocationName = location.Name,
                            LocationCode = location.Code,
                            Type = container.Type,
                            Capacity = container.Capacity,
                            Height = container.Height,
                            CreatedOn = container.CreatedOn
                        };

            if (!string.IsNullOrEmpty(model.Code))
            {
                query = query.Where(s => s.Code.Contains(model.Code));
            }

            if (!model.CapSearch_Start.Equals(null))
            {
                query = query.Where(s => s.Capacity >= model.CapSearch_Start);
            }
            if (!model.CapSearch_End.Equals(null))
            {
                query = query.Where(s => s.Capacity <= model.CapSearch_End);
            }

            if (!model.HeightSearch_Start.Equals(null))
            {
                query = query.Where(s => s.Height >= model.HeightSearch_Start);
            }
            if (!model.HeightSearch_End.Equals(null))
            {
                query = query.Where(s => s.Height <= model.HeightSearch_End);
            }

            result.ListContainer = query.OrderBy(s => s.Code).Skip(model.IDisplayStart).Take(model.IDisplayLength).ToList();
            result.TotalRecords = query.Count();
            return result;
        }



        public ContainerModel GetContainer(string id)
        {
            var query = from container in context.Container
                        join location in context.Location on container.LocationId equals location.Id
                        join factory in context.Factory on location.FactotyId equals factory.Id
                        where container.Type == semigura.Commons.Properties.CONTAINER_TYPE_TANK && container.Id == id
                        select new ContainerModel
                        {
                            Id = container.Id,
                            Code = container.Code,
                            FactoryId = factory.Id,
                            FactoryName = factory.Name,
                            LocationId = location.Id,
                            LocationName = location.Name,
                            LocationCode = location.Code,
                            Type = container.Type,
                            Capacity = container.Capacity,
                            Height = container.Height,
                            CreatedOn = container.CreatedOn
                        };
            return query.FirstOrDefault();
        }

        public string GetContainerCodeById(string containerId)
        {
            var query = context.Container.Where(s => s.Id == containerId).Select(x => x.Code).FirstOrDefault();
            return query;
        }

        public string Add(Container entity)
        {
            var item = context.Container.Where(s => s.Code == entity.Code && s.DeleteFlg != true).FirstOrDefault();
            if (item != null)
            {
                return string.Format(localizer["msg_item_exist"], localizer["tankcode"], entity.Code);
            }
            entity.Type = 1;
            entity.Id = Utils.GenerateId(context);
            context.Container.Add(entity);
            context.SaveChanges();

            return string.Empty;
        }

        public void AddContainerTermiald(Container entity, ref string containerID)
        {
            var item = context.Container.Where(s => s.Code == entity.Code).FirstOrDefault();
            if (item != null)
            {
                containerID = item.Id;
                return;
            }
            entity.Id = Utils.GenerateId(context);
            containerID = entity.Id;
            context.Container.Add(entity);
            context.SaveChanges();

            return;
        }

        public string Update(Container entity)
        {
            var systemDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time);
            var item = context.Container.Where(s => s.Id == entity.Id).FirstOrDefault();
            if (item != null)
            {
                var duplicateItem = context.Container.Where(s => s.Id != item.Id && s.Code == entity.Code && s.DeleteFlg != true).FirstOrDefault();
                if (duplicateItem != null)
                {
                    return string.Format(localizer["msg_item_exist"], localizer["tankcode"], entity.Code);
                }

                item.Code = entity.Code;
                item.LocationId = entity.LocationId;
                item.Capacity = entity.Capacity;
                item.Height = entity.Height;
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

        public string Delete(string id)
        {
            var systemDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time);
            var item = context.Container.Where(s => s.Id == id).FirstOrDefault();
            if (item != null)
            {
                var sql = from lotContainer in context.LotContainer
                          join lot in context.Lot on lotContainer.LotId equals lot.Id
                          where lotContainer.ContainerId == item.Id
                          select new
                          {
                              LotContainerEndDate = lotContainer.EndDate,
                              LotCode = lot.Code,
                          };

                var usingItem = sql.Where(s => s.LotContainerEndDate == null).FirstOrDefault();

                if (usingItem != null)
                {
                    return string.Format(localizer["msg_container_using"], usingItem.LotCode);
                }

                var linkingFlg = sql.Count() > 0;
                if (linkingFlg)
                {
                    item.DeleteFlg = true;
                }
                else
                {
                    context.Container.Remove(item);
                }

                context.SaveChanges();
            }
            else
            {
                return localizer["C01004"];
            }

            return string.Empty;
        }

        public void DeleteContainerId(string containerId)
        {
            var item = context.Container.Where(s => s.Id == containerId && s.Type == semigura.Commons.Properties.CONTAINER_TYPE_SEIGIKU).FirstOrDefault();
            if (item != null)
            {
                context.Container.Remove(item);

                context.SaveChanges();
            }
        }

        public List<Container> GetListContainer(string factoryId, int type)
        {
            var query = from container in context.Container
                        join location in context.Location on container.LocationId equals location.Id
                        where location.FactotyId == factoryId && container.Type == type
                        select container;

            return query.OrderBy(s => s.Code).ToList();
        }
    }
}