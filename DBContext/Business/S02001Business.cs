using Microsoft.Extensions.Localization;
using semigura.Commons;
using semigura.DBContext.Entities;
using semigura.DBContext.Models;
using semigura.DBContext.Repositories;
using semigura.Models;

namespace semigura.DBContext.Business
{
    public class S02001Business : BaseBusiness
    {
        private readonly IStringLocalizer<S02001Business> localizer;

        public S02001Business(DBEntities db,
            ILogger<S02001Business> logger,
            IStringLocalizer<S02001Business> localizer) : base(db, logger, localizer)
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
        public List<Lot> GetListLotByFactory(string? factoryId, bool isInUse)
        {
            var result = new List<Lot>();

            if (!string.IsNullOrEmpty(factoryId))
            {
                try
                {
                    var lotDao = new LotDao(context, localizer);
                    result = lotDao.GetListLotByFactoryAndStatus(factoryId, isInUse);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message);
                    throw;
                }
            }

            return result;
        }

        public List<ContainerModel> GetListTankByLotId(string? lotId)
        {
            var result = new List<ContainerModel>();

            if (!string.IsNullOrEmpty(lotId))
            {
                try
                {
                    var tankDao = new ContainerDao(context, localizer);

                    result = tankDao.GetListContainerByLotId(lotId);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message);
                    throw;
                }
            }

            return result;
        }

        public S02001ViewModel GetAllDataByLotContainer(S02001ViewModel model)
        {
            var result = new S02001ViewModel();
            result.LastestTempData = new S02001ViewModel.LastestTemperatureDataModel();
            result.ListTableColumnData = new List<S02001ViewModel.TableColumnDataModel>();
            try
            {
                var lotContainerDao = new LotContainerDao(context, localizer);
                var sensorDataDao = new SensorDataDao(context, localizer);
                var dataEntryDao = new DataEntryDao(context, localizer);
                var mediaDao = new MediaDao(context, localizer);

                var lotContainer = lotContainerDao.GetLotContainerWithDeviceId(model.LotContainerId);
                if (lotContainer != null && lotContainer.StartDate != null)
                {
                    result.LotContainerStartDate = lotContainer.StartDate;
                    result.LotContainerEndDate = lotContainer.EndDate;

                    result.LastestTempData.LocationId = lotContainer.LocationId;
                    result.LastestTempData.LocationName = lotContainer.LocationName;
                    result.LastestTempData.LotEndDate = lotContainer.EndDate;

                    var listDataEntry = dataEntryDao.GetListDataEntryByLotContainerId(lotContainer.Id, lotContainer.StartDate, lotContainer.EndDate);
                    var listTankSensorData = sensorDataDao.GetListSensorDataByLotContainerId(lotContainer.Id, lotContainer.StartDate, lotContainer.EndDate);
                    var listLocationSensorData = sensorDataDao.GetListSensorDataOfLocationByLotContainerId(lotContainer.Id, lotContainer.StartDate, lotContainer.EndDate);

                    // I．最新温度分を取得
                    var dataEntry = listDataEntry.OrderByDescending(s => s.MeasureDate).FirstOrDefault();
                    var dataEntryMeasureDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time);
                    if (dataEntry != null)
                    {
                        result.LastestTempData.DataEntryMeasuareDate = dataEntry.MeasureDate;
                        result.LastestTempData.BaumeDegree = dataEntry.BaumeDegree.HasValue ? (Decimal?)Math.Abs((decimal)dataEntry.BaumeDegree) : null;
                        result.LastestTempData.BaumeDegree = dataEntry.BaumeDegree.HasValue
                            ? (dataEntry.BaumeDegree < 0 ? (Decimal?)Math.Abs((decimal)dataEntry.BaumeDegree / 10) : dataEntry.BaumeDegree)
                            : null;
                        result.LastestTempData.AlcoholDegree = dataEntry.AlcoholDegree;
                        result.LastestTempData.Acid = dataEntry.Acid;
                        result.LastestTempData.AminoAcid = dataEntry.AminoAcid;
                        result.LastestTempData.Glucose = dataEntry.Glucose;

                        dataEntryMeasureDate = dataEntry.MeasureDate;
                    }

                    // ロケーションの温度・湿度
                    var closestObj = listLocationSensorData
                        .Where(s => s.MeasureDate.Year == dataEntryMeasureDate.Year && s.MeasureDate.Month == dataEntryMeasureDate.Month && s.MeasureDate.Day == dataEntryMeasureDate.Day)
                        .GroupBy(s => new { s.LotContainerId })
                        .Select(s => new { s.Key, MinTick = s.Min(r => Math.Abs(r.MeasureDate.Ticks - dataEntryMeasureDate.Ticks)) })
                        .FirstOrDefault();

                    if (closestObj != null)
                    {
                        var sensorDataOfLocation = listLocationSensorData.Where(s => Math.Abs(s.MeasureDate.Ticks - dataEntryMeasureDate.Ticks) == closestObj.MinTick).FirstOrDefault();
                        if (sensorDataOfLocation != null)
                        {
                            result.LastestTempData.LocationTemperature = sensorDataOfLocation.Temperature1.HasValue ? (decimal?)decimal.Round(sensorDataOfLocation.Temperature1.Value, 1) : null;
                            result.LastestTempData.LocationHumidity = sensorDataOfLocation.Humidity.HasValue ? (decimal?)decimal.Round(sensorDataOfLocation.Humidity.Value, 1) : null;
                            result.LastestTempData.LocationMeasuareDate = sensorDataOfLocation.MeasureDate;
                        }
                    }


                    // タンクの温度
                    closestObj = listTankSensorData
                        .Where(s => s.MeasureDate.Year == dataEntryMeasureDate.Year && s.MeasureDate.Month == dataEntryMeasureDate.Month && s.MeasureDate.Day == dataEntryMeasureDate.Day)
                        .GroupBy(s => new { s.LotContainerId })
                        .Select(s => new { s.Key, MinTick = s.Min(r => Math.Abs(r.MeasureDate.Ticks - dataEntryMeasureDate.Ticks)) })
                        .FirstOrDefault();

                    if (closestObj != null)
                    {
                        var sensorDataOfTank = listTankSensorData.Where(s => Math.Abs(s.MeasureDate.Ticks - dataEntryMeasureDate.Ticks) == closestObj.MinTick).FirstOrDefault();
                        if (sensorDataOfTank != null)
                        {
                            var arr = new List<decimal?> {
                                sensorDataOfTank.Temperature1,
                                sensorDataOfTank.Temperature2 != null ? sensorDataOfTank.Temperature2 : sensorDataOfTank.Temperature1,
                                sensorDataOfTank.Temperature3!=null?sensorDataOfTank.Temperature3:sensorDataOfTank.Temperature1
                            };
                            var arr2 = arr.ConvertAll(s => s.HasValue ? (decimal?)decimal.Round(s.Value, 1) : null);
                            result.LastestTempData.ProductTemperature1 = arr2[0];
                            result.LastestTempData.ProductTemperature2 = arr2[1];
                            result.LastestTempData.ProductTemperature3 = arr2[2];
                            var tempAvg = arr.Average();
                            result.LastestTempData.ProductTemperatureAvg = tempAvg.HasValue ? (decimal?)decimal.Round(tempAvg.Value, 1) : null;
                            result.LastestTempData.ProductMeasuareDate = sensorDataOfTank.MeasureDate;
                        }
                    }

                    // Ⅱ．テーブルのデータを取得
                    var startDate = (DateTime)lotContainer.StartDate;
                    var endDate = lotContainer.EndDate;
                    if (endDate == null)
                    {
                        endDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time);
                    }
                    while (endDate.Value.Hour < 23)
                    {
                        endDate = endDate.Value.AddHours(1);
                    }

                    var dayCount = 1;
                    var dateTemp = startDate;
                    while (dateTemp <= endDate)
                    {
                        // 手入力のデータ
                        var lastestDataEntryOfDay = listDataEntry.Where(s => s.MeasureDate.Year == dateTemp.Year && s.MeasureDate.Month == dateTemp.Month && s.MeasureDate.Day == dateTemp.Day).OrderByDescending(s => s.MeasureDate).FirstOrDefault();

                        // ロケーションの温度データ
                        SensorDataModel closestLocationSensorData = null;
                        if (lastestDataEntryOfDay != null)
                        {
                            var obj = listLocationSensorData
                                .Where(s => s.MeasureDate.Year == lastestDataEntryOfDay.MeasureDate.Year && s.MeasureDate.Month == lastestDataEntryOfDay.MeasureDate.Month && s.MeasureDate.Day == lastestDataEntryOfDay.MeasureDate.Day)
                                .GroupBy(s => new { s.LotContainerId })
                                .Select(s => new { s.Key, MinTick = s.Min(r => Math.Abs(r.MeasureDate.Ticks - lastestDataEntryOfDay.MeasureDate.Ticks)) })
                                .FirstOrDefault();

                            closestLocationSensorData = obj != null ? listLocationSensorData.Where(s => Math.Abs(s.MeasureDate.Ticks - lastestDataEntryOfDay.MeasureDate.Ticks) == obj.MinTick).FirstOrDefault() : null;
                        }
                        else
                        {
                            closestLocationSensorData = listLocationSensorData.Where(s => s.MeasureDate.Year == dateTemp.Year && s.MeasureDate.Month == dateTemp.Month && s.MeasureDate.Day == dateTemp.Day).OrderByDescending(s => s.MeasureDate).FirstOrDefault();
                        }

                        // タンクの温度データ
                        SensorData closestTankSensorData = null;
                        if (lastestDataEntryOfDay != null)
                        {
                            var obj = listTankSensorData
                                .Where(s => s.MeasureDate.Year == lastestDataEntryOfDay.MeasureDate.Year && s.MeasureDate.Month == lastestDataEntryOfDay.MeasureDate.Month && s.MeasureDate.Day == lastestDataEntryOfDay.MeasureDate.Day)
                                .GroupBy(s => new { s.LotContainerId })
                                .Select(s => new { s.Key, MinTick = s.Min(r => Math.Abs(r.MeasureDate.Ticks - lastestDataEntryOfDay.MeasureDate.Ticks)) })
                                .FirstOrDefault();

                            closestTankSensorData = obj != null ? listTankSensorData.Where(s => Math.Abs(s.MeasureDate.Ticks - lastestDataEntryOfDay.MeasureDate.Ticks) == obj.MinTick).FirstOrDefault() : null;
                        }
                        else
                        {
                            closestTankSensorData = listTankSensorData.Where(s => s.MeasureDate.Year == dateTemp.Year && s.MeasureDate.Month == dateTemp.Month && s.MeasureDate.Day == dateTemp.Day).OrderByDescending(s => s.MeasureDate).FirstOrDefault();
                        }

                        decimal? tempAvg = closestTankSensorData != null ? new decimal?[] { closestTankSensorData.Temperature1, closestTankSensorData.Temperature2, closestTankSensorData.Temperature3 }.Average() : null;

                        result.ListTableColumnData.Add(new S02001ViewModel.TableColumnDataModel
                        {
                            Id_LocationTemperature = (closestLocationSensorData != null) ? closestLocationSensorData.Id : null,
                            IdTerminal_LocationTemperature = (closestLocationSensorData != null) ? closestLocationSensorData.TerminalId : null,
                            LotcontainerId_LocationTemperature = (closestLocationSensorData != null) ? closestLocationSensorData.LocationId : null,
                            Id_TemperatureAvg = (closestTankSensorData != null) ? closestTankSensorData.Id : null,
                            IdTerminal_TemperatureAvg = (closestTankSensorData != null) ? closestTankSensorData.TerminalId : null,
                            LotcontainerId_TemperatureAvg = (closestTankSensorData != null) ? closestTankSensorData.LotContainerId : null,
                            DayNum = dayCount,
                            MeasuareDate = dateTemp,
                            LocationTemperature = (closestLocationSensorData != null) ? closestLocationSensorData.Temperature1 : null,
                            ProductTemperature1 = (closestTankSensorData != null) ? closestTankSensorData.Temperature1 : null,
                            ProductTemperature2 = (closestTankSensorData != null) ? closestTankSensorData.Temperature2 : null,
                            ProductTemperature3 = (closestTankSensorData != null) ? closestTankSensorData.Temperature3 : null,
                            ProductTemperatureAvg = tempAvg.HasValue ? (decimal?)decimal.Round(tempAvg.Value, 1) : null,
                            Id_Container = (lastestDataEntryOfDay != null) ? lastestDataEntryOfDay.ContainerId : null,
                            Id_DataEntry = (lastestDataEntryOfDay != null) ? lastestDataEntryOfDay.Id : null,
                            BaumeDegree = (lastestDataEntryOfDay != null && lastestDataEntryOfDay.BaumeDegree.HasValue && lastestDataEntryOfDay.BaumeDegree >= 0) ? lastestDataEntryOfDay.BaumeDegree : null,
                            BaumeSakeDegree = (lastestDataEntryOfDay != null && lastestDataEntryOfDay.BaumeDegree.HasValue && lastestDataEntryOfDay.BaumeDegree < 0) ? (Decimal?)Math.Abs((decimal)lastestDataEntryOfDay.BaumeDegree / 10) : null,
                            AlcoholDegree = (lastestDataEntryOfDay != null && lastestDataEntryOfDay.AlcoholDegree.HasValue) ? lastestDataEntryOfDay.AlcoholDegree : null,
                            Acid = (lastestDataEntryOfDay != null && lastestDataEntryOfDay.Acid.HasValue) ? lastestDataEntryOfDay.Acid : null,
                            AminoAcid = (lastestDataEntryOfDay != null && lastestDataEntryOfDay.AminoAcid.HasValue) ? lastestDataEntryOfDay.AminoAcid : null,
                            Glucose = (lastestDataEntryOfDay != null && lastestDataEntryOfDay.Glucose.HasValue) ? lastestDataEntryOfDay.Glucose : null,
                        });

                        dateTemp = dateTemp.AddDays(1);
                        dayCount++;
                    }

                    // Ⅲ．チャートのデータを取得
                    result.ChartData = new S02001ViewModel.ChartDataModel();
                    result.ChartData.ListMeasuareUnixTimestamp = result.ListTableColumnData.Select(s => Utils.DateTimeToLongTimeString(s.MeasuareDate)).ToList();
                    result.ChartData.ListLocationTemperature = result.ListTableColumnData.Select(s => s.LocationTemperature).ToList();
                    result.ChartData.ListProductTemperatureAvg = result.ListTableColumnData.Select(s => s.ProductTemperatureAvg).ToList();
                    result.ChartData.ListBaumeDegree = result.ListTableColumnData.Select(s => s.BaumeDegree != null ? s.BaumeDegree : s.BaumeSakeDegree).ToList();
                    result.ChartData.ListAlcoholDegree = result.ListTableColumnData.Select(s => s.AlcoholDegree).ToList();

                    // Ⅳ．イメージを取得
                    result.ListMedia = mediaDao.GetListMedia(lotContainer.Id, semigura.Commons.Properties.MEDIA_TYPE_IMAGE, 6);
                    result.CameraDeviceCode = lotContainer.TerminalCode;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }

            return result;
        }

        public S02001ViewModel GetListDataEntryByLotContainer(S02001ViewModel model)
        {
            try
            {
                var dataEntryDao = new DataEntryDao(context, localizer);

                return dataEntryDao.GetListDataEntry(model);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
        }

        public string Delete(S02001ViewModel model)
        {
            string message = string.Empty;
            try
            {
                var dao = new DataEntryDao(context, localizer);
                if (string.IsNullOrEmpty(model.DataEntryId))
                {
                    throw new Exception("model.DataEntryId is empty");
                }
                message = dao.Delete(model.DataEntryId);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
            return message;
        }

        public string Add(S02001ViewModel model, UserInfoModel UserInfoModel)
        {
            string message = string.Empty;
            try
            {
                var lotContainerDao = new LotContainerDao(context, localizer);
                var dataEntryDao = new DataEntryDao(context, localizer);

                if (model.DataEntry == null)
                {
                    throw new Exception("model.DataEntry is null");
                }
                var lotContainer = lotContainerDao.GetLotContainer(model.DataEntry.LotContainerId);

                DataEntry dataEntry = model.DataEntry.Map<DataEntry>();
                //dataEntry.MeasureDate = Utils.ConvertToSystemDatetime(dataEntry.MeasureDate, model.ClientTimezoneOffset);
                dataEntry.CreatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time);
                dataEntry.CreatedById = UserInfoModel.Id;
                dataEntry.ContainerId = lotContainer?.ContainerId;

                message = dataEntryDao.Add(dataEntry);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
            return message;
        }

        public string SaveTemperateEdit(List<S02001ViewModel.TemperateEdit> temperateChange, List<S02001ViewModel.TemperateEdit> temperateAvgChange)
        {
            try
            {

                temperateChange?.ForEach(s =>
                    {
                        string message = string.Empty;
                        {
                            var dao = new SensorDataDao(context, localizer);
                            //Add
                            if (s.Id_LocationTemperature == null)
                            {
                                dao.AddTemperateEdit(s);
                            }
                            //Edit
                            else
                            {
                                dao.ModifyTemperateEdit(s);
                            }

                        }

                    });

                temperateAvgChange?.ForEach(s =>
                    {
                        string message = string.Empty;
                        {
                            var dao = new SensorDataDao(context, localizer);
                            //Add
                            if (s.Id_TemperatureAvg == null)
                            {
                                dao.AddTemperateAvgEdit(s);
                            }
                            //Edit
                            else
                            {
                                dao.ModifyTemperateAvgEdit(s);
                            }

                        }

                    });
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
            return "";
        }

        public string SaveDataEntryEdit(List<DataEntry> dataEntryChange)
        {
            try
            {

                dataEntryChange?.ForEach(s =>
                    {
                        string message = string.Empty;
                        {
                            var dao = new DataEntryDao(context, localizer);
                            //Add
                            if (s.Id == null)
                            {
                                dao.AddDataEntryEdit(s);
                            }
                            //Edit
                            else
                            {
                                dao.ModifyDataEntryEdit(s);
                            }

                        }

                    });
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
            return "";
        }

    }
}