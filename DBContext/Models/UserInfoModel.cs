namespace semigura.DBContext.Models
{

    public class UserInfoModel
    {
        public string? Id { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string? CreatedById { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public string? ModifiedById { get; set; }
        public bool? IsSysAdmin { get; set; }
    }
}