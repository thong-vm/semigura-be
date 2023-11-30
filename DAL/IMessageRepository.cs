namespace semigura.DAL
{
    public interface IMessageRepository : IDisposable
    {
        Message GetByID(string id);
        void Add(Message Message);
        Task<int> SaveChangesAsync();
        Task<Message> FindAsync(string id);
        Message Remove(Message message);
        bool Exists(string id);
        void SetModified(Message message);
        IQueryable<Message> GetAll();
    }
}
