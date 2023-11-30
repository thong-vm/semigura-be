using System.ComponentModel.DataAnnotations;

namespace semigura.Models
{
    public class S09002ViewModel : BaseViewModel
    {
        public List<S09002ViewModel>? AlertMailList { get; set; }
        public int RowNo { get; set; }
        public string? Id { get; set; }
        [Display(Name = "email")]
        [Required(ErrorMessage = "C01001")]
        [EmailAddress(ErrorMessage = "invalid_mail_format")]
        public string? Email { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string? CreatedById { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public string? ModifiedById { get; set; }

    }
}