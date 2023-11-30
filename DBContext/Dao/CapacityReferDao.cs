using semigura.DBContext.Entities;

namespace semigura.DBContext.Repositories
{
    public class CapacityReferDao
    {
        private readonly DBEntities context;

        public CapacityReferDao(DBEntities context)
        {
            this.context = context;
        }

        public List<CapacityRefer> GetListCapacityRefer(string containerId)
        {
            return context.CapacityRefer.Where(s => s.ContainerId == containerId).ToList();
        }
    }
}