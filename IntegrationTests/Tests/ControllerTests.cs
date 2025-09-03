using System.Net;
using IntegrationTests.Factories;
using Xunit.Abstractions;
using HttpMethod = Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpMethod;

namespace IntegrationTests.Tests;

/// <summary>
/// Tests that active Controller methods are responding correctly (haven't been renamed, etc)
/// </summary>
public class ControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly ITestOutputHelper _testOutputHelper;

    public ControllerTests(CustomWebApplicationFactory<Program> factory, ITestOutputHelper testOutputHelper)
    {
        _factory = factory;
        _testOutputHelper = testOutputHelper;
    }

    [Theory]
    [InlineData("/api/v1/Account/GetUserDetails", HttpMethod.Get)]
    [InlineData("/api/v1/Account/GetUserClubMemberships", HttpMethod.Get)]
    [InlineData("/api/v1/Account/GetUserClubInvitations", HttpMethod.Get), ]
    [InlineData("/api/v1/Account/LeaveClub", HttpMethod.Delete)]
    
    [InlineData("/api/v1/Auth/Register", HttpMethod.Post)]
    [InlineData("/api/v1/Auth/Login", HttpMethod.Post)]
    [InlineData("/api/v1/Auth/GetToken", HttpMethod.Post)]
    
    [InlineData("/api/v1/Club/GetClub", HttpMethod.Get)]
    [InlineData("/api/v1/Club/GetClubs", HttpMethod.Get)]
    [InlineData("/api/v1/Club/GetClubMemberships", HttpMethod.Get)]
    [InlineData("/api/v1/Club/CreateClub", HttpMethod.Post)]
    [InlineData("/api/v1/Club/UpdateClub", HttpMethod.Patch)]
    [InlineData("/api/v1/Club/UpdateMemberRole", HttpMethod.Patch)]
    [InlineData("/api/v1/Club/RemoveMemberFromClub", HttpMethod.Delete)]
    [InlineData("/api/v1/Club/DeleteClub", HttpMethod.Delete)]
    
    [InlineData("/api/v1/Invitation/SendInvitation", HttpMethod.Post)]
    [InlineData("/api/v1/Invitation/AcceptInvitation", HttpMethod.Patch)]
    [InlineData("/api/v1/Invitation/DeclineInvitation", HttpMethod.Patch)]
    public async Task ApiController_Endpoint_Exists(string url, HttpMethod method)
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        HttpResponseMessage response;

        switch (method)
        {
            case HttpMethod.Get:
                response = await client.GetAsync(url);
                break;
            case HttpMethod.Post:
                response = await client.PostAsync(url, new StringContent(string.Empty));
                break;
            case HttpMethod.Put:
                response = await client.PutAsync(url, new StringContent(string.Empty));
                break;
            case HttpMethod.Patch:
                response = await client.PatchAsync(url, new StringContent(string.Empty));
                break;
            case HttpMethod.Delete:
                response = await client.DeleteAsync(url);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(method), method, null);
        }

        // Assert
        var actual = response.StatusCode;
        Assert.NotEqual(HttpStatusCode.NotFound, actual);
        Assert.NotEqual(HttpStatusCode.NotImplemented, actual);
        Assert.NotEqual(HttpStatusCode.MethodNotAllowed, actual);
    }
}