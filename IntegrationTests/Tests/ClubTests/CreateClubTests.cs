using System.Net;
using IntegrationTests.Factories;
using IntegrationTests.Models.DTO.Actions;
using IntegrationTests.Models.DTO.Objects;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace IntegrationTests.Tests.ClubTests;

[Collection("ClubTests")]
public class CreateClubTests : InjectableWebApplicationFactory
{
    private readonly ITestOutputHelper _testOutputHelper;

    public CreateClubTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }
    
    [Fact]
    public async Task CreateClub_ShouldSucceed_WhenRequestIsValid()
    {
        // Arrange
        var club = new ClubCreateDto()
        {
            Name = "Test Club",
            Motto = "Test Motto",
            IsPrivate = false,
            ImageUrl = "www.test.com"
        };
        
        // Act
        await AuthoriseAdminSUT();
        
        var request = await HttpClient.PostAsJsonAsync("api/v1/Club/CreateClub", club);
        
        _testOutputHelper.WriteLine(request.StatusCode.ToString());
        _testOutputHelper.WriteLine(await request.Content.ReadAsStringAsync());
        
        var response = await request.Content.ReadFromJsonAsync<EntityIdDto>();
        
        // Assert
        Assert.Equal(HttpStatusCode.Created, request.StatusCode);
        Assert.NotNull(response?.Id);
        Assert.IsType<Guid>(response.Id);
    }
    
    [Fact]
    public async Task CreateClub_ShouldFail_WhenRequestIsInvalid()
    {
        // Arrange
        var club = new ClubCreateDto()
        {
            Name = null!
        };
        
        // Act
        await AuthoriseAdminSUT();
        
        var request = await HttpClient.PostAsJsonAsync("api/v1/Club/CreateClub", club);
        
        _testOutputHelper.WriteLine(request.StatusCode.ToString());
        _testOutputHelper.WriteLine(await request.Content.ReadAsStringAsync());
        
        var response = await request.Content.ReadFromJsonAsync<EntityIdDto>();
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, request.StatusCode);
    }
}