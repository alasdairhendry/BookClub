using Domain.Interfaces;
using IntegrationTests.Factories;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit.Abstractions;

namespace IntegrationTests.Tests;

/// <summary>
/// Checks the Auth flow for user registration and login
/// </summary>
public class AuthFlowTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly ITestOutputHelper _testOutputHelper;

    public AuthFlowTests(CustomWebApplicationFactory<Program> factory, ITestOutputHelper testOutputHelper)
    {
        _factory = factory;
        _testOutputHelper = testOutputHelper;

        _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }
   
    [Fact]
    public async Task Get_UserAuthFlowIsSuccessful_Stage_01_Register()
    {
        // Arrange
        var content = new IntegrationTests.Models.DTO.Actions.UserRegistrationModel
        {
            Username = "BobDylan",
            Email = "bobdylan@gmail.com",
            Password = "Test1234!",
            ConfirmPassword = "Test1234!",
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/Auth/Register", content);
        var responseString = await response.Content.ReadAsStringAsync();
        _testOutputHelper.WriteLine(responseString);

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
    }
    
    [Fact]
    public async Task Get_UserAuthFlowIsSuccessful_Stage_02_Login()
    {
        // Arrange
        var content = new IntegrationTests.Models.DTO.Actions.UserLoginModel()
        {
            Email = "bobdylan@gmail.com",
            Password = "Test1234!",
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/Auth/Login", content);
        var responseString = await response.Content.ReadAsStringAsync();
        _testOutputHelper.WriteLine(responseString);

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
    }
}