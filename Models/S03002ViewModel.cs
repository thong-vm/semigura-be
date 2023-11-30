using semigura.DBContext.Entities;
using semigura.DBContext.Models;

namespace semigura.Models
{
    public class S03002ViewModel : BaseViewModel
    {
        public List<LotContainerModel>? LotContainerList { get; set; }
        public List<Factory>? ListFactory { get; set; }
        public List<Lot>? ListLot { get; set; }
        public List<semigura.DBContext.Entities.Location>? ListLocation { get; set; }
        public string? Id { get; set; }
        public string? FactoryId { get; set; }
        public string? LocationId { get; set; }
        public string? LotId { get; set; }
        public string? ContainerId { get; set; }
        public string? Code { get; set; }
        public bool IsInUse { get; set; }
    }
}