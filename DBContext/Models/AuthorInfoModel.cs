namespace semigura.DBContext.Models
{
    public class AuthorInfoModel
    {
        public string Username { get; set; }
        public string GroupName { get; set; }
        public Nullable<int> Role { get; set; }
        public string ResourceId { get; set; }
        public string AppName { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string ResourceName { get; set; }
        public Nullable<int> Type { get; set; }
        public Nullable<int> Level { get; set; }
        public string ParentId { get; set; }
        public Nullable<int> Sort { get; set; }
        public string IconClass { get; set; }
        public List<AuthorInfoModel> ChildList { get; set; }

        //public string DisplayResource
        //{
        //    get
        //    {
        //        var label = this.ResourceName;
        //        if (!string.IsNullOrEmpty(this.ResourceName))
        //        {
        //            var obj = new System.Resources.ResourceManager(typeof(SharedResource)).GetString(this.ResourceName, System.Globalization.CultureInfo.CurrentCulture); //TODO
        //            if (obj != null)
        //            {
        //                label = obj;
        //            }
        //        }

        //        return label;
        //    }
        //}
    }
}