using Data;
using Domain.Models.Dbo;
using Application.Interfaces;
using Application.Services;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTests.Factories;

public static class ServiceFactory
{
    private static IServiceProvider? _serviceProvider;

    public static T GetRequiredService<T>()
    {
        GetProvider();
        
        return _serviceProvider!.GetRequiredService<T>();
    }
    
    private static IServiceProvider GetProvider()
    {
        if (_serviceProvider == null)
        {
            var collection = new ServiceCollection();

            collection.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            collection.AddScoped<IAccountService, AccountService>();
            collection.AddScoped<IClubService, ClubService>();
            collection.AddScoped<IHttpContextService, HttpContextService>();
            collection.AddScoped<IInvitationService, InvitationService>();
            collection.AddScoped<IPermissionService, PermissionService>();
            collection.AddScoped<IUserService, UserService>();

            collection.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("AppDatabase"));
            collection.AddIdentityApiEndpoints<ApplicationUserDbo>().AddEntityFrameworkStores<ApplicationDbContext>();
            collection.AddAuthentication();
            collection.AddAuthorization();

            _serviceProvider = collection.BuildServiceProvider();
        }

        return _serviceProvider;
    }
}