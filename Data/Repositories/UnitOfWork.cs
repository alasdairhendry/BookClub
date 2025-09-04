using Application.Interfaces;
using Domain.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Data.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ApplicationDbContext _context;
    private bool _disposed = false;

    private GenericDictionary<Type> genericMap = new(); 
    
    public UnitOfWork(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _context = serviceProvider.GetRequiredService<ApplicationDbContext>();
    }
    
    public IDataRepository<T> GetRepository<T>() where T : class
    {
        var cache = genericMap.GetValue<IDataRepository<T>>(typeof(T));

        if (cache is not null)
            return cache;
        
        var service = _serviceProvider.GetRequiredService<IDataRepository<T>>();
        genericMap.Add(typeof(T), service);
        
        return service;
    }

    public Task<int> SaveAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
    
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}