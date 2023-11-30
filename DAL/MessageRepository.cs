using Microsoft.EntityFrameworkCore;
using semigura.DBContext.Entities;

namespace semigura.DAL
{
    public class MessageRepository : IMessageRepository, IDisposable
    {
        private DBEntities db;
        public MessageRepository(DBEntities context)
        {
            this.db = context;
        }

        public IQueryable<Message> GetAll()
        {
            return db.Messages;
        }
        public Message GetByID(string id)
        {
            return db.Messages.FirstOrDefault(p => p.Id == id);
        }
        public void Add(Message Message)
        {
            db.Messages.Add(Message);
            db.SaveChanges();
        }

        public void SetModified(Message message)
        {
            db.Entry(message).State = EntityState.Modified;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await db.SaveChangesAsync();
        }

        public async Task<Message> FindAsync(string id)
        {
            return await db.Messages.FindAsync(id);
        }

        public Message Remove(Message message)
        {
            db.Remove(message);
            return message;
        }

        public bool Exists(string id)
        {
            return db.Messages.Count(e => e.Id == id) > 0;
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (db != null)
                {
                    db.Dispose();
                    db = null;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
