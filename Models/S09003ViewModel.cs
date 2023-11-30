using semigura.DBContext.Entities;
using semigura.DBContext.Models;
using System.ComponentModel.DataAnnotations;

namespace semigura.Models
{
    public class S09003ViewModel : BaseViewModel
    {
        public List<ContainerModel>? ListContainer { get; set; }
        public List<Factory>? ListFactory { get; set; }
        public List<semigura.DBContext.Entities.Location>? ListLocation { get; set; }
        public int RowNo { get; set; }
        public string? Id { get; set; }
        [Display(Name = "factory")]
        [Required(ErrorMessage = "C01005")]
        public string? FactoryId { get; set; }
        [Display(Name = "location")]
        [Required(ErrorMessage = "C01005")]
        public string? LocationId { get; set; }
        public string? LocationName { get; set; }
        public string? LocationCode { get; set; }
        [Display(Name = "tankcode")]
        [Required(ErrorMessage = "C01001")]
        [StringLength(20, ErrorMessage = "lengthCheck")]
        public string? Code { get; set; }
        [Display(Name = "capacity")]
        [Required(ErrorMessage = "C01001")]
        [Range(typeof(decimal), "0", "5000", ErrorMessage = "rankCheck")]
        public Nullable<decimal> Capacity { get; set; }
        public Nullable<decimal> CapSearch_Start { get; set; }
        public Nullable<decimal> CapSearch_End { get; set; }
        [Display(Name = "height")]
        [Range(typeof(decimal), "0", "10000", ErrorMessage = "rankCheck")]
        public Nullable<decimal> Height { get; set; }
        public Nullable<decimal> HeightSearch_Start { get; set; }
        public Nullable<decimal> HeightSearch_End { get; set; }
        public DateTime CreateDate { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string? CreatedById { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public string? ModifiedById { get; set; }
    }
}