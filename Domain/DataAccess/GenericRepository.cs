using Data;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Domain.DataAccess;

public class GenericRepository<TEntity> where TEntity : class
{
    private ApplicationDbContext context;
    private DbSet<TEntity> dbSet;

    public GenericRepository(ApplicationDbContext context)
    {
        this.context = context;
        this.dbSet = context.Set<TEntity>();
    }

    public virtual async Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>>? filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, string includeProperties = "")
    {
        IQueryable<TEntity> query = dbSet;

        if (filter != null)
        {
            query = query.Where(filter);
        }

        foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
        {
            query = query.Include(includeProperty);
        }

        if (orderBy != null)
        {
            return await orderBy(query).ToListAsync();
        }
        else
        {
            return await query.ToListAsync(();
        }
    }

    public virtual TEntity? GetByID(object id)
    {
        return dbSet.Find(id);
    }
    
    public virtual async Task<TEntity?> GetByIDAsync(object id)
    {
        return await dbSet.FindAsync(id);
    }

    public virtual void Insert(TEntity entity)
    {
        dbSet.Add(entity);
    }
    
    public virtual async Task InsertAsync(TEntity entity)
    {
        await dbSet.AddAsync(entity);
    }

    public virtual void Delete(object id)
    {
        TEntity? entityToDelete = dbSet.Find(id);

        if (entityToDelete != null)
            Delete(entityToDelete);
    }

    public virtual void Delete(TEntity entityToDelete)
    {
        if (context.Entry(entityToDelete).State == EntityState.Detached)
        {
            dbSet.Attach(entityToDelete);
        }

        dbSet.Remove(entityToDelete);
    }

    public virtual void Update(TEntity entityToUpdate)
    {
        dbSet.Attach(entityToUpdate);
        context.Entry(entityToUpdate).State = EntityState.Modified;
    }
}