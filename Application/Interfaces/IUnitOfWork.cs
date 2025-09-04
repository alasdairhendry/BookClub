using Application.Interfaces;
using Domain.Interfaces.Dbo;

namespace Data.Repositories;

public interface IUnitOfWork : IDisposable
{
    IDataRepository<T> GetRepository<T>() where T : class;
    Task<int> SaveAsync(CancellationToken cancellationToken = default);
}