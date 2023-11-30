using semigura.Commons;
using semigura.DBContext.Entities;
using semigura.DBContext.Models;
using System.ComponentModel.DataAnnotations;

namespace semigura.Models
{
    public class S03001ViewModel : BaseViewModel
    {
        public List<LotContainerModel>? LotContainerList { get; set; }

        public List<Factory>? FactoryList { get; set; }

        public List<semigura.DBContext.Entities.Location>? ListLocation { get; set; }

        public List<Rice>? RiceList { get; set; }

        public List<Semaibuai>? SeimaibuaiList { get; set; }

        public List<Kubun>? KubunList { get; set; }

        public List<Tank>? TankList { get; set; }

        public List<Terminal>? SensorList { get; set; }

        [Display(Name = "factory")]
        [Required(ErrorMessage = "C01001")]
        public string FactoryID { get; set; } = null!;

        [Display(Name = "location")]
        [Required(ErrorMessage = "C01001")]
        public string LocationID { get; set; } = null!;

        public string? LotId { get; set; }

        public string? RiceId { get; set; }

        public string? SemaibuaiId { get; set; }

        public string? SemaibuaiValue { get; set; }

        public Nullable<int> KubunId { get; set; }

        public Nullable<decimal> RicePolishingRatio { get; set; }

        public Nullable<decimal> MinTempSeigiku { get; set; }
        public Nullable<decimal> MaxTempSeigiku { get; set; }


        [Display(Name = "lot")]
        [Required(ErrorMessage = "C01001")]
        public string LotCode { get; set; } = null!;

        public string? TankCode { get; set; }


        public List<Tank>? TankListId { get; set; }

        public List<Terminal>? SensorListId { get; set; }


        public bool IsInUse { get; set; }


        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> StartDate { get; set; }
        public string? StartDateUnixTimeStamp
        {
            get
            {
                return StartDate.HasValue ? Utils.DateTimeToLongTimeString(StartDate) : null;
            }
        }

        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> EndDate { get; set; }
        public string? EndDateUnixTimeStamp
        {
            get
            {
                return EndDate.HasValue ? Utils.DateTimeToLongTimeString(EndDate) : null;
            }
        }

        public Rice? RiceInfo { get; set; }

        //public string RiceName { get; set; }

        public class Factory
        {
            public string? Id { get; set; }
            public string? Code { get; set; }
            public string? Name { get; set; }
        }

        public class Rice
        {
            public string? Id { get; set; }
            public string? Name { get; set; }

            public Nullable<decimal> MinTempMoromi { get; set; }
            public Nullable<decimal> MaxTempMoromi { get; set; }

            public Nullable<decimal> MinTempSeigiku { get; set; }
            public Nullable<decimal> MaxTempSeigiku { get; set; }
        }

        public class Semaibuai
        {
            public string? Id { get; set; }
            public string? Name { get; set; }
        }

        public class Kubun
        {
            public Nullable<int> Id { get; set; }
            public string? Name { get; set; }
        }


        public class Tank
        {
            public string? Id { get; set; }
            public string? Code { get; set; }
            public Nullable<decimal> MinTemp { get; set; }
            public Nullable<decimal> MaxTemp { get; set; }

            public Nullable<System.DateTime> StartDate { get; set; }
            public Nullable<System.DateTime> EndDate { get; set; }
            public Nullable<bool> DeleteFlg { get; set; }
            public string? IdOld { get; set; }
        }

        public class Terminal
        {
            public string? Id { get; set; }
            public string? Code { get; set; }
            public string? Name { get; set; }
            public Nullable<bool> DeleteFlg { get; set; }

            public Nullable<System.DateTime> StartDate { get; set; }
            public Nullable<System.DateTime> EndDate { get; set; }
            public int Type { get; set; }
            public string? IdOld { get; set; }
        }

        //public string sltRiceName { get; set; }
        //public SelectList sltListRice { get; set; }

        //public string sltRiceID { get; set; }
        //public SelectList sltListSeimaibuai { get; set; }

    }
}