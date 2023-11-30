
using Models;
using Template;
using semigura.DBContext.Entities;

namespace Repositories
{
    public class SensorRepository : TRepository<Sensor, DBEntities>
    {
        public SensorRepository (DBEntities context) : base(context)
        {

        }
    }
}
