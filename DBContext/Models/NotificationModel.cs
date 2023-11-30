using Microsoft.Extensions.Localization;
using semigura.Commons;

namespace semigura.DBContext.Models
{
    public class NotificationModel
    {
        private readonly IStringLocalizer localizer;

        public NotificationModel(IStringLocalizer localizer)
        {
            this.localizer = localizer;
        }
        public string Id { get; set; }
        public string LotId { get; set; }
        public string LotCode { get; set; }
        public string Location { get; set; }
        public string LocationId { get; set; }
        public string Container { get; set; }
        public string Type
        {
            get
            {
                var result = string.Empty;
                if (TypeId == 1)
                {
                    result = localizer["type_tank"];
                }
                else if (TypeId == 2)
                {
                    result = localizer["type_seigiku"];
                }
                return result;
            }
        }
        public Nullable<int> TypeId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Level
        {
            get
            {
                var result = string.Empty;
                if (LevelId == 1)
                {
                    result = localizer["level_warning"];
                }
                else if (LevelId == 2)
                {
                    result = localizer["level_emergency"];
                }
                return result;
            }
        }
        public Nullable<int> LevelId { get; set; }
        public string Status
        {
            get
            {
                var result = string.Empty;
                if (StatusVal == 1)
                {
                    result = localizer["status_open"];
                }
                else if (StatusVal == 2)
                {
                    result = localizer["status_processing"];
                }
                else if (StatusVal == 3)
                {
                    result = localizer["status_closed"];
                }
                return result;
            }
        }
        public Nullable<int> StatusVal { get; set; }
        public string PersonInCharge { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string CreatedOnUnixTimeStamp
        {
            get
            {
                return CreatedOn.HasValue ? Utils.DateTimeToLongTimeString(CreatedOn) : null;
            }
        }
        public string Note { get; set; }
        public string Factory { get; set; }
        public string FactoryId { get; set; }
        public string ContainerId { get; set; }
        public string ParentId { get; set; }
        public string ParentName { get; set; }
        public Nullable<int> Type_ParentId { get; set; }
        public Nullable<int> ContainerType { get; set; }
    }
}