namespace Interfaces
{
    public interface IRepository<T> where T : class, IEntity
    {
        //Task<List<T>> GetAll();
        IQueryable<T> GetAll();
        //Task<T?> Get(string id);
        IQueryable<T> Get(string id);
        Task<T?> Add(T entity);
        Task<T> Update(T entity);
        Task<T?> Delete(string id);
    }
}
