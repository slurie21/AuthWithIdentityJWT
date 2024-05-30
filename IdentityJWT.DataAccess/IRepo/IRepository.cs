using System.Linq.Expressions;

namespace IdentityJWT.DataAccess.IRepo
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll(string? includeProperties = null);
        T Get(Expression<Func<T, bool>> filter, string? includeProperties = null);
        Task<bool> Add(T entity);

        Task<bool> Update(T entity);

        Task<bool> Remove(T entity);
        Task<int> DeleteUsingFilter(T entity, Func<T, bool> filter);
        void DeleteRange(IEnumerable<T> entities);
    }
}
