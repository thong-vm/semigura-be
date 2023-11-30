using Microsoft.Extensions.Localization;
using semigura.Commons;
using semigura.DBContext.Entities;
using semigura.DBContext.Models;
using semigura.Models;

namespace semigura.DBContext.Repositories
{
    public class LotContainerDao
    {
        private readonly DBEntities context;
        private readonly IStringLocalizer localizer;

        public LotContainerDao(DBEntities context, IStringLocalizer localizer)
        {
            this.context = context;
            this.localizer = localizer;
        }

        public LotContainer GetLotContainer(string lotContainerId)
        {
            return context.LotContainer.Where(s => s.Id == lotContainerId).FirstOrDefault();
        }

        public LotContainerModel GetLotContainerWithDeviceId(string lotContainerId)
        {
            var query = from lotContainer in context.LotContainer

                        join container in context.Container on lotContainer.ContainerId equals container.Id into containerLJoin
                        from containerLResult in containerLJoin.DefaultIfEmpty()

                        join location in context.Location on lotContainer.LocationId equals location.Id into locationLJoin
                        from locationLResult in locationLJoin.DefaultIfEmpty()

                        from terminal in context.Terminal.Where(s => s.ParentId == containerLResult.Id && s.DeleteFlg != true).DefaultIfEmpty()

                        where lotContainer.Id == lotContainerId

                        select new LotContainerModel
                        {
                            Id = lotContainer.Id,
                            LotId = lotContainer.LotId,
                            ContainerId = lotContainer.ContainerId,
                            LocationId = lotContainer.LocationId,
                            LocationName = locationLResult.Name,
                            StartDate = lotContainer.StartDate,
                            EndDate = lotContainer.EndDate,
                            TerminalCode = terminal.Code,
                        };

            return query.FirstOrDefault();
        }

        public S03002ViewModel GetLisLotContainerForTank(S03002ViewModel model)
        {
            var result = new S03002ViewModel();


            var containerQuery = context.Container.AsQueryable();
            containerQuery = containerQuery.Where(s => s.Type == semigura.Commons.Properties.CONTAINER_TYPE_TANK);

            var materialStandValQuery = context.MaterialStandVal.AsQueryable();
            materialStandValQuery = materialStandValQuery.Where(s => s.Type == semigura.Commons.Properties.CONTAINER_TYPE_TANK);


            var query = from lotContainer in context.LotContainer
                        join lot in context.Lot on lotContainer.LotId equals lot.Id
                        join factory in context.Factory on lot.FactotyId equals factory.Id
                        join container in containerQuery on lotContainer.ContainerId equals container.Id
                        join location in context.Location on container.LocationId equals location.Id

                        join material in context.Material on lot.MaterialId equals material.Id into materialLJoin
                        from materiaLResult in materialLJoin.DefaultIfEmpty()

                        join materialStandVal in materialStandValQuery on materiaLResult.Id equals materialStandVal.MaterialId into materialStandValLJoin
                        from materialStandValLResult in materialStandValLJoin.DefaultIfEmpty()

                        where container.DeleteFlg != true

                        select new LotContainerModel
                        {
                            Id = lotContainer.Id,
                            TankId = container.Id,
                            TankCode = container.Code,
                            FactoryId = factory.Id,
                            FactoryName = factory.Name,
                            LotId = lot.Id,
                            LotCode = lot.Code,
                            LocationId = location.Id,
                            LocationName = location.Name,
                            Capacity = container.Capacity,
                            Height = container.Height,
                            StartDate = lotContainer.StartDate,
                            EndDate = lotContainer.EndDate,
                            Rice = materiaLResult.Name,
                            TempMin = lotContainer.TempMin == null ? (materialStandValLResult.TempMin == null ? null : materialStandValLResult.TempMin) : lotContainer.TempMin,
                            TempMax = lotContainer.TempMax == null ? (materialStandValLResult.TempMax == null ? null : materialStandValLResult.TempMax) : lotContainer.TempMax,
                        };

            if (!string.IsNullOrEmpty(model.FactoryId))
            {
                query = query.Where(s => s.FactoryId.Equals(model.FactoryId));
            }

            if (!string.IsNullOrEmpty(model.LocationId))
            {
                query = query.Where(s => s.LocationId.Equals(model.LocationId));
            }

            if (!string.IsNullOrEmpty(model.LotId))
            {
                query = query.Where(s => s.LotId.Equals(model.LotId));
            }

            if (!string.IsNullOrEmpty(model.Code))
            {
                query = query.Where(s => s.TankCode.Contains(model.Code));
            }
            if (model.IsInUse)
            {
                query = query.Where(s => s.EndDate == null);
            }

            result.LotContainerList = query.OrderBy(s => s.TankCode).Skip(model.IDisplayStart).Take(model.IDisplayLength).ToList();
            result.TotalRecords = query.ToList().Count();

            return result;
        }

        public decimal DepthToCapacity(decimal? depth, string tankCode)
        {
            var result = from capacityRefer in context.CapacityRefer
                         join container in context.Container on capacityRefer.Id equals container.Id
                         where capacityRefer.ContainerId == container.Id && capacityRefer.Depth == depth
                         select capacityRefer.Capacity;

            return result.FirstOrDefault();

        }

        public decimal CapacityToDepth(decimal? capacity, string tankCode)
        {
            decimal dataReturn = 0;
            var queryCapRef = context.CapacityRefer.AsQueryable();
            var queryCon = context.Container.AsQueryable();

            var result = from capacityrefer in queryCapRef
                         join container in queryCon
                         on tankCode equals container.Code
                         where capacityrefer.Capacity == capacity && capacityrefer.ContainerId == container.Id
                         select capacityrefer.Depth;

            foreach (decimal value in result)
            {
                dataReturn = value;
            }
            return dataReturn;

        }


        public S03001ViewModel GetLisLotContainerForLot(S03001ViewModel model)
        {
            var result = new S03001ViewModel();

            var query = from lot in context.Lot

                        join factory in context.Factory on lot.FactotyId equals factory.Id into factoryJoin
                        from factoryResult in factoryJoin.DefaultIfEmpty()

                        join lotContainer in context.LotContainer on lot.Id equals lotContainer.LotId into lotContainerJoin
                        from lotContainerResult in lotContainerJoin.DefaultIfEmpty()

                        join container in context.Container on lotContainerResult.ContainerId equals container.Id into containerJoin
                        from containerResult in containerJoin.DefaultIfEmpty()

                        join material in context.Material on lot.MaterialId equals material.Id into materialJoin
                        from materialResult in materialJoin.DefaultIfEmpty()

                        from materialStandValMoromi in context.MaterialStandVal.Where(s => s.Type == semigura.Commons.Properties.MATERIALSTANDVAL_TYPE_TANK && s.MaterialId == materialResult.Id).DefaultIfEmpty()

                        from materialStandValSeigiku in context.MaterialStandVal.Where(s => s.Type == semigura.Commons.Properties.MATERIALSTANDVAL_TYPE_SEIGIKU && s.MaterialId == materialResult.Id).DefaultIfEmpty()

                        from lotContainerTerminal in context.LotContainerTerminal.Where(s => containerResult.Type == semigura.Commons.Properties.CONTAINER_TYPE_SEIGIKU && s.LotContainerId == lotContainerResult.Id).DefaultIfEmpty()

                        join terminal in context.Terminal on lotContainerTerminal.TerminalId equals terminal.Id into terminalJoin
                        from terminalResult in terminalJoin.DefaultIfEmpty()

                        select new LotContainerModel
                        {
                            Id = containerResult.Type == semigura.Commons.Properties.CONTAINER_TYPE_TANK ? lotContainerResult.Id : lotContainerTerminal.Id,
                            FactoryId = factoryResult.Id,
                            FactoryName = factoryResult.Name,
                            LotId = lot.Id,
                            LotCode = lot.Code,
                            LocationId = lotContainerResult.LocationId,
                            Rice = materialResult.Name,
                            Kubun = lot.MaterialClassification,
                            Seimaibuai = lot.RicePolishingRatioName,
                            RiceRatio = lot.RicePolishingRatio,
                            StartDateLot = lot.StartDate,
                            EndDateLot = lot.EndDate,
                            Type = containerResult.Type,
                            TankCode = containerResult.Type == semigura.Commons.Properties.CONTAINER_TYPE_TANK ? containerResult.Code : (string.IsNullOrEmpty(terminalResult.Name) ? terminalResult.Code + "(-)" : terminalResult.Code + " (" + terminalResult.Name + ")"),
                            Capacity = containerResult.Capacity,
                            Height = containerResult.Height,
                            StartDate = containerResult.Type == semigura.Commons.Properties.CONTAINER_TYPE_TANK ? lotContainerResult.StartDate : lotContainerTerminal.StartDate,
                            EndDate = containerResult.Type == semigura.Commons.Properties.CONTAINER_TYPE_TANK ? lotContainerResult.EndDate : lotContainerTerminal.EndDate,
                            TempMin = lotContainerResult.TempMin != null ? lotContainerResult.TempMin : (containerResult.Type == semigura.Commons.Properties.CONTAINER_TYPE_TANK ? materialStandValMoromi.TempMin : materialStandValSeigiku.TempMin),
                            TempMax = lotContainerResult.TempMax != null ? lotContainerResult.TempMax : (containerResult.Type == semigura.Commons.Properties.CONTAINER_TYPE_TANK ? materialStandValMoromi.TempMax : materialStandValSeigiku.TempMax),
                            DeleteFlg = containerResult.Type == semigura.Commons.Properties.CONTAINER_TYPE_TANK ? containerResult.DeleteFlg : terminalResult.DeleteFlg,
                        };

            if (!string.IsNullOrEmpty(model.FactoryID))
            {
                query = query.Where(s => s.FactoryId.Equals(model.FactoryID));
            }

            if (!string.IsNullOrEmpty(model.LotCode))
            {
                query = query.Where(s => s.LotCode.Contains(model.LotCode));
            }

            if (!string.IsNullOrEmpty(model.TankCode))
            {
                query = query.Where(s => s.TankCode.Contains(model.TankCode));
            }

            if (model.IsInUse)
            {
                query = query.Where(s => s.EndDateLot == null);
            }

            query = query.Where(s => s.TankCode != "(-)");

            var listLotCode = context.Lot.OrderByDescending(s => s.Code).Select(s => s.Code).ToList();

            result.LotContainerList = query.OrderByDescending(s => s.LotCode).Skip(model.IDisplayStart).Take(model.IDisplayLength).ToList();
            result.LotContainerList.ForEach(s =>
            {
                if (listLotCode.Contains(s.LotCode))
                {
                    s.No = listLotCode.IndexOf(s.LotCode) + 1;
                }
            });
            result.TotalRecords = query.Count();
            return result;
        }

        public bool AllChildDone(string lotId)
        {
            var query = from lotContainer in context.LotContainer

                        join container in context.Container on lotContainer.ContainerId equals container.Id

                        join lotContainerTerminal in context.LotContainerTerminal on lotContainer.Id equals lotContainerTerminal.LotContainerId //into lotContainerTerminalJoin
                        //from lotContainerTerminalResult in lotContainerTerminalJoin.DefaultIfEmpty()

                        where lotContainer.LotId == lotId && ((container.Type == semigura.Commons.Properties.CONTAINER_TYPE_TANK && lotContainer.EndDate == null) || (container.Type == semigura.Commons.Properties.CONTAINER_TYPE_SEIGIKU && lotContainerTerminal.EndDate == null))
                        select lotContainer;
            if (query.Count() == 0) return true;

            return false;
        }

        public bool GetSegikuMinMaxByLotId(string lotId,
                                    ref Nullable<decimal> minTempSegiku,
                                    ref Nullable<decimal> maxTempSegiku)
        {
            var container = context.Container.AsQueryable();
            container = container.Where(s => s.Type.Equals(semigura.Commons.Properties.TERMINAL_TYPE_SEIGIKU));
            var query = from lotcontainer in context.LotContainer
                        join lot in context.Lot on lotcontainer.LotId equals lot.Id
                        join sContainer in container on lotcontainer.ContainerId equals sContainer.Id
                        where lot.Id == lotId
                        select new
                        {
                            min = lotcontainer.TempMin,
                            max = lotcontainer.TempMax
                        };
            if (!query.Any())
            {
                return false;
            }
            minTempSegiku = query.First().min;
            maxTempSegiku = query.First().max;
            return true;
        }

        public S03001ViewModel.Tank LoadInfoByTankID(string tankID)
        {
            var lotcontainer = context.LotContainer.AsQueryable();
            var query = from s in lotcontainer
                        where s.ContainerId == tankID
                        select new S03001ViewModel.Tank
                        {
                            Id = s.ContainerId,
                            MinTemp = s.TempMin,
                            MaxTemp = s.TempMax
                        };
            S03001ViewModel.Tank result = new S03001ViewModel.Tank();

            if (query.ToList().Count > 0)
            {
                result = query.ToList()[0];
            }
            return result;
        }

        public string FinishDateLotContainer(string lotContainerID)
        {
            var systemDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time);
            var query = context.LotContainer.Where(s => s.Id == lotContainerID).FirstOrDefault();
            if (query != null)
            {
                if (systemDate < query.StartDate)
                {
                    query.EndDate = query.StartDate;
                }
                else
                {
                    query.EndDate = systemDate;
                }

                context.SaveChanges();
            }
            else
            {
                return localizer["C01004"];
            }
            return string.Empty;
        }

        public string RollBackDateLotContainer(string lotContainerID)
        {
            var systemDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time);
            var query = context.LotContainer.Where(s => s.Id == lotContainerID).FirstOrDefault();
            if (query != null)
            {
                if (query.EndDate != null)
                {
                    query.EndDate = null;
                    context.SaveChanges();
                }
            }
            else
            {
                return localizer["C01004"];
            }
            return string.Empty;
        }

        public string FinishDateLotContainerSeigiku(string lotContainerTerminalID)
        {
            var systemDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time);
            var LotContainerTerminal = context.LotContainerTerminal.Where(s => s.Id == lotContainerTerminalID).FirstOrDefault();

            if (LotContainerTerminal != null)
            {
                var lotcontainerID = LotContainerTerminal.LotContainerId;
                var lotContainerFinish = context.LotContainerTerminal.Where(s => s.LotContainerId == lotcontainerID && s.EndDate == null).FirstOrDefault();

                if (lotContainerFinish == null)
                {
                    var query = context.LotContainer.Where(s => s.Id == lotcontainerID).FirstOrDefault();
                    if (query != null)
                    {
                        if (systemDate < query.StartDate)
                        {
                            query.EndDate = query.StartDate;
                        }
                        else
                        {
                            query.EndDate = systemDate;
                        }
                    }

                }

                context.SaveChanges();
            }
            else
            {
                return localizer["C01004"];
            }
            return string.Empty;
        }

        public string GetlstConIdAndlstLotConIdbyLotId(string lotID, ref string containerId, ref string lotContainerId)
        {
            var query = context.LotContainer.Where(s => s.LotId == lotID).FirstOrDefault();
            if (query != null)
            {
                var item = from lotcontainer in context.LotContainer
                           join container in context.Container on lotcontainer.ContainerId equals container.Id
                           where (lotcontainer.LotId == lotID && container.Type == semigura.Commons.Properties.CONTAINER_TYPE_SEIGIKU)
                           select new
                           {
                               LotContainerID = lotcontainer.Id,
                               ContainerID = lotcontainer.ContainerId

                           };
                if (item.Any())
                {
                    containerId = item.First().ContainerID;
                    lotContainerId = item.First().LotContainerID;
                }

            }
            else
            {
                return localizer["C01004"];
            }
            return string.Empty;
        }

        public string Add(LotContainer entity, List<S03001ViewModel.Tank> containerID, ref List<string> lstlotContainerID)
        {
            var systemDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time);
            var query = context.LotContainer.Where(s => s.LotId == entity.LotId);

            if (query.Any())
            {
                return string.Format(localizer["msg_item_exist"], localizer["lotcode"], entity.LotId);
            }
            else
            {
                var queryContainer = context.Container.ToArray();
                foreach (var item in containerID)
                {
                    var locationId = queryContainer.Where(s => s.Id == item.Id).Select(y => y.LocationId).FirstOrDefault();
                    LotContainer entityLotCon = new LotContainer();
                    entityLotCon.Id = Utils.GenerateId(context);
                    entityLotCon.ContainerId = item.Id;
                    entityLotCon.LotId = entity.LotId;
                    entityLotCon.LocationId = locationId;
                    entityLotCon.CreatedOn = entity.CreatedOn;
                    entityLotCon.CreatedById = entity.CreatedById;
                    entityLotCon.StartDate = item.StartDate;
                    entityLotCon.EndDate = item.EndDate;
                    entityLotCon.TempMin = item.MinTemp;
                    entityLotCon.TempMax = item.MaxTemp;
                    context.LotContainer.Add(entityLotCon);
                    context.SaveChanges();
                    lstlotContainerID.Add(entityLotCon.Id);
                }
            }


            return string.Empty;
        }

        public string AddContainerByLotID(LotContainer entity, List<S03001ViewModel.Tank> containerID, ref List<string> lstlotContainerID)
        {
            var systemDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time);
            var queryContainer = context.Container.ToArray();
            foreach (var item in containerID)
            {
                var locationId = queryContainer.Where(s => s.Id == item.Id).Select(y => y.LocationId).FirstOrDefault();
                LotContainer entityLotCon = new LotContainer();
                entityLotCon.Id = Utils.GenerateId(context);
                entityLotCon.ContainerId = item.Id;
                entityLotCon.LotId = entity.LotId;
                entityLotCon.LocationId = locationId;
                entityLotCon.CreatedOn = entity.ModifiedOn;
                entityLotCon.CreatedById = entity.ModifiedById;
                entityLotCon.StartDate = item.StartDate;
                entityLotCon.EndDate = null;
                entityLotCon.TempMin = item.MinTemp;
                entityLotCon.TempMax = item.MaxTemp;
                context.LotContainer.Add(entityLotCon);
                lstlotContainerID.Add(entityLotCon.Id);
            }
            context.SaveChanges();

            return string.Empty;
        }

        public string AddforTerminal(LotContainer entity, string containerID, ref string lstlotContainerID)
        {
            var query = context.LotContainer.Where(s => s.LotId == entity.LotId && s.ContainerId == containerID).FirstOrDefault();
            if (query != null)
            {
                return string.Format(localizer["msg_item_exist"], localizer["lotcode"], entity.LotId);
            }
            else
            {
                LotContainer entityLotCon = new LotContainer();
                entityLotCon.Id = Utils.GenerateId(context);
                entityLotCon.ContainerId = containerID;
                entityLotCon.LotId = entity.LotId;
                entityLotCon.LocationId = entity.LocationId;
                entityLotCon.CreatedOn = entity.CreatedOn;
                entityLotCon.CreatedById = entity.CreatedById;
                entityLotCon.StartDate = entity.StartDate;
                entityLotCon.EndDate = entity.EndDate;
                entityLotCon.TempMin = entity.TempMin;
                entityLotCon.TempMax = entity.TempMax;
                context.LotContainer.Add(entityLotCon);
                lstlotContainerID = entityLotCon.Id;
                context.SaveChanges();
            }

            return string.Empty;
        }

        public bool CheckListTankInUse(string tankID)
        {
            var query = context.LotContainer.Where(s => s.ContainerId == tankID);
            if (query.Any())
            {
                var result = query.Where(s => s.EndDate == null);
                if (result.Any())
                {
                    return true;
                }
            }
            return false;
        }

        public List<string> GetListContainerIdbyLotId(string lotId)
        {
            var query = from lotcontainer in context.LotContainer
                        join container in context.Container
                        on lotcontainer.ContainerId equals container.Id
                        where lotcontainer.LotId == lotId && container.Type == semigura.Commons.Properties.CONTAINER_TYPE_TANK
                        select (container.Id);
            return query.ToList();
        }

        public List<LotContainerModel> GetListLotContainerIdbyLotId(string lotId)
        {
            var query = from lotcontainer in context.LotContainer
                        join container in context.Container
                        on lotcontainer.ContainerId equals container.Id
                        where lotcontainer.LotId == lotId
                        select new LotContainerModel
                        {
                            Id = lotcontainer.Id,
                            Type = container.Type
                        };
            return query.ToList();
        }

        public List<string> GetListContainerIdUsingByLotId(string lotId)
        {
            var query = from lotcontainer in context.LotContainer
                        join container in context.Container
                        on lotcontainer.ContainerId equals container.Id
                        where (lotcontainer.LotId == lotId && container.Type == semigura.Commons.Properties.CONTAINER_TYPE_TANK && lotcontainer.EndDate == null)
                        select (container.Id);
            return query.ToList();
        }

        public string UpdatebyLotIdAndContainerId(LotContainer entity, S03001ViewModel.Tank containerID,
                                                    ref List<string> lotcontainerId, ref List<string> lstContainerIdChange)
        {
            var systemDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time);
            var item = context.LotContainer.Where(s => s.LotId == entity.LotId && s.ContainerId == containerID.IdOld).FirstOrDefault();
            var deleteflag = context.Container.Where(s => s.Id == containerID.Id).Select(x => x.DeleteFlg).FirstOrDefault();
            if (deleteflag == true)
            {
                return string.Empty;
            }
            if (item != null)
            {
                item.ContainerId = containerID.Id;
                var locationId = context.Container.Where(s => s.Id == item.ContainerId).Select(y => y.LocationId).FirstOrDefault();
                item.StartDate = containerID.StartDate;
                if (containerID.EndDate < item.StartDate)
                {
                    item.EndDate = item.StartDate;
                }
                else
                {
                    item.EndDate = containerID.EndDate;
                }
                item.TempMin = containerID.MinTemp;
                item.TempMax = containerID.MaxTemp;
                item.LocationId = locationId;
                item.ModifiedById = entity.ModifiedById;
                item.ModifiedOn = entity.ModifiedOn;

                if (containerID.Id != containerID.IdOld)
                {
                    lotcontainerId.Add(item.Id);
                    lstContainerIdChange.Add(containerID.IdOld);
                }

                context.SaveChanges();
            }
            else
            {
                return localizer["C01004"];
            }

            return string.Empty;
        }

        public string UpdateById(LotContainer entity, string id)
        {
            var systemDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time);
            var item = context.LotContainer.Where(s => s.Id == id).FirstOrDefault();
            if (item != null)
            {
                item.EndDate = entity.EndDate;
                item.LocationId = entity.LocationId;
                item.ModifiedById = entity.ModifiedById;
                item.ModifiedOn = entity.ModifiedOn;
                if (entity.TempMin != null)
                {
                    item.TempMin = entity.TempMin;
                }
                if (entity.TempMax != null)
                {
                    item.TempMax = entity.TempMax;
                }
                context.SaveChanges();
            }
            else
            {
                return localizer["C01004"];
            }

            return string.Empty;
        }

        public List<LotContainerModel> GetLotContainerGoingOn(List<string> listContainerId)
        {
            var query = from lotContainer in context.LotContainer
                        join lot in context.Lot on lotContainer.LotId equals lot.Id
                        join container in context.Container on lotContainer.ContainerId equals container.Id

                        join material in context.Material on lot.MaterialId equals material.Id into materialLJoin
                        from materialLResult in materialLJoin.DefaultIfEmpty()

                        from materialStandVal in context.MaterialStandVal.Where(s => s.MaterialId == materialLResult.Id && s.Type == container.Type).DefaultIfEmpty()

                        from sensorData in context.SensorData
                           .Where(s => s.LotContainerId == lotContainer.Id
                               && ((new decimal?[] { s.Temperature1,
                                    s.Temperature2 != null? s.Temperature1:s.Temperature2,
                                    s.Temperature3 != null? s.Temperature3:s.Temperature1}.Average() < (lotContainer.TempMin != null ? lotContainer.TempMin : materialStandVal.TempMin))
                                   || (new decimal?[] { s.Temperature1, s.Temperature2 != null? s.Temperature2: s.Temperature1,
                                        s.Temperature3 != null? s.Temperature3:s.Temperature1 }.Average() > (lotContainer.TempMax != null ? lotContainer.TempMax : materialStandVal.TempMax))
                                   )
                           ).DefaultIfEmpty()

                        join terminal in context.Terminal on sensorData.TerminalId equals terminal.Id into terminallLJoin
                        from terminalLResult in terminallLJoin.DefaultIfEmpty()

                        where lotContainer.EndDate == null && listContainerId.Contains(lotContainer.Id)
                        select new LotContainerModel
                        {
                            Id = lotContainer.Id,
                            LotId = lotContainer.LotId,
                            LotCode = lot.Code,
                            ContainerId = lotContainer.ContainerId,
                            TankCode = container.Code,
                            ContainerType = container.Type,
                            LocationId = lotContainer.LocationId,
                            TempMin = lotContainer.TempMin != null ? lotContainer.TempMin : materialStandVal.TempMin,
                            TempMax = lotContainer.TempMax != null ? lotContainer.TempMax : materialStandVal.TempMax,
                            TerminalId = terminalLResult.Id,
                            TerminalCode = terminalLResult.Code,
                            TerminalName = terminalLResult.Name,
                            Temperature1 = sensorData.Temperature1,
                            Temperature2 = sensorData.Temperature2 != null ? sensorData.Temperature2 : sensorData.Temperature1,
                            Temperature3 = sensorData.Temperature3 != null ? sensorData.Temperature3 : sensorData.Temperature1,
                            TemperatureAvg = new decimal?[] { sensorData.Temperature1,
                                 sensorData.Temperature2!=null?sensorData.Temperature2:sensorData.Temperature1,
                                 sensorData.Temperature3 !=null?sensorData.Temperature3:sensorData.Temperature1}.Average(),
                            MeasureDate = sensorData.MeasureDate,
                        };

            return query.ToList();
        }

        public bool CheckUsingbyContainerId(string containerId)
        {
            var query = context.LotContainer.Where(s => s.ContainerId == containerId);
            if (query != null && query.Any())
            {
                foreach (var item in query)
                {
                    if (item.EndDate == null)
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        public string Delete(string id, ref List<string> lotcontainerID, ref List<string> containerID)
        {
            var query = context.LotContainer.Where(s => s.LotId == id);
            if (!query.Any())
            {
                return localizer["C01004"];
            }
            else
            {
                foreach (var item in query)
                {
                    lotcontainerID.Add(item.Id);
                    containerID.Add(item.ContainerId);
                    context.LotContainer.Remove(item);

                    DeleteContainerIsDeleted(item.ContainerId);

                    context.SaveChanges();
                }
            }
            return string.Empty;
        }

        public string DeleteByContainerId(string lotId, string containerID, ref string lstLotContainerID)
        {
            var item = context.LotContainer.Where(s => s.LotId == lotId && s.ContainerId == containerID).FirstOrDefault();
            if (item == null)
            {
                return localizer["C01004"];
            }
            else
            {
                lstLotContainerID = item.Id;

                context.LotContainer.Remove(item);

                DeleteContainerIsDeleted(item.ContainerId);

                context.SaveChanges();
            }
            return string.Empty;
        }

        public void DeleteById(string lotContainerId)
        {
            var item = context.LotContainer.Where(s => s.Id == lotContainerId).FirstOrDefault();
            if (item != null)
            {
                context.LotContainer.Remove(item);

                DeleteContainerIsDeleted(item.ContainerId);

                context.SaveChanges();
            }
        }

        public string UpdateLocationByContainer(Container entity)
        {
            var systemDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time);
            var list = context.LotContainer.Where(s => s.ContainerId == entity.Id && s.EndDate == null).ToList();
            if (list != null && list.Any())
            {
                foreach (var item in list)
                {
                    item.LocationId = entity.LocationId;
                    item.ModifiedOn = entity.ModifiedOn;
                    item.ModifiedById = entity.ModifiedById;

                    context.SaveChanges();
                }
            }

            return string.Empty;
        }

        private void DeleteContainerIsDeleted(string containerId)
        {
            var sql = from container in context.Container
                      join lotContainer in context.LotContainer on container.Id equals lotContainer.ContainerId
                      where container.DeleteFlg == true && container.Id == containerId
                      select lotContainer;

            if (sql.Count() == 1)
            {
                var containerDeletedItem = context.Container.Where(s => s.Id == containerId).FirstOrDefault();
                if (containerDeletedItem != null)
                {
                    context.Container.Remove(containerDeletedItem);
                }
            }
        }

        public string GetLocationIdforSegiku(string lotId)
        {
            var query = from lot in context.Lot
                        join lotcontainer in context.LotContainer on lot.Id equals lotcontainer.LotId
                        join container in context.Container on lotcontainer.ContainerId equals container.Id
                        where container.Type == semigura.Commons.Properties.CONTAINER_TYPE_SEIGIKU && lot.Id == lotId
                        select lotcontainer.LocationId;
            if (query == null || !query.Any())
            {
                return string.Empty;
            }
            return query.FirstOrDefault().ToString();
        }

        public string UpdateLocation(string lotID, string locationID)
        {
            //var item = context.LotContainer.Where(s => s.LotId == LotID).FirstOrDefault();
            var item = from lotcontainer in context.LotContainer
                       join container in context.Container on lotcontainer.ContainerId equals container.Id
                       where container.Type == semigura.Commons.Properties.CONTAINER_TYPE_SEIGIKU && lotcontainer.LotId == lotID
                       select lotcontainer;
            if (item == null || !item.Any())
            {
                return localizer["C01004"];
            }
            item.FirstOrDefault().LocationId = locationID;
            context.SaveChanges();

            return string.Empty;
        }

        public LotContainer getLotContainerByContainerId(string containerId)
        {
            LotContainer result = new LotContainer();
            result = context.LotContainer.Where(s => s.ContainerId == containerId && s.EndDate == null).FirstOrDefault();
            //result = query;
            return result;
        }

        public LotContainer getLotContainerByTerminalId(string terminalId)
        {
            LotContainer result = new LotContainer();
            //result = context.LotContainer.Where(s => s.ContainerId == containerId && s.EndDate == null).FirstOrDefault();
            var query = from lotcontainer in context.LotContainer
                        join container in context.Container on lotcontainer.ContainerId equals container.Id
                        join terminal in context.Terminal on container.Id equals terminal.ParentId
                        where terminal.Id == terminalId && lotcontainer.EndDate == null
                        select lotcontainer;
            result = query.FirstOrDefault();
            return result;
        }

        public string getLotCodeByLotContainerId(string lotContainerId)
        {
            string result = string.Empty;
            var query = from lotcontainer in context.LotContainer
                        join lot in context.Lot on lotcontainer.LotId equals lot.Id
                        where lotcontainer.Id == lotContainerId
                        select lot.Code;

            result = query.FirstOrDefault().ToString();
            return result;
        }
    }
}