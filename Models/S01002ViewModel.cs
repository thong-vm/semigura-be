using semigura.DBContext.Entities;


namespace semigura.Models
{
    public class S01002ViewModel : BaseViewModel
    {
        public List<Factory>? FactoryList { get; set; }

        public string? FactoryId { get; set; }
    }
}