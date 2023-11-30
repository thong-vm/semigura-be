using semigura.Commons;
using semigura.DBContext.Entities;
using semigura.DBContext.Models;
using System.ComponentModel.DataAnnotations;

namespace semigura.Models
{
    public class S03005ViewModel : BaseViewModel
    {
        public List<NotificationModel>? DataNotificationList { get; set; }
        public List<Factory>? FactoryList { get; set; }
        public List<Container>? ContainerList { get; set; }
        public List<semigura.DBContext.Entities.Location>? LocationList { get; set; }
        public List<Lot>? LotList { get; set; }
        public int RowNo { get; set; }
        public string? Id { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public Nullable<int> Level { get; set; }
        public Nullable<int>[]? SearchStatus { get; set; }
        public Nullable<int> Status { get; set; }
        public string? PersonInCharge { get; set; }
        [StringLength(500, ErrorMessage = "lengthCheck")]
        public string? Note { get; set; }
        public Nullable<DateTime> CreatedOn { get; set; }
        public string? CreatedOnUnixTimeStamp
        {
            get
            {
                return CreatedOn.HasValue ? Utils.DateTimeToLongTimeString(CreatedOn) : null;
            }
        }
        public string? CreatedById { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string? ModifiedById { get; set; }
        public string? Name { get; set; }
        public string? ParentId { get; set; }
        public string? ParentName { get; set; }
        public string? FactoryId { get; set; }
        public string? LocationId { get; set; }
        public string? ContainerId { get; set; }
        public Nullable<int> TypeId { get; set; }
        public Nullable<int> Type { get; set; }
        public string? FactoryName { get; set; }
        public string? Location { get; set; }
        public string? Container { get; set; }
        public Nullable<int> Type_ParentId { get; set; }
        public string? LotId { get; set; }
        public string? LotCode { get; set; }
    }
}