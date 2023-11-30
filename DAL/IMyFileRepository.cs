namespace semigura.DAL
{
    public interface IMyFileRepository
    {
        void Add(MyFile myFile);
        void Dispose();
        bool Exists(string id);
        Task<MyFile> FindAsync(string id);
        IQueryable<MyFile> GetAll();
        MyFile GetById(string id);
        MyFile GetByName(string userName);
        string GetUploadPath();
        MyFile Remove(MyFile myFile);
        Task<int> SaveChangesAsync();
    }
}