using semigura.DBContext.Models;
using System.ComponentModel.DataAnnotations;

namespace semigura.Models
{
    public class S09001ViewModel : BaseViewModel
    {
        public List<MaterialModel>? MaterialList { get; set; }
        public string? Id { get; set; }
        [Display(Name = "material")]
        [Required(ErrorMessage = "C01001")]
        [StringLength(50, ErrorMessage = "lengthCheck")]
        public string? Name { get; set; }
        [StringLength(500, ErrorMessage = "lengthCheck")]
        public string? Note { get; set; }
        [Display(Name = "temp_min")]
        [Required(ErrorMessage = "C01001")]
        [Range(typeof(decimal), "-273", "1000", ErrorMessage = "rankCheck")]
        public Nullable<decimal> TempMin { get; set; }
        [Display(Name = "temp_max")]
        [Required(ErrorMessage = "C01001")]
        [Range(typeof(decimal), "-273", "1000", ErrorMessage = "rankCheck")]
        public Nullable<decimal> TempMax { get; set; }
        public Nullable<decimal> HumidityMin { get; set; }
        public Nullable<decimal> HumidityMax { get; set; }
        [Display(Name = "type")]
        [Required(ErrorMessage = "C01001")]
        public int? Type { get; set; }
        public string? Type2 { get; set; }

        public List<S09001ViewModel>? MaterialStandValList { get; set; }
    }
}