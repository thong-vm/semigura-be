namespace semigura.DBContext.Models
{
    public class UserDataModel
    {
        public UserInfoModel? userInfoModel { get; set; }
        public List<AuthorInfoModel>? authorInfoModelList { get; set; }
    }
}