using Models;
using semigura.DBContext.Entities;
using Template;

namespace Controllers
{
    public class SensorsController : TController<Sensor,TRepository<Sensor, DBEntities>>
    {
        public SensorsController(TRepository<Sensor, DBEntities> repository) : base(repository) 
        {
        }
    }
}
