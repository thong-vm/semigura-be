using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;
using semigura.DBContext.Entities;
using semigura.DBContext.Models;
using System.ComponentModel.DataAnnotations;

namespace semigura.Models
{
    public class S09004ViewModel : BaseViewModel
    {
        public IStringLocalizer? localizer = null;

        public S09004ViewModel() { }

        public List<TerminalModel>? TerminalList { get; set; }
        public List<Factory>? FactoryList { get; set; }
        public List<Container>? ContainerList { get; set; }
        public List<semigura.DBContext.Entities.Location>? LocationList { get; set; }
        public int RowNo { get; set; }
        public string? Id { get; set; }
        [Display(Name = "terminal_code")]
        [Required(ErrorMessage = "C01001")]
        [StringLength(20, ErrorMessage = "lengthCheck")]
        public string Code { get; set; } = null!;
        [StringLength(50, ErrorMessage = "lengthCheck")]
        public string? Name { get; set; }
        public string? LoginId { get; set; }
        public string? Password { get; set; }
        public string? ParentId { get; set; }
        [Display(Name = "type")]
        [Required(ErrorMessage = "C01001")]
        public Nullable<int> Type { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string? CreatedById { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public string? ModifiedById { get; set; }
        public bool IsNotUsed { get; set; }

        [ValidateNever]
        public string TypeLabel
        {
            get
            {
                if (localizer == null) return "localizer is null";

                var result = string.Empty;
                if (this.Type != null)
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

        public string? ParentName { get; set; }
        public string? FactoryId { get; set; }
        public string? LocationId { get; set; }
        public string? ContainerId { get; set; }

        [ValidateNever]
        public List<S09004ViewModel> TypeList
        {
            get
            {
                var result = new List<S09004ViewModel>();

                var item = new S09004ViewModel();
                item.localizer = localizer;
                item.Type = semigura.Commons.Properties.TERMINAL_TYPE_TANK;
                result.Add(item);

                item = new S09004ViewModel();
                item.localizer = localizer;
                item.Type = semigura.Commons.Properties.TERMINAL_TYPE_SEIGIKU;
                result.Add(item);

                item = new S09004ViewModel();
                item.localizer = localizer;
                item.Type = semigura.Commons.Properties.TERMINAL_TYPE_LOCATION;
                result.Add(item);

                //item = new S09004ViewModel();
                //item.Type = Properties.TERMINAL_TYPE_CAMERA;
                //result.Add(item);

                return result;
            }
        }
    }
}