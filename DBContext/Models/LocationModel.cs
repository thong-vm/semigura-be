namespace semigura.DBContext.Models
{
    public class LocationModel
    {
        public string FactoryId { get; set; }
        public string LocationId { get; set; }
        public string LocationName { get; set; }
        public Nullable<decimal> LocationTemp { get; set; }
        public Nullable<decimal> LocationHumi { get; set; }
        public Nullable<System.DateTime> MeasureDate { get; set; }
    }
}