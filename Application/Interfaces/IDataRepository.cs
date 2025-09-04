using System.Linq.Expressions;

namespace Application.Interfaces;

public interface IDataRepository<TEntity> where TEntity : class
{
    Task<TEntity?> GetAsync(Guid? id);
    Task<List<TEntity>> GetAllAsync();
    Task<int> GetCount(Expression<Func<TEntity, bool>>? filter = null);

    Task<IEnumerable<TEntity>> QueryAsync(Expression<Func<TEntity, bool>>? filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, string includeProperties = "");
    Task<TEntity?> QueryAsSingleAsync(Expression<Func<TEntity, bool>>? filter = null, string includeProperties = "");

    Task InsertAsync(TEntity entity);
    void Update(TEntity entity);

    void Delete(TEntity entity);
    Task DeleteAsync(object? id);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}