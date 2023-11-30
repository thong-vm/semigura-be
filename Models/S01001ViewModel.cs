using System.ComponentModel.DataAnnotations;

namespace semigura.Models
{
    public class S01001ViewModel
    {
        //[Required(ErrorMessageResourceName = "S01001_01", ErrorMessageResourceType = typeof(CResources))]
        [Display(Name = "username")]
        [Required(ErrorMessage = "C01001")]
        public string Username { get; set; } = null!;

        [DataType(DataType.Password)]
        //[Required(ErrorMessageResourceName = "S01001_02", ErrorMessageResourceType = typeof(CResources))]
        [Display(Name = "password")]
        [Required(ErrorMessage = "C01001")]
        public string Password { get; set; } = null!;

        [Display(Name = "remember_me")]
        public bool IsRememberMe { get; set; }
        public string? ReturnUrl { get; set; }
        public bool LoginByAjax { get; set; }
        public string? OldUsername { get; set; }
    }
}