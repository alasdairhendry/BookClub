using System.Net;
using IntegrationTests.Factories;
using IntegrationTests.Models.DTO.Actions;
using IntegrationTests.Models.DTO.Objects;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace IntegrationTests.Tests.ClubTests;

[Collection("ClubTests")]
public class GetClubTests : InjectableWebApplicationFactory
{
    private readonly ITestOutputHelper _testOutputHelper;

    public GetClubTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task GetClubById_ShouldReturnClub_WhenClubExists()
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
        var clubId = await CreateClubAsync(club);

        var request = await HttpClient.GetAsync($"api/v1/Club/GetClub?id={clubId}");
        var response = await request.Content.ReadFromJsonAsync<ClubDto>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, request.StatusCode);
        Assert.Equal(clubId, response!.Id);
        Assert.Equal(club.Name, response!.Name);
        Assert.Equal(club.Motto, response.Motto);
        Assert.Equal(club.IsPrivate, response.IsPrivate);
        Assert.Equal(club.ImageUrl, response.ImageUrl);
        Assert.Single(response.MembershipIds);
    }

    [Fact]
    public async Task GetClubById_ShouldReturnNotFound_WhenClubDoesNotExist()
    {
        // Arrange
        var guid = Guid.NewGuid();
        
        // Act
        var request = await HttpClient.GetAsync($"api/v1/Club/GetClub?id={guid}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, request.StatusCode);
    }
    
    [Fact]
    public async Task GetClubMembershipById_ShouldReturnClubMembership_WhenClubExists()
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
        var clubId = await CreateClubAsync(club);

        var createRequest = await HttpClient.GetAsync($"api/v1/Club/GetClub?id={clubId}");
        var fetchRequest = await HttpClient.GetAsync($"api/v1/Club/GetClubMemberships?id={clubId}");
        var response = await fetchRequest.Content.ReadFromJsonAsync<List<ClubMembershipDto>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, createRequest.StatusCode);
        Assert.NotNull(response);
        Assert.Single(response);
        Assert.Equal(AuthenticatedUserId, response.First().User.Id);
        Assert.True(response.First().IsAdmin);
    }

    [Fact]
    public async Task GetClubMembershipById_ShouldReturnNotFound_WhenClubDoesNotExist()
    {
        // Arrange
        var guid = Guid.NewGuid();
        
        // Act
        var request = await HttpClient.GetAsync($"api/v1/Club/GetClubMemberships?id={guid}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, request.StatusCode);
    }

    private async Task<Guid?> CreateClubAsync(ClubCreateDto club)
    {
        var request = await HttpClient.PostAsJsonAsync("api/v1/Club/CreateClub", club);
        var response = await request.Content.ReadFromJsonAsync<EntityIdDto>();

        return response!.Id;
    }
}