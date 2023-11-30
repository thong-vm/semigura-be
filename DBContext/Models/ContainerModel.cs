namespace semigura.DBContext.Models
{
    public class ContainerModel
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string FactoryId { get; set; }
        public string FactoryName { get; set; }
        public string LocationId { get; set; }
        public string LocationName { get; set; }
        public string LocationCode { get; set; }
        public int Type { get; set; }
        public Nullable<decimal> Capacity { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string CreatedById { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public string ModifiedById { get; set; }
        public Nullable<decimal> Height { get; set; }
        public string LotContainerId { get; set; }
    }
}