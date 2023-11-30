using Microsoft.Extensions.Localization;

namespace semigura.DBContext.Models
{
    public class TerminalModel
    {
        public IStringLocalizer localizer;

        public TerminalModel() { }

        public string? Id { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? LoginId { get; set; }
        public string? Password { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string? CreatedById { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public string? ModifiedById { get; set; }
        public string? ParentId { get; set; }
        public int Type { get; set; }
        public string? ParentName { get; set; }
        public string? FactoryId { get; set; }
        public string? FactoryName { get; set; }
        public string TypeLabel
        {
            get
            {
                if (localizer == null) { return "localizer is null"; }
                var result = string.Empty;
                if (this.Type != 0)
                {
                    int type = (int)this.Type;
                    if (type == semigura.Commons.Properties.TERMINAL_TYPE_TANK)
                    {
                        result = localizer["type_tank"];
                    }
                    else if (type == semigura.Commons.Properties.TERMINAL_TYPE_SEIGIKU)
                    {
                        result = localizer["type_seigiku"];
                    }
                    else if (type == semigura.Commons.Properties.TERMINAL_TYPE_LOCATION)
                    {
                        result = localizer["type_location"];
                    }
                    else if (type == semigura.Commons.Properties.TERMINAL_TYPE_CAMERA)
                    {
                        result = localizer["type_camera"];
                    }
                }

                return result;
            }
        }
        // LotContainerID／LocationID
        public string? LotContainerId { get; set; }
        public Nullable<System.DateTime> LotContainerStartDate { get; set; }
        public Nullable<System.DateTime> LotContainerEndDate { get; set; }
    }
}