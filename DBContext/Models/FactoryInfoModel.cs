namespace semigura.DBContext.Models
{
    public class FactoryInfoModel
    {
        public Nullable<int> ContainerType { get; set; }
        public List<Location> LocationList { get; set; }
        public class Location
        {
            public string LocationName { get; set; }
            public List<Lot> LotList { get; set; }
        }

        public class Lot
        {
            public string LotCode { get; set; }
            public List<Tank> TankList { get; set; }
        }

        public class Tank
        {
            public string TankCode { get; set; }
            public List<Terminal> TerminalList { get; set; }
            public List<MediaModel> ImageList { get; set; }
        }
        public class Terminal
        {
            public string TerminalCode { get; set; }
            public List<SensorData> SensorDataList { get; set; }
        }
        public class SensorData
        {
            public string SensorName { get; set; }
            public Nullable<decimal> SensorTemp { get; set; }
            public Nullable<decimal> SensorTemp2 { get; set; }
            public Nullable<decimal> SensorTemp3 { get; set; }
        }

        public string ImagePath { get; set; }
        public string MediaId { get; set; }
        public Nullable<int> TerminalType { get; set; }
        public string LocationName { get; set; }
        public string LotCode { get; set; }
        public Nullable<System.DateTime> LotStartDate { get; set; }
        public Nullable<System.DateTime> LotEndDate { get; set; }
        public string ContainerCode { get; set; }
        public string TerminalId { get; set; }
        public string SensorCode { get; set; }
        public string SensorName { get; set; }
        public Nullable<decimal> SensorTemp1 { get; set; }
        public Nullable<decimal> SensorTemp2 { get; set; }
        public Nullable<decimal> SensorTemp3 { get; set; }
        public Nullable<System.DateTime> SensorTempMeasureDate { get; set; }
        public string LocationId { get; set; }
        public string LocationContainer { get; set; }
        public Nullable<System.DateTime> ImageCreateOn { get; set; }
        public Nullable<int> ImageType { get; set; }
    }
}