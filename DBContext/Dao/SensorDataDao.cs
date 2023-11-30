using Microsoft.Extensions.Localization;
using semigura.Commons;
using semigura.DBContext.Entities;
using semigura.DBContext.Models;
using semigura.Models;

namespace semigura.DBContext.Repositories
{
    public class SensorDataDao
    {
        private readonly DBEntities context;
        private readonly IStringLocalizer localizer;
        public SensorDataDao(DBEntities context, IStringLocalizer localizer)
        {
            this.localizer = localizer;
            this.context = context;
        }
        public List<SensorData> GetListSensor()
        {
            return context.SensorData.ToList();
        }

        /// <summary>
        /// タンクの温度・湿度を取得
        /// </summary>
        /// <param name="lotContainerId"></param>
        /// <param name="measuareDate"></param>
        /// <returns></returns>
        public List<SensorData> GetListSensorDataByLotContainerId(string lotContainerId, DateTime? startDate, DateTime? endDate)
        {
            return context.SensorData
                .Where(s => s.LotContainerId == lotContainerId
                    && (startDate != null && s.MeasureDate >= startDate)
                    && (endDate == null || (endDate != null && s.MeasureDate <= endDate)))
                .ToList();
        }

        /// <summary>
        /// ロケーションの温度・湿度を取得
        /// </summary>
        /// <param name="lotContainerId"></param>
        /// <param name="measuareDate"></param>
        /// <returns></returns>
        public List<SensorDataModel> GetListSensorDataOfLocationByLotContainerId(string lotContainerId, DateTime? startDate, DateTime? endDate)
        {
            var query = from lotContainer in context.LotContainer
                        join sensorData in context.SensorData on lotContainer.LocationId equals sensorData.LotContainerId
                        join location in context.Location on lotContainer.LocationId equals location.Id

                        where lotContainer.Id == lotContainerId
                            && (startDate != null && sensorData.MeasureDate >= startDate)
                            && (endDate == null || (endDate != null && sensorData.MeasureDate <= endDate))

                        select new SensorDataModel
                        {
                            Id = sensorData.Id,
                            TerminalId = sensorData.TerminalId,
                            Humidity = sensorData.Humidity,
                            Temperature1 = sensorData.Temperature1,
                            Temperature2 = sensorData.Temperature2 != null ? sensorData.Temperature2 : sensorData.Temperature1,
                            Temperature3 = sensorData.Temperature3 != null ? sensorData.Temperature3 : sensorData.Temperature1,
                            MeasureDate = sensorData.MeasureDate,
                            LocationId = lotContainer.LocationId,
                            LocationName = location.Name,
                        };

            return query.ToList();
        }


        public SensorData GetFirstSensorData(string teminalId)
        {
            return context.SensorData.Where(s => s.TerminalId == teminalId).OrderByDescending(s => s.MeasureDate).FirstOrDefault();
        }

        public string Add(SensorData entity)
        {
            entity.Id = Utils.GenerateId(context);
            context.SensorData.Add(entity);
            context.SaveChanges();

            return string.Empty;
        }

        public string AddTemperateEdit(S02001ViewModel.TemperateEdit temperateChange)
        {
            var systemDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time);
            SensorData entity = new SensorData();
            entity.Id = Utils.GenerateId(context);
            entity.TerminalId = temperateChange.IdTerminal_LocationTemperature;
            entity.Temperature1 = Decimal.Parse(temperateChange.LocationTemperature);
            entity.LotContainerId = temperateChange.LotcontainerId_LocationTemperature;
            entity.MeasureDate = temperateChange.MeasuareDateUnixTimeStamp;
            entity.CreatedOn = systemDate;
            context.SensorData.Add(entity);
            context.SaveChanges();

            return string.Empty;
        }

        public string AddTemperateAvgEdit(S02001ViewModel.TemperateEdit temperateAvgChange)
        {
            var systemDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time);
            SensorData entity = new SensorData();
            entity.Id = Utils.GenerateId(context);
            entity.TerminalId = temperateAvgChange.IdTerminal_TemperatureAvg;
            entity.Temperature1 = Decimal.Parse(temperateAvgChange.ProductTemperatureAvg);
            entity.Temperature2 = Decimal.Parse(temperateAvgChange.ProductTemperatureAvg);
            entity.Temperature3 = Decimal.Parse(temperateAvgChange.ProductTemperatureAvg);
            entity.LotContainerId = temperateAvgChange.LotcontainerId_TemperatureAvg;
            entity.MeasureDate = temperateAvgChange.MeasuareDateUnixTimeStamp;
            entity.CreatedOn = systemDate;
            context.SensorData.Add(entity);
            context.SaveChanges();

            return string.Empty;
        }

        public string ModifyTemperateEdit(S02001ViewModel.TemperateEdit temperateChange)
        {
            var systemDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time);
            var query = context.SensorData.Where(s => s.Id == temperateChange.Id_LocationTemperature).FirstOrDefault();
            if (query != null)
            {
                query.Temperature1 = Decimal.Parse(temperateChange.LocationTemperature);
                context.SaveChanges();
            }
            return string.Empty;
        }

        public string ModifyTemperateAvgEdit(S02001ViewModel.TemperateEdit temperateAvgChange)
        {
            var query = context.SensorData.Where(s => s.Id == temperateAvgChange.Id_TemperatureAvg).FirstOrDefault();
            if (query != null)
            {
                query.Temperature1 = Decimal.Parse(temperateAvgChange.ProductTemperatureAvg);
                query.Temperature2 = Decimal.Parse(temperateAvgChange.ProductTemperatureAvg);
                query.Temperature3 = Decimal.Parse(temperateAvgChange.ProductTemperatureAvg);
                context.SaveChanges();
            }
            return string.Empty;
        }

        public string DeleteByLotContainerId(string lotcontainerId)
        {
            var item = context.SensorData.Where(s => s.LotContainerId == lotcontainerId);
            if (item.Any())
            {
                //item.ForEach(s => context.SensorData.Remove(s));
                context.SensorData.RemoveRange(item);
                context.SaveChanges();
            }
            else
            {
                return localizer["C01004"];
            }

            return string.Empty;
        }

        public string DeleteByLotContainerIdAndTerminalId(string lotcontainerId, string terminalId)
        {
            var item = context.SensorData.Where(s => s.LotContainerId == lotcontainerId && s.TerminalId == terminalId);
            if (item.Any())
            {
                //item.ForEach(s => context.SensorData.Remove(s));
                context.SensorData.RemoveRange(item);
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