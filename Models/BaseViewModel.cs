using semigura.DBContext.Models;

namespace semigura.Models
{
    public class BaseViewModel
    {
        public List<AuthorInfoModel> AuthorInfoModelList
        {
            get
            {
                List<AuthorInfoModel> authorInfoList = new List<AuthorInfoModel>();
                //if (HttpContext.Current.Session != null && HttpContext.Current.Session[Properties.AUTHOR_INFO] != null)
                //{
                //    authorInfoList = (List<AuthorInfoModel>)HttpContext.Current.Session[Properties.AUTHOR_INFO];
                //}
                return authorInfoList;
            }
        }

        public UserInfoModel UserInfo
        {
            get
            {
                UserInfoModel userObj = new UserInfoModel();

                //if (HttpContext.Current.Session != null && HttpContext.Current.Session[Properties.USER_INFO] != null)
                //{
                //    userObj = (UserInfoModel)HttpContext.Current.Session[Properties.USER_INFO];
                //}

                return userObj;
            }
        }

        // Datatableのパラメータ
        public string? SEcho { get; set; }
        public string? SSearch { get; set; }
        public int IDisplayLength { get; set; }
        public int IDisplayStart { get; set; }
        public int IColumns { get; set; }
        public int ISortCol_0 { get; set; }
        public string? SSortDir_0 { get; set; }
        public int ISortingCols { get; set; }
        public string? SColumns { get; set; }
        public int TotalRecords { get; set; }

        public string? ClientTimezoneOffset { get; set; }
    }
}