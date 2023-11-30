using semigura.DBContext.Entities;

namespace semigura.Models
{
    public class S02002ViewModel : BaseViewModel
    {
        public string? FactoryId { get; set; }
        public bool IsInUse { get; set; }
        public string? LotId { get; set; }
        public DateTime SearchByDate { get; set; }
        public List<Factory>? FactoryList { get; set; }
        public List<Lot>? LotList { get; set; }
        public NewInfoByLotId? NewDataByLotId { get; set; }
        public class NewInfoByLotId
        {
            public List<LocationInfo> Location { get; set; }
            public List<SenSorInfo> SenSor { get; set; }

        }
        public class LocationInfo
        {
            public string LocationName { get; set; }
            public Nullable<decimal> LocationTemp { get; set; }
            public Nullable<decimal> LocationHumi { get; set; }
        }
        public class SenSorInfo
        {
            public string SensorName { get; set; }
            public string SensorCode { get; set; }
            public Nullable<decimal> NewestTemp1 { get; set; }
            public Nullable<decimal> NewestTemp2 { get; set; }
            public Nullable<System.DateTime> MeasuaDate { get; set; }
            public Nullable<System.DateTime> StartDate { get; set; }
            public Nullable<System.DateTime> EndDate { get; set; }
            // LotContainerTerminal StartDate
            public Nullable<System.DateTime> LCTStartDate { get; set; }
            // LotContainerTerminal EndDate
            public Nullable<System.DateTime> LCTEndDate { get; set; }
        }

        public AllInfoByLotId? AllDataByLotId { get; set; }
        public class AllInfoByLotId
        {
            public List<SensorTemp> SenSor { get; set; }
            public List<string> ListDateTimeUnixTimeStamp { get; set; }
        }
        public class SensorTemp
        {
            public string SensorName { get; set; }
            public string SensorCode { get; set; }
            public List<Nullable<decimal>> Temperature1 { get; set; }
            public List<Nullable<decimal>> Temperature2 { get; set; }
        }

    }
}