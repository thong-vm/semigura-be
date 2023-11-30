using semigura.Commons;
using semigura.DBContext.Entities;
using semigura.DBContext.Models;
using semigura.Models;
using static semigura.Models.S02003ViewModel;

namespace semigura.DBContext.Repositories
{
    public class LocationDao
    {
        private readonly DBEntities context;
        public LocationDao(DBEntities context)
        {
            this.context = context;
        }
        public List<semigura.DBContext.Entities.Location> GetListLocation()
        {
            return context.Location.OrderBy(s => s.Name).ToList();
        }

        public List<semigura.DBContext.Entities.Location> GetListLocation(string factoryId)
        {
            return context.Location.Where(s => s.FactotyId == factoryId).ToList();
        }
        public List<semigura.DBContext.Entities.Location> GetLocationByTankId(string tankId)
        {
            var query = from location in context.Location
                        join container in context.Container
                            on location.Id equals container.LocationId
                        where container.Id == tankId
                        select location;

            return query.ToList();
        }

        public S02003ViewModel GetDataByLocationId(S02003ViewModel model)
        {
            var result = new S02003ViewModel();
            DateTime rankTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time).AddDays(-30);
            if (model.LocationId != "all")
            {
                var query = from location in context.Location
                            from sensor in context.SensorData.Where(s => s.LotContainerId == location.Id && s.MeasureDate >= rankTime).DefaultIfEmpty()
                            where location.Id == model.LocationId
                            select new LocationModel
                            {
                                LocationId = location.Id,
                                LocationName = location.Name,
                                LocationTemp = sensor.Temperature1,
                                LocationHumi = sensor.Humidity,
                                MeasureDate = sensor.MeasureDate
                            };
                var data = query.OrderBy(s => s.MeasureDate).ToList();
                var locationInfo = new LocationInfo();
                locationInfo.LocationTemp = new List<decimal?>();
                locationInfo.LocationHumi = new List<decimal?>();
                locationInfo.ListMeasureUnixTimeStamp = new List<string>();
                foreach (var loc in data)
                {
                    locationInfo.LocationId = loc.LocationId;
                    locationInfo.LocationName = loc.LocationName;
                    if (loc.LocationTemp.HasValue || loc.LocationHumi.HasValue)
                    {
                        if (loc.LocationTemp.HasValue)
                        {
                            locationInfo.LocationTemp.Add(decimal.Round((decimal)loc.LocationTemp, 1));
                        }
                        else
                        {
                            locationInfo.LocationTemp.Add(null);
                        }
                        if (loc.LocationHumi.HasValue)
                        {
                            locationInfo.LocationHumi.Add(decimal.Round((decimal)loc.LocationHumi, 1));
                        }
                        else
                        {
                            locationInfo.LocationHumi.Add(null);
                        }

                        if (loc.MeasureDate.HasValue) locationInfo.ListMeasureUnixTimeStamp.Add(Utils.DateTimeToLongTimeString(loc.MeasureDate));
                    }
                }
                result.LocationInfoList = new List<LocationInfo>();
                result.LocationInfoList.Add(locationInfo);
            }
            else
            {
                var query = from factory in context.Factory
                            join location in context.Location on factory.Id equals location.FactotyId
                            from sensor in context.SensorData.Where(s => s.LotContainerId == location.Id && s.MeasureDate >= rankTime).DefaultIfEmpty()
                            where factory.Id == model.FactoryId
                            select new LocationModel
                            {
                                LocationId = location.Id,
                                LocationName = location.Name,
                                LocationTemp = sensor.Temperature1,
                                LocationHumi = sensor.Humidity,
                                MeasureDate = sensor.MeasureDate
                            };
                var data = query.GroupBy(s => new { s.LocationId, s.LocationName })
                 .Select(a => new
                 {
                     Key = a.Key,
                     Location = a.Select(b => new
                     {
                         LocationId = b.LocationId,
                         LocationName = b.LocationName,
                         LocationTemp = b.LocationTemp,
                         LocationHumi = b.LocationHumi,
                         MeasureDate = b.MeasureDate
                     }).OrderBy(s => s.MeasureDate).ToList()
                 }).ToList();

                result.LocationInfoList = new List<LocationInfo>();
                foreach (var item in data)
                {
                    var locationInfo = new LocationInfo();
                    locationInfo.LocationId = item.Key.LocationId;
                    locationInfo.LocationName = item.Key.LocationName;
                    locationInfo.LocationTemp = new List<decimal?>();
                    locationInfo.LocationHumi = new List<decimal?>();
                    locationInfo.ListMeasureUnixTimeStamp = new List<string>();
                    foreach (var loc in item.Location)
                    {
                        if (loc.LocationTemp.HasValue || loc.LocationHumi.HasValue)
                        {
                            if (loc.LocationTemp.HasValue)
                            {
                                locationInfo.LocationTemp.Add(decimal.Round((decimal)loc.LocationTemp, 1));
                            }
                            else
                            {
                                locationInfo.LocationTemp.Add(null);
                            }
                            if (loc.LocationHumi.HasValue)
                            {
                                locationInfo.LocationHumi.Add(decimal.Round((decimal)loc.LocationHumi, 1));
                            }
                            else
                            {
                                locationInfo.LocationHumi.Add(null);
                            }
                            if (loc.MeasureDate.HasValue) locationInfo.ListMeasureUnixTimeStamp.Add(Utils.DateTimeToLongTimeString(loc.MeasureDate));

                        }
                    }
                    result.LocationInfoList.Add(locationInfo);
                }
            }

            return result;
        }
        public S02003ViewModel GetDataByRoomName(S02003ViewModel model)
        {
            Nullable<System.DateTime> rankTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time).AddDays(-30);
            var result = new S02003ViewModel();
            if (model.LocationId != null)
            {
                var query = from location in context.Location
                            from sensor in context.SensorData.Where(s => s.LotContainerId == location.Id && s.MeasureDate >= rankTime).DefaultIfEmpty()
                            where location.Id == model.LocationId
                            select new LocationModel
                            {
                                LocationId = location.Id,
                                LocationName = location.Name,
                                LocationTemp = sensor.Temperature1,
                                LocationHumi = sensor.Humidity,
                                MeasureDate = sensor.MeasureDate
                            };
                if (model.SearchMode == "oneDay")
                {
                    DateTime lastDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time).AddHours(-24);
                    query = query.Where(s => s.MeasureDate >= lastDate);
                }

                if (model.SearchMode == "searchDay")
                {
                    if (model.SearchDate != null)
                    {
                        DateTime lastDate = model.SearchDate.Value.AddHours(24);
                        query = query.Where(s => s.MeasureDate >= model.SearchDate && s.MeasureDate <= lastDate);
                    }
                }

                var data = query.OrderBy(s => s.MeasureDate).ToList();
                var locationInfo = new LocationInfo();
                locationInfo.LocationTemp = new List<decimal?>();
                locationInfo.LocationHumi = new List<decimal?>();
                locationInfo.ListMeasureUnixTimeStamp = new List<string>();
                foreach (var loc in data)
                {
                    locationInfo.LocationId = loc.LocationId;
                    locationInfo.LocationName = loc.LocationName;
                    if (loc.LocationTemp.HasValue || loc.LocationHumi.HasValue)
                    {
                        if (loc.LocationTemp.HasValue)
                        {
                            locationInfo.LocationTemp.Add(decimal.Round((decimal)loc.LocationTemp, 1));
                        }
                        else
                        {
                            locationInfo.LocationTemp.Add(null);
                        }
                        if (loc.LocationHumi.HasValue)
                        {
                            locationInfo.LocationHumi.Add(decimal.Round((decimal)loc.LocationHumi, 1));
                        }
                        else
                        {
                            locationInfo.LocationHumi.Add(null);
                        }
                        if (loc.MeasureDate.HasValue) locationInfo.ListMeasureUnixTimeStamp.Add(Utils.DateTimeToLongTimeString(loc.MeasureDate));

                    }
                }

                if (data.Count == 0)
                {
                    locationInfo.LocationId = model.LocationId;
                }
                result.LocationInfoList = new List<LocationInfo>();
                result.LocationInfoList.Add(locationInfo);
            }
            return result;
        }

        public List<semigura.DBContext.Entities.Location> GetListLocationByFactory(string factoryId)
        {
            var query = from location in context.Location
                        where location.FactotyId == factoryId
                        select new { LocationId = location.Id, LocationCode = location.Code, LocationName = location.Name };

            var results = query.ToList();
            var locationList = new List<semigura.DBContext.Entities.Location>();
            foreach (var item in results)
            {
                semigura.DBContext.Entities.Location location = new semigura.DBContext.Entities.Location();
                location.Id = item.LocationId;
                location.Code = item.LocationCode;
                location.Name = item.LocationName;
                locationList.Add(location);
            }
            return locationList;
        }
    }
}