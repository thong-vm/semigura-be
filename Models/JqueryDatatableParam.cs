using semigura.DBContext.Entities;

namespace semigura.Models
{
    public class JqueryDatatableParam
    {
        public string SEcho { get; set; }
        public string SSearch { get; set; }
        public int IDisplayLength { get; set; }
        public int IDisplayStart { get; set; }
        public int IColumns { get; set; }
        public int ISortCol_0 { get; set; }
        public string SSortDir_0 { get; set; }
        public int ISortingCols { get; set; }
        public string SColumns { get; set; }
        // ----
        public int TotalRecords { get; set; }
        public List<UserInfo> UserInfoList { get; set; }

    }
}