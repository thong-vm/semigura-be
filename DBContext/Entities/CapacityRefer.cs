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

    [Table("CapacityRefer")]
    public partial class CapacityRefer
    {
        public string Id { get; set; }
        public string ContainerId { get; set; }
        public decimal Depth { get; set; }
        public decimal Capacity { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string CreatedById { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public string ModifiedById { get; set; }
    }
}
