using semigura.Commons;
using semigura.DBContext.Entities;
using semigura.DBContext.Models;
using System.ComponentModel.DataAnnotations;

namespace semigura.Models
{
    public class S02001ViewModel : BaseViewModel
    {
        public List<Factory>? FactoryList { get; set; }
        public bool IsInUse { get; set; }
        public List<Lot>? LotList { get; set; }
        public List<ContainerModel>? TankList { get; set; }
        public string? FactoryId { get; set; }
        public string? LotId { get; set; }
        public string? LotContainerId { get; set; }
        public Nullable<System.DateTime> LotContainerStartDate { get; set; }
        public Nullable<System.DateTime> LotContainerEndDate { get; set; }


        public LastestTemperatureDataModel? LastestTempData { get; set; }
        public List<TableColumnDataModel>? ListTableColumnData { get; set; }
        public ChartDataModel? ChartData { get; set; }
        public string? CameraDeviceCode { get; set; }
        public List<MediaModel>? ListMedia { get; set; }


        public class LastestTemperatureDataModel
        {
            public string LocationId { get; set; }
            public string LocationName { get; set; }
            public Nullable<decimal> LocationTemperature { get; set; }
            public Nullable<decimal> LocationHumidity { get; set; }
            public Nullable<decimal> ProductTemperature1 { get; set; }
            public Nullable<decimal> ProductTemperature2 { get; set; }
            public Nullable<decimal> ProductTemperature3 { get; set; }
            public Nullable<decimal> ProductTemperatureAvg { get; set; }
            public Nullable<decimal> BaumeDegree { get; set; }
            public Nullable<decimal> AlcoholDegree { get; set; }
            public Nullable<decimal> Acid { get; set; }
            public Nullable<decimal> AminoAcid { get; set; }
            public Nullable<decimal> Glucose { get; set; }
            public Nullable<System.DateTime> DataEntryMeasuareDate { get; set; }
            public string DataEntryMeasuareDateUnixTimeStamp
            {
                get
                {
                    return DataEntryMeasuareDate.HasValue ? Utils.DateTimeToLongTimeString(DataEntryMeasuareDate) : null;
                }
            }
            public Nullable<System.DateTime> LocationMeasuareDate { get; set; }
            public string LocationMeasuareDateUnixTimeStamp
            {
                get
                {
                    return LocationMeasuareDate.HasValue ? Utils.DateTimeToLongTimeString(LocationMeasuareDate) : null;
                }
            }
            public Nullable<System.DateTime> ProductMeasuareDate { get; set; }
            public string ProductMeasuareDateUnixTimeStamp
            {
                get
                {
                    return ProductMeasuareDate.HasValue ? Utils.DateTimeToLongTimeString(ProductMeasuareDate) : null;
                }
            }
            public Nullable<System.DateTime> LotEndDate { get; set; }
            public string LotEndDateUnixTimeStamp
            {
                get
                {
                    return LotEndDate.HasValue ? Utils.DateTimeToLongTimeString(LotEndDate) : null;
                }
            }
        }


        public class TableColumnDataModel
        {
            public string Id_LocationTemperature { get; set; }

            public string Id_TemperatureAvg { get; set; }

            public string IdTerminal_LocationTemperature { get; set; }

            public string IdTerminal_TemperatureAvg { get; set; }

            public string LotcontainerId_LocationTemperature { get; set; }

            public string LotcontainerId_TemperatureAvg { get; set; }

            public string Id_DataEntry { get; set; }

            public string Id_Container { get; set; }
            public int DayNum { get; set; }
            public Nullable<System.DateTime> MeasuareDate { get; set; }
            public string MeasuareDateUnixTimeStamp
            {
                get
                {
                    return MeasuareDate.HasValue ? Utils.DateTimeToLongTimeString(MeasuareDate) : null;
                }
            }
            public Nullable<decimal> LocationTemperature { get; set; }
            public Nullable<decimal> ProductTemperature1 { get; set; }
            public Nullable<decimal> ProductTemperature2 { get; set; }
            public Nullable<decimal> ProductTemperature3 { get; set; }
            public Nullable<decimal> ProductTemperatureAvg { get; set; }
            public Nullable<decimal> BaumeDegree { get; set; }
            public Nullable<decimal> BaumeSakeDegree { get; set; }
            public Nullable<decimal> AlcoholDegree { get; set; }
            public Nullable<decimal> Acid { get; set; }
            public Nullable<decimal> AminoAcid { get; set; }
            public Nullable<decimal> Glucose { get; set; }
        }

        public class ChartDataModel
        {
            public List<string> ListMeasuareUnixTimestamp { get; set; }
            public List<Nullable<decimal>> ListLocationTemperature { get; set; }
            public List<Nullable<decimal>> ListProductTemperatureAvg { get; set; }
            public List<Nullable<decimal>> ListBaumeDegree { get; set; }
            public List<Nullable<decimal>> ListAlcoholDegree { get; set; }

        }


        // ↓ DataEntry画面 -------------------------------------------------------------------------
        public DateTime? SearchByDate { get; set; }
        public string? DataEntryId { get; set; }
        public DataEntryModel? DataEntry { get; set; }
        public List<DataEntryModel>? DataEntryList { get; set; }

        public class DataEntryModel
        {
            public string Id { get; set; }
            public string LotContainerId { get; set; }
            [Display(Name = "baume1")]
            [Range(typeof(decimal), "-1000", "1000", ErrorMessage = "rankCheck")]
            public Nullable<decimal> BaumeDegree { get; set; }
            [Display(Name = "alcohol")]
            [Range(typeof(decimal), "-1000", "1000", ErrorMessage = "rankCheck")]
            public Nullable<decimal> AlcoholDegree { get; set; }
            [Display(Name = "acid")]
            [Range(typeof(decimal), "-1000", "1000", ErrorMessage = "rankCheck")]
            public Nullable<decimal> Acid { get; set; }
            [Display(Name = "amomiAcid")]
            [Range(typeof(decimal), "-1000", "1000", ErrorMessage = "rankCheck")]
            public Nullable<decimal> AminoAcid { get; set; }
            [Display(Name = "glucose")]
            [Range(typeof(decimal), "-1000", "1000", ErrorMessage = "rankCheck")]
            public Nullable<decimal> Glucose { get; set; }
            public string Note { get; set; }
            [Display(Name = "dateTime")]
            [Required(ErrorMessage = "C01001")]
            public System.DateTime MeasureDate { get; set; }
            public string MeasureDateUnixTimeStamp
            {
                get
                {
                    return Utils.DateTimeToLongTimeString(MeasureDate);
                }
            }
            public Nullable<System.DateTime> CreatedOn { get; set; }
            public string CreatedById { get; set; }
            public Nullable<System.DateTime> ModifiedOn { get; set; }
            public string ModifiedById { get; set; }
        }

        public class TemperateEdit
        {
            public string Id_LocationTemperature { get; set; }

            public string LocationTemperature { get; set; }

            public string Id_TemperatureAvg { get; set; }

            public string ProductTemperatureAvg { get; set; }

            public DateTime MeasuareDateUnixTimeStamp { get; set; }

            public string IdTerminal_LocationTemperature { get; set; }

            public string IdTerminal_TemperatureAvg { get; set; }

            public string LotcontainerId_LocationTemperature { get; set; }

            public string LotcontainerId_TemperatureAvg { get; set; }

        }
    }
}