using semigura.DBContext.Entities;
using Template;

namespace semigura.DAL
{
    public class UserRepository : TRepository<User, DBEntities>
    {
        public UserRepository(DBEntities dbContext) : base(dbContext) { }
    }
}
