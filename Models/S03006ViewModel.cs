using semigura.DBContext.Models;

namespace semigura.Models
{
    public class S03006ViewModel : BaseViewModel
    {
        public List<ContainerModel>? ListContainer { get; set; }

        public string? ContainerId { get; set; }
    }
}