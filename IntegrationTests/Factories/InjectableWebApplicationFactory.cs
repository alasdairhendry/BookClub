using System.Data.Common;
using Data;
using Domain.Models.DTO.Objects;
using IntegrationTests.Models.DTO.Actions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace IntegrationTests.Factories;

public class InjectableWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    protected HttpClient HttpClient { get; private set; } = null!;
    protected Guid? AuthenticatedUserId { get; private set; } = null!;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                     typeof(IDbContextOptionsConfiguration<ApplicationDbContext>));

            services.Remove(dbContextDescriptor);

            var dbConnectionDescriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                     typeof(DbConnection));

            services.Remove(dbConnectionDescriptor);

            services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("AppDatabase"));
        });

        builder.UseEnvironment("Development");
    }

    public async Task InitializeAsync()
    {
        HttpClient = CreateClient();
        await AuthoriseSUT();
    }

    /// <summary>
    /// Register and logs in a user account to allow access to restricted endpoints
    /// </summary>
    protected async Task AuthoriseSUT()
    {
        await Register();
        await Login();
    }

    private async Task Register()
    {
        // Arrange
        var content = new UserRegistrationModel
        {
            Username = "Admin",
            Email = "admin@gmail.com",
            Password = "Test1234!",
            ConfirmPassword = "Test1234!",
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync("/api/v1/Auth/Register", content);
        var responseString = await response.Content.ReadAsStringAsync();

        // Assert
        // response.EnsureSuccessStatusCode(); // Status Code 200-299
    }

    private async Task Login()
    {
        // Arrange
        var content = new UserLoginModel()
        {
            Email = "admin@gmail.com",
            Password = "Test1234!",
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync("/api/v1/Auth/Login", content);
        var responseObject = await response.Content.ReadFromJsonAsync<EntityIdDto>();

        AuthenticatedUserId = responseObject.Id;

        // Assert
        // response.EnsureSuccessStatusCode(); // Status Code 200-299
    }

    public new async Task DisposeAsync()
    {
        await Task.Delay(0);
    }
}