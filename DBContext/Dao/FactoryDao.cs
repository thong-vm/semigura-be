using Microsoft.EntityFrameworkCore;
using semigura.DBContext.Entities;
using semigura.DBContext.Models;

namespace semigura.DBContext.Repositories
{
    public class FactoryDao
    {
        private readonly DBEntities context;
        public FactoryDao(DBEntities context)
        {
            this.context = context;
        }
        public List<Factory> GetListFactory()
        {
            try
            {
                return context.Factory.ToList();
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }
            return context.Factory.ToList();
        }
        public List<FactoryInfoModel> GetFactoryInfoById(string factoryId)
        {
            var query = from factory in context.Factory

                        from location in context.Location.Where(s => s.FactotyId == factory.Id).DefaultIfEmpty()

                        from lot in context.Lot.Where(s => s.FactotyId == factory.Id && s.EndDate == null).DefaultIfEmpty()

                        from lotContainer in context.LotContainer.Where(s => s.LotId == lot.Id && s.EndDate == null && s.LocationId == location.Id).DefaultIfEmpty()

                        from container in context.Container.Where(s => s.Id == lotContainer.ContainerId).DefaultIfEmpty()

                        from terminal in context.Terminal.Where(s => s.ParentId == container.Id && s.DeleteFlg != true).DefaultIfEmpty()

                        from sensorData in context.SensorData.Where(s => s.LotContainerId == lotContainer.Id && s.TerminalId == terminal.Id).OrderByDescending(s => s.MeasureDate).Take(1).DefaultIfEmpty()

                        from media in context.Media.Where(s => s.LotContainerId == lotContainer.Id).OrderByDescending(s => s.CreatedOn).Take(5).DefaultIfEmpty()

                        where factory.Id == factoryId
                        select new FactoryInfoModel
                        {
                            LocationName = location.Name,
                            LocationId = location.Id,
                            LotCode = lot.Code,
                            LotStartDate = lot.StartDate,
                            LotEndDate = lot.EndDate,
                            LocationContainer = container.LocationId,
                            ContainerCode = container.Code,
                            ContainerType = container.Type,
                            TerminalType = terminal.Type,
                            TerminalId = terminal.Id,
                            SensorCode = terminal.Code,
                            SensorName = terminal.Name,
                            SensorTemp1 = sensorData.Temperature1,
                            SensorTemp2 = sensorData.Temperature2 != null ? sensorData.Temperature2 : sensorData.Temperature1,
                            SensorTemp3 = sensorData.Temperature3 != null ? sensorData.Temperature3 : sensorData.Temperature1,
                            SensorTempMeasureDate = sensorData.MeasureDate,
                            MediaId = media.Id,
                            ImagePath = media.Path,
                            ImageCreateOn = media.CreatedOn,
                            ImageType = media.Type
                        };

            var data = query.OrderBy(s => s.LotStartDate).ToList();

            var results = data.GroupBy(s => s.ContainerType)
                .Select(a => new
                {
                    ContainerType = a.Key,
                    Location = a.GroupBy(b => b.LocationName)
                   .Select(c => new
                   {
                       LocationName = c.Key,
                       Lot = c.GroupBy(d => d.LotCode)
                       .Select(e => new
                       {
                           LotCode = e.Key,
                           Tank = e.GroupBy(f => f.ContainerCode)
                           .Select(g => new
                           {
                               TankCode = g.Key,
                               Image = g.GroupBy(mId => mId.MediaId).Select(m => new
                               {
                                   Id = m.Key,
                                   MediaModel = m.Select(n => new MediaModel
                                   {
                                       Id = n.MediaId,
                                       Path = n.ImagePath,
                                       CreatedOn = n.ImageCreateOn,
                                       Type = n.ImageType
                                   }).Where(type => type.Type == semigura.Commons.Properties.MEDIA_TYPE_IMAGE).FirstOrDefault()
                               }).Take(5).ToList(),
                               Terminal = g.GroupBy(h => h.TerminalId)
                               .Select(i => new
                               {
                                   TerminalCode = i.Key,
                                   Sensor = i.Select(j => new { j.SensorCode, j.SensorName, j.SensorTemp1, j.SensorTemp2, j.SensorTemp3, j.TerminalType, j.SensorTempMeasureDate })
                                   .OrderByDescending(k => k.SensorTempMeasureDate)
                                   .FirstOrDefault()
                               }).ToList()
                           }).ToList()
                       }).ToList()
                   }).ToList()
                }).ToList();

            var factoryInfoList = new List<FactoryInfoModel>();
            foreach (var items in results)
            {
                if (items.ContainerType != null)
                {
                    var factoryInfo = new FactoryInfoModel();
                    factoryInfo.ContainerType = items.ContainerType;
                    factoryInfo.LocationList = new List<FactoryInfoModel.Location>();
                    foreach (var a in items.Location)
                    {
                        var location = new FactoryInfoModel.Location();
                        location.LocationName = a.LocationName;
                        location.LotList = new List<FactoryInfoModel.Lot>();
                        foreach (var b in a.Lot)
                        {
                            var lot = new FactoryInfoModel.Lot();
                            var tankList = new List<FactoryInfoModel.Tank>();
                            lot.LotCode = b.LotCode;
                            foreach (var c in b.Tank)
                            {
                                var tank = new FactoryInfoModel.Tank();
                                var terminalList = new List<FactoryInfoModel.Terminal>();
                                tank.TankCode = c.TankCode;
                                tank.ImageList = new List<MediaModel>();
                                foreach (var m in c.Image)
                                {
                                    tank.ImageList.Add(m.MediaModel);
                                }
                                foreach (var d in c.Terminal)
                                {
                                    var terminal = new FactoryInfoModel.Terminal();
                                    terminal.SensorDataList = new List<FactoryInfoModel.SensorData>();
                                    var sensor = new FactoryInfoModel.SensorData();
                                    if (d.Sensor != null && d.Sensor.TerminalType != semigura.Commons.Properties.TERMINAL_TYPE_LOCATION && d.Sensor.TerminalType != semigura.Commons.Properties.TERMINAL_TYPE_CAMERA)
                                    {
                                        if (d.Sensor.SensorName != null)
                                        {
                                            sensor.SensorName = d.Sensor.SensorName;
                                        }
                                        else if (d.Sensor.SensorCode != null)
                                        {
                                            sensor.SensorName = d.Sensor.SensorCode;
                                        }

                                        if (d.Sensor.SensorTemp1 != null)
                                        {
                                            sensor.SensorTemp = decimal.Round((decimal)d.Sensor.SensorTemp1, 1);
                                        }

                                        if (d.Sensor.SensorTemp2 != null)
                                        {
                                            sensor.SensorTemp2 = decimal.Round((decimal)d.Sensor.SensorTemp2, 1);
                                        }

                                        if (d.Sensor.SensorTemp3 != null)
                                        {
                                            sensor.SensorTemp3 = decimal.Round((decimal)d.Sensor.SensorTemp3, 1);
                                        }
                                        terminal.SensorDataList.Add(sensor);
                                        terminalList.Add(terminal);
                                    }
                                    tank.TerminalList = terminalList;
                                }
                                tankList.Add(tank);
                                lot.TankList = tankList;
                            }
                            location.LotList.Add(lot);
                        }
                        factoryInfo.LocationList.Add(location);
                    }
                    factoryInfoList.Add(factoryInfo);
                }
            }

            return factoryInfoList;
        }
    }
}