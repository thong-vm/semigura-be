//------------------------------------------------------------------------------
// <auto-generated>
//     このコードはテンプレートから生成されました。
//
//     このファイルを手動で変更すると、アプリケーションで予期しない動作が発生する可能性があります。
//     このファイルに対する手動の変更は、コードが再生成されると上書きされます。
// </auto-generated>
//------------------------------------------------------------------------------

namespace semigura.DBContext.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Resource")]
    public partial class Resource
    {
        public string Id { get; set; }
        public string AppName { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string ResourceName { get; set; }
        public Nullable<int> Type { get; set; }
        public Nullable<int> Level { get; set; }
        public string ParentId { get; set; }
        public Nullable<int> Sort { get; set; }
        public string IconClass { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string CreatedById { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public string ModifiedById { get; set; }
    }
}
