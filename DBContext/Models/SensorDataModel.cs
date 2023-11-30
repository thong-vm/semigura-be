namespace semigura.DBContext.Models
{
    public class SensorDataModel
    {
        public string Id { get; set; }
        public string TerminalId { get; set; }
        public Nullable<decimal> Humidity { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string CreatedById { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public string ModifiedById { get; set; }
        public Nullable<decimal> Temperature2 { get; set; }
        public Nullable<decimal> Temperature3 { get; set; }
        public Nullable<decimal> Temperature1 { get; set; }
        public System.DateTime MeasureDate { get; set; }
        public string LotContainerId { get; set; }
        public string LocationId { get; set; }
        public string LocationName { get; set; }
    }
}