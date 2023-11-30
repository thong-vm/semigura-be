using Microsoft.Extensions.Localization;

namespace semigura.DBContext.Models
{
    public class MaterialModel
    {
        public IStringLocalizer localizer;

        public int No { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string RicePolishingRatioName { get; set; }
        public Nullable<decimal> RicePolishingRatio { get; set; }
        public string Yeast { get; set; }
        public Nullable<int> PreparationCombination { get; set; }
        public string Note { get; set; }
        public Nullable<decimal> TempMin { get; set; }
        public Nullable<decimal> TempMax { get; set; }
        public Nullable<decimal> HumidityMin { get; set; }
        public Nullable<decimal> HumidityMax { get; set; }
        public Nullable<int> Type { get; set; }
        public string TypeLabel
        {
            get
            {
                if (localizer == null) { return "localizer is null"; }
                var label = string.Empty;
                if (Type.HasValue && Type.Value == semigura.Commons.Properties.MATERIALSTANDVAL_TYPE_TANK)
                {
                    label = localizer["type_tank"];
                }
                else if (Type.HasValue && Type.Value == semigura.Commons.Properties.MATERIALSTANDVAL_TYPE_SEIGIKU)
                {
                    label = localizer["type_seigiku"];
                }
                else if (Type.HasValue && Type.Value == semigura.Commons.Properties.MATERIALSTANDVAL_TYPE_LOCATION)
                {
                    label = localizer["type_location"];
                }

                return label;
            }
        }
    }
}