using Microsoft.Extensions.Localization;
using semigura.Commons;
using semigura.DBContext.Entities;
using semigura.DBContext.Models;
using semigura.Models;
using static semigura.Models.S02002ViewModel;

namespace semigura.DBContext.Repositories
{
    public class LotDao
    {
        private readonly DBEntities context;
        private readonly IStringLocalizer localizer;

        public LotDao(DBEntities context, IStringLocalizer localizer)
        {
            this.context = context;
            this.localizer = localizer;
        }
        public List<Lot> GetListLot()
        {
            return context.Lot.ToList();
        }

        public List<Lot> GetListLotByFactory(string factoryId)
        {
            return context.Lot.Where(s => s.FactotyId == factoryId).ToList();
        }
        public List<Lot> GetListLotByFactoryAndStatus(string factoryId, bool isInUse)
        {
            return context.Lot.Where(s => s.FactotyId == factoryId && (isInUse ? (s.EndDate == null) : (s.StartDate != null))).ToList();
        }

        public S02002ViewModel GetDataByLotId(string lotId)
        {
            Nullable<System.DateTime> nowDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time);
            var query = from lot in context.Lot
                        join lotContainer in context.LotContainer on lot.Id equals lotContainer.LotId
                        from container in context.Container.Where(s => s.Id == lotContainer.ContainerId && s.Type == Commons.Properties.CONTAINER_TYPE_SEIGIKU)
                        from lotContainerTerminal in context.LotContainerTerminal.Where(s => s.LotContainerId == lotContainer.Id)
                        from terminal in context.Terminal.Where(s => s.Id == lotContainerTerminal.TerminalId)
                        from sensorData in context.SensorData.Where(s => s.LotContainerId == lotContainer.Id && s.TerminalId == terminal.Id && s.MeasureDate >= lotContainer.StartDate).DefaultIfEmpty()
                        where lot.Id == lotId
                        select new S02002ViewModel.SenSorInfo
                        {
                            SensorName = terminal.Name,
                            SensorCode = terminal.Code,
                            NewestTemp1 = sensorData.Temperature1,
                            NewestTemp2 = sensorData.Temperature2 != null ? sensorData.Temperature2 : sensorData.Temperature1,
                            MeasuaDate = sensorData.MeasureDate,
                            EndDate = lot.EndDate != null ? lot.EndDate : nowDate,
                            LCTStartDate = lotContainerTerminal.StartDate,
                            LCTEndDate = lotContainerTerminal.EndDate != null ? lotContainerTerminal.EndDate :
                            (lotContainer.EndDate != null ? lotContainer.EndDate : nowDate)
                        };
            var data = query.GroupBy(s => s.SensorCode)
                .Select(a => new
                {
                    SenSorCode = a.Key,
                    sensor = a.Select(b => new
                    {
                        sensorName = b.SensorName,
                        temp1 = b.NewestTemp1,
                        temp2 = b.NewestTemp2,
                        lastestTime = b.MeasuaDate,
                        lotStart = b.LCTStartDate,
                        lotEnd = b.LCTEndDate
                    }).Where(s => s.lastestTime >= s.lotStart && s.lastestTime <= s.lotEnd)
                    .OrderByDescending(c => c.lastestTime).FirstOrDefault()
                }).ToList();

            var locationQuery = from lot in context.Lot
                                join lotContainer in context.LotContainer on lot.Id equals lotContainer.LotId
                                join location in context.Location on lotContainer.LocationId equals location.Id
                                join sensor in context.SensorData on location.Id equals sensor.LotContainerId
                                orderby sensor.MeasureDate descending
                                where lot.Id == lotId
                                select new
                                {
                                    LocationName = location.Code,
                                    Temp = sensor.Temperature1,
                                    Humi = sensor.Humidity,
                                    DateTime = sensor.MeasureDate
                                };
            var locationInfo = locationQuery.GroupBy(s => s.LocationName)
               .Select(a => new
               {
                   LocationName = a.Key,
                   location = a.Select(b => new
                   {
                       temp = b.Temp,
                       humi = b.Humi,
                       lastestTime = b.DateTime
                   }).OrderByDescending(c => c.lastestTime).FirstOrDefault()
               }).ToList();

            var results = new S02002ViewModel();
            results.NewDataByLotId = new S02002ViewModel.NewInfoByLotId();
            results.NewDataByLotId.SenSor = new List<S02002ViewModel.SenSorInfo>();
            foreach (var item in data)
            {
                var sensorInfo = new S02002ViewModel.SenSorInfo();
                sensorInfo.SensorName = item.SenSorCode;
                if (item.sensor != null)
                {
                    sensorInfo.SensorName = item.sensor.sensorName != null ? item.sensor.sensorName : item.SenSorCode;
                    sensorInfo.NewestTemp1 = decimal.Round((decimal)item.sensor.temp1, 1);
                    sensorInfo.NewestTemp2 = decimal.Round((decimal)item.sensor.temp2, 1);
                }
                results.NewDataByLotId.SenSor.Add(sensorInfo);
            }

            results.NewDataByLotId.Location = new List<S02002ViewModel.LocationInfo>();
            foreach (var item in locationInfo)
            {
                var roomInfo = new S02002ViewModel.LocationInfo();
                roomInfo.LocationName = item.LocationName;
                roomInfo.LocationTemp = decimal.Round((decimal)item.location.temp, 1);
                roomInfo.LocationHumi = decimal.Round((decimal)item.location.humi, 1);
                results.NewDataByLotId.Location.Add(roomInfo);
            }
            return results;
        }

        public S02002ViewModel GetAllSensorByLotId(string lotId)
        {
            Nullable<System.DateTime> nowDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time);
            var result = new S02002ViewModel();

            var query = from lot in context.Lot
                        join lotContainer in context.LotContainer on lot.Id equals lotContainer.LotId
                        from container in context.Container.Where(s => s.Id == lotContainer.ContainerId && s.Type == Commons.Properties.CONTAINER_TYPE_SEIGIKU)
                        from lotContainerTerminal in context.LotContainerTerminal.Where(s => s.LotContainerId == lotContainer.Id)
                        from terminal in context.Terminal.Where(s => s.Id == lotContainerTerminal.TerminalId)
                        from sensorData in context.SensorData.Where(s => s.LotContainerId == lotContainer.Id && s.TerminalId == terminal.Id && s.MeasureDate >= lotContainer.StartDate).DefaultIfEmpty()
                        where lot.Id == lotId
                        select new SenSorInfo
                        {
                            SensorCode = terminal.Code,
                            SensorName = terminal.Name,
                            NewestTemp1 = sensorData.Temperature1,
                            NewestTemp2 = sensorData.Temperature2 != null ? sensorData.Temperature2 : sensorData.Temperature1,
                            MeasuaDate = sensorData.MeasureDate,
                            StartDate = lot.StartDate,
                            EndDate = lot.EndDate != null ? lot.EndDate : nowDate,
                            LCTStartDate = lotContainerTerminal.StartDate,
                            LCTEndDate = lotContainerTerminal.EndDate != null ? lotContainerTerminal.EndDate :
                            (lot.EndDate != null ? lot.EndDate : nowDate)
                        };
            var lotDate = query.Select(s => new
            {
                s.StartDate,
                s.EndDate
            }).OrderByDescending(s => s.EndDate).FirstOrDefault();
            var data = query.GroupBy(s => s.SensorCode)
                .Select(a => new
                {
                    SensorCode = a.Key,
                    sensor = a.Select(b => new
                    {
                        sensorName = b.SensorName,
                        temp1 = b.NewestTemp1,
                        temp2 = b.NewestTemp2,
                        measuareDate = b.MeasuaDate,
                        lotStart = b.LCTStartDate,
                        lotEnd = b.LCTEndDate
                    }).Where(s => s.measuareDate >= s.lotStart && s.measuareDate <= s.lotEnd)
                    .OrderBy(c => c.measuareDate).ToList()
                }).ToList();

            var timeX = new List<Nullable<System.DateTime>>();
            if (lotDate != null)
            {
                if (lotDate.EndDate != null)
                {
                    nowDate = lotDate.EndDate;
                }
                var startD = lotDate.StartDate;
                while (startD < nowDate)
                {
                    timeX.Add(startD);
                    startD = startD.Value.AddDays(1);
                }
                timeX.Add(nowDate);
                timeX.Add(nowDate.Value.AddDays(1));
            }

            result.AllDataByLotId = new S02002ViewModel.AllInfoByLotId();
            result.AllDataByLotId.SenSor = new List<S02002ViewModel.SensorTemp>();
            result.AllDataByLotId.ListDateTimeUnixTimeStamp = new List<string>();

            foreach (var item in data)
            {
                result.AllDataByLotId.ListDateTimeUnixTimeStamp = new List<string>();
                var sensorInfo = new S02002ViewModel.SensorTemp();
                sensorInfo.Temperature1 = new List<decimal?>();
                sensorInfo.Temperature2 = new List<decimal?>();
                sensorInfo.SensorName = item.SensorCode;
                foreach (var s in item.sensor)
                {
                    sensorInfo.SensorName = s.sensorName != null ? s.sensorName : item.SensorCode;
                }
                for (var i = 0; i < timeX.Count - 1; i++)
                {
                    result.AllDataByLotId.ListDateTimeUnixTimeStamp.Add(Utils.DateTimeToLongTimeString(timeX[i]));
                    var a = item.sensor.Where(s => s.measuareDate >= timeX[i] && s.measuareDate <= timeX[i + 1])
                            .OrderByDescending(t => t.measuareDate)
                            .FirstOrDefault();
                    if (a != null)
                    {
                        sensorInfo.Temperature1.Add(decimal.Round((decimal)a.temp1, 1));
                        sensorInfo.Temperature2.Add(decimal.Round((decimal)a.temp2, 1));
                    }
                    else
                    {
                        sensorInfo.Temperature1.Add(new Nullable<decimal>());
                        sensorInfo.Temperature2.Add(new Nullable<decimal>());
                    }
                }
                result.AllDataByLotId.SenSor.Add(sensorInfo);
            }

            return result;
        }
        public S02002ViewModel GetDataByLastDay(string lotId)
        {
            Nullable<System.DateTime> nowDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time);
            Nullable<System.DateTime> lastday = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time).AddHours(-24);
            var result = new S02002ViewModel();
            var query = from lot in context.Lot
                        join lotContainer in context.LotContainer on lot.Id equals lotContainer.LotId
                        from container in context.Container.Where(s => s.Id == lotContainer.ContainerId && s.Type == Commons.Properties.CONTAINER_TYPE_SEIGIKU)
                        from lotContainerTerminal in context.LotContainerTerminal.Where(s => s.LotContainerId == lotContainer.Id)
                        from terminal in context.Terminal.Where(s => s.Id == lotContainerTerminal.TerminalId)
                        from sensorData in context.SensorData.Where(s => s.LotContainerId == lotContainer.Id && s.TerminalId == terminal.Id && s.MeasureDate >= lastday).DefaultIfEmpty()
                        where lot.Id == lotId
                        select new SenSorInfo
                        {
                            SensorCode = terminal.Code,
                            SensorName = terminal.Name,
                            NewestTemp1 = sensorData.Temperature1,
                            NewestTemp2 = sensorData.Temperature2 != null ? sensorData.Temperature2 : sensorData.Temperature1,
                            MeasuaDate = sensorData.MeasureDate,
                            StartDate = lot.StartDate,
                            EndDate = lot.EndDate != null ? lot.EndDate : nowDate,
                            LCTStartDate = lotContainerTerminal.StartDate,
                            LCTEndDate = lotContainerTerminal.EndDate != null ? lotContainerTerminal.EndDate :
                            (lot.EndDate != null ? lot.EndDate : nowDate)
                        };

            var lotDate = query.Select(s => new
            {
                s.EndDate
            }).FirstOrDefault();
            var data = query.GroupBy(s => s.SensorCode)
                .Select(a => new
                {
                    SensorCode = a.Key,
                    sensor = a.Select(b => new
                    {
                        sensorName = b.SensorName,
                        temp1 = b.NewestTemp1,
                        temp2 = b.NewestTemp2,
                        measuareDate = b.MeasuaDate,
                        lotStart = b.LCTStartDate,
                        lotEnd = b.LCTEndDate
                    }).Where(s => s.measuareDate >= s.lotStart && s.measuareDate <= s.lotEnd)
                    .OrderBy(c => c.measuareDate).ToList()
                }).ToList();
            var timeX = new List<Nullable<System.DateTime>>();
            if (lotDate != null)
            {
                if (lotDate.EndDate != null)
                {
                    nowDate = lotDate.EndDate;
                }
                while (lastday < nowDate)
                {
                    timeX.Add(lastday);
                    lastday = lastday.Value.AddHours(1);
                }
                timeX.Add(nowDate);
                timeX.Add(nowDate.Value.AddHours(1));
            }
            result.AllDataByLotId = new S02002ViewModel.AllInfoByLotId();
            result.AllDataByLotId.SenSor = new List<S02002ViewModel.SensorTemp>();
            result.AllDataByLotId.ListDateTimeUnixTimeStamp = new List<string>();

            foreach (var item in data)
            {
                result.AllDataByLotId.ListDateTimeUnixTimeStamp = new List<string>();
                var sensorInfo = new S02002ViewModel.SensorTemp();
                sensorInfo.Temperature1 = new List<decimal?>();
                sensorInfo.Temperature2 = new List<decimal?>();
                sensorInfo.SensorName = item.SensorCode;
                foreach (var s in item.sensor)
                {
                    sensorInfo.SensorName = s.sensorName != null ? s.sensorName : item.SensorCode;
                }
                for (var i = 0; i < timeX.Count - 1; i++)
                {
                    result.AllDataByLotId.ListDateTimeUnixTimeStamp.Add(Utils.DateTimeToLongTimeString(timeX[i]));
                    var a = item.sensor.Where(s => s.measuareDate >= timeX[i] && s.measuareDate <= timeX[i + 1])
                            .OrderByDescending(t => t.measuareDate)
                            .FirstOrDefault();
                    if (a != null)
                    {
                        sensorInfo.Temperature1.Add(decimal.Round((decimal)a.temp1, 1));
                        sensorInfo.Temperature2.Add(decimal.Round((decimal)a.temp2, 1));
                    }
                    else
                    {
                        sensorInfo.Temperature1.Add(new Nullable<decimal>());
                        sensorInfo.Temperature2.Add(new Nullable<decimal>());
                    }
                }
                result.AllDataByLotId.SenSor.Add(sensorInfo);
            }
            return result;
        }
        public S02002ViewModel GetDataBySearchDay(S02002ViewModel model)
        {
            var result = new S02002ViewModel();
            if (model.SearchByDate.Ticks == 0)
            {
                result = GetAllSensorByLotId(model.LotId);
            }
            else
            {
                Nullable<System.DateTime> lastTime = model.SearchByDate.AddHours(25);
                var query = from lot in context.Lot
                            join lotContainer in context.LotContainer on lot.Id equals lotContainer.LotId
                            from container in context.Container.Where(s => s.Id == lotContainer.ContainerId && s.Type == Commons.Properties.CONTAINER_TYPE_SEIGIKU)
                            from lotContainerTerminal in context.LotContainerTerminal.Where(s => s.LotContainerId == lotContainer.Id)
                            from terminal in context.Terminal.Where(s => s.Id == lotContainerTerminal.TerminalId)
                            from sensorData in context.SensorData.Where(s => s.LotContainerId == lotContainer.Id && s.TerminalId == terminal.Id && s.MeasureDate >= model.SearchByDate && s.MeasureDate <= lastTime).DefaultIfEmpty()
                            where lot.Id == model.LotId
                            select new SenSorInfo
                            {
                                SensorCode = terminal.Code,
                                SensorName = terminal.Name,
                                NewestTemp1 = sensorData.Temperature1,
                                NewestTemp2 = sensorData.Temperature2,
                                MeasuaDate = sensorData.MeasureDate,
                                StartDate = lot.StartDate,
                                EndDate = lot.EndDate != null ? lot.EndDate : lastTime,
                                LCTStartDate = lotContainerTerminal.StartDate,
                                LCTEndDate = lotContainerTerminal.EndDate != null ? lotContainerTerminal.EndDate :
                            (lot.EndDate != null ? lot.EndDate : lastTime)
                            };

                var data = query.GroupBy(s => s.SensorCode)
                    .Select(a => new
                    {
                        SensorCode = a.Key,
                        sensor = a.Select(b => new
                        {
                            sensorName = b.SensorName,
                            temp1 = b.NewestTemp1,
                            temp2 = b.NewestTemp2,
                            measuareDate = b.MeasuaDate,
                            lotStart = b.LCTStartDate,
                            lotEnd = b.LCTEndDate
                        }).Where(s => s.measuareDate >= s.lotStart && s.measuareDate <= s.lotEnd)
                    .OrderBy(c => c.measuareDate).ToList()
                    }).ToList();
                var timeX = new List<Nullable<System.DateTime>>();
                if (data != null)
                {
                    var startTime = (Nullable<System.DateTime>)model.SearchByDate;
                    while (startTime < lastTime)
                    {
                        timeX.Add(startTime);
                        startTime = startTime.Value.AddHours(1);
                    }
                }
                result.AllDataByLotId = new S02002ViewModel.AllInfoByLotId();
                result.AllDataByLotId.SenSor = new List<S02002ViewModel.SensorTemp>();
                result.AllDataByLotId.ListDateTimeUnixTimeStamp = new List<string>();

                foreach (var item in data)
                {
                    result.AllDataByLotId.ListDateTimeUnixTimeStamp = new List<string>();
                    var sensorInfo = new S02002ViewModel.SensorTemp();
                    sensorInfo.Temperature1 = new List<decimal?>();
                    sensorInfo.Temperature2 = new List<decimal?>();
                    sensorInfo.SensorName = item.SensorCode;
                    foreach (var s in item.sensor)
                    {
                        sensorInfo.SensorName = s.sensorName != null ? s.sensorName : item.SensorCode;
                    }
                    for (var i = 0; i < timeX.Count - 1; i++)
                    {
                        result.AllDataByLotId.ListDateTimeUnixTimeStamp.Add(Utils.DateTimeToLongTimeString(timeX[i]));
                        var a = item.sensor.Where(s => s.measuareDate >= timeX[i] && s.measuareDate <= timeX[i + 1])
                                .OrderByDescending(t => t.measuareDate)
                                .FirstOrDefault();
                        if (a != null)
                        {
                            sensorInfo.Temperature1.Add(decimal.Round((decimal)a.temp1, 1));
                            sensorInfo.Temperature2.Add(decimal.Round((decimal)a.temp2, 1));
                        }
                        else
                        {
                            sensorInfo.Temperature1.Add(new Nullable<decimal>());
                            sensorInfo.Temperature2.Add(new Nullable<decimal>());
                        }
                    }
                    result.AllDataByLotId.SenSor.Add(sensorInfo);
                }
            }
            return result;
        }

        public Boolean GetInfoLotByLotId(string lotId,
                                        ref string lotCode,
                                        ref string factoryId,
                                        ref Nullable<System.DateTime> startDate,
                                        ref Nullable<System.DateTime> endDate,
                                        ref string riceId,
                                        ref Nullable<int> kubunId,
                                        ref string semaibuaiId,
                                        ref Nullable<decimal> riceRatio)
        {
            IEnumerable<LotModel> query = from lot in context.Lot.AsQueryable()
                                          where lot.Id == lotId
                                          select new LotModel
                                          {
                                              LotCode = lot.Code,
                                              Factory = lot.FactotyId,
                                              Start = lot.StartDate,
                                              End = lot.EndDate,
                                              Rice = lot.MaterialId,
                                              Kubun = lot.MaterialClassification,
                                              Semaibuai = lot.RicePolishingRatioName,
                                              Ratio = lot.RicePolishingRatio
                                          };
            if (query.Any())
            {
                var result = query.ToList().First();
                lotCode = result.LotCode;
                factoryId = result.Factory;
                startDate = result.Start;
                endDate = result.End;
                riceId = result.Rice;
                kubunId = result.Kubun;
                semaibuaiId = result.Semaibuai;
                riceRatio = result.Ratio;
            }
            else
            {
                return false;
            }
            return true;
        }

        public List<S03001ViewModel.Tank> GetListTankIdByLotId(string lotId)
        {
            var lotcontainer = context.LotContainer.AsQueryable();
            var container = context.Container.AsQueryable();
            var materialstandval = context.MaterialStandVal.AsQueryable();

            lotcontainer = lotcontainer.Where(s => s.LotId.Equals(lotId));
            container = container.Where(s => s.Type.Equals(Commons.Properties.CONTAINER_TYPE_TANK));
            materialstandval = materialstandval.Where(s => s.Type == Commons.Properties.CONTAINER_TYPE_TANK);

            IEnumerable<S03001ViewModel.Tank> query = from sLotCon in lotcontainer
                                                      join sCon in container
                                                      on sLotCon.ContainerId.Trim() equals sCon.Id.Trim()
                                                      join lot in context.Lot
                                                      on sLotCon.LotId equals lot.Id
                                                      join sMatVal in materialstandval
                                                      on lot.MaterialId equals sMatVal.MaterialId
                                                      into tMatValJon
                                                      from sMatValResult in tMatValJon.DefaultIfEmpty()
                                                      select new S03001ViewModel.Tank
                                                      {
                                                          Id = sCon.Id,
                                                          Code = sCon.Code,
                                                          MinTemp = sLotCon.TempMin == null ? sMatValResult.TempMin : sLotCon.TempMin,
                                                          MaxTemp = sLotCon.TempMax == null ? sMatValResult.TempMax : sLotCon.TempMax,
                                                          StartDate = sLotCon.StartDate,
                                                          EndDate = sLotCon.EndDate,
                                                          IdOld = sCon.Id,
                                                          DeleteFlg = sCon.DeleteFlg,
                                                      };


            var result = query.ToList();
            return result;
        }

        public string FinishDateLot(string lotID)
        {
            var systemDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time);
            var query = context.Lot.Where(s => s.Id == lotID).FirstOrDefault();
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
        public string Add(Lot entity, ref string lotID)
        {
            var item = context.Lot.Where(s => s.Code == entity.Code && s.FactotyId == entity.FactotyId).FirstOrDefault();
            if (item != null)
            {
                return string.Format(localizer["msg_item_exist"], localizer["lotcode"], entity.Code);
            }
            entity.Id = Utils.GenerateId(context);
            context.Lot.Add(entity);
            context.SaveChanges();
            lotID = entity.Id;

            return string.Empty;
        }

        public string Update(Lot entity)
        {
            var systemDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time);
            var item = context.Lot.Where(s => s.Id == entity.Id).FirstOrDefault();
            if (item != null)
            {
                var duplicateItem = context.Lot.Where(s => s.Id != item.Id && s.Code == entity.Code && s.FactotyId == entity.FactotyId).FirstOrDefault();
                if (duplicateItem != null)
                {
                    return string.Format(localizer["msg_item_exist"], localizer["lotcode"], entity.Code);
                }

                item.Code = entity.Code;
                item.FactotyId = entity.FactotyId;
                item.MaterialId = entity.MaterialId;
                item.StartDate = entity.StartDate;
                item.EndDate = entity.EndDate;
                item.ModifiedOn = entity.ModifiedOn;
                item.ModifiedById = entity.ModifiedById;
                item.MaterialClassification = entity.MaterialClassification;
                item.RicePolishingRatioName = entity.RicePolishingRatioName;
                item.RicePolishingRatio = entity.RicePolishingRatio;

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
            var item = context.Lot.Where(s => s.Id == id).FirstOrDefault();
            if (item != null)
            {
                if (item.EndDate == null)
                {
                    return localizer["msg_detele_using"];
                }
                context.Lot.Remove(item);

                context.SaveChanges();
            }
            else
            {
                return localizer["C01004"];
            }

            return string.Empty;
        }
    }
}