using semigura.DBContext.Entities;

namespace semigura.Models
{
    public class S02003ViewModel : BaseViewModel
    {
        public string? FactoryId { get; set; }
        public string? LocationId { get; set; }
        public string? LocationName { get; set; }
        public Nullable<decimal> LocationTemp { get; set; }
        public Nullable<decimal> LocationHumi { get; set; }
        public Nullable<System.DateTime> MeasureDate { get; set; }
        public string? SearchMode { get; set; }
        public Nullable<System.DateTime> SearchDate { get; set; }
        public List<Factory>? FactoryList { get; set; }
        public List<semigura.DBContext.Entities.Location>? LocationList { get; set; }

        public class LocationInfo
        {
            public string? LocationId { get; set; }
            public string? LocationName { get; set; }
            public List<Nullable<decimal>>? LocationTemp { get; set; }
            public List<Nullable<decimal>>? LocationHumi { get; set; }
            public List<string>? ListMeasureUnixTimeStamp { get; set; }
        }
        public List<LocationInfo>? LocationInfoList { get; set; }
    }
}