namespace semigura.DAL
{
    public interface IUserRepository
    {
        int Add(User user);
        bool Exists(string account);
        IQueryable<User> GetAll();
        User GetByName(string Account);
        int Update(User user);
    }
}
