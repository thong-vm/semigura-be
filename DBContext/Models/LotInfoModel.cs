﻿//------------------------------------------------------------------------------
// <auto-generated>
//     このコードはテンプレートから生成されました。
//
//     このファイルを手動で変更すると、アプリケーションで予期しない動作が発生する可能性があります。
//     このファイルに対する手動の変更は、コードが再生成されると上書きされます。
// </auto-generated>
//------------------------------------------------------------------------------

namespace semigura.DBContext.Models
{

    public partial class LotModel
    {
        public string LotCode { get; set; }
        public string Factory { get; set; }
        public Nullable<System.DateTime> Start { get; set; }
        public Nullable<System.DateTime> End { get; set; }
        public string Rice { get; set; }
        public Nullable<int> Kubun { get; set; }
        public string Semaibuai { get; set; }
        public Nullable<decimal> Ratio { get; set; }

    }
}