using System.Net;
using IntegrationTests.Factories;
using IntegrationTests.Models.DTO.Actions;
using IntegrationTests.Models.DTO.Objects;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace IntegrationTests.Tests.ClubTests;

[Collection("ClubTests")]
public class DeleteClubTests : InjectableWebApplicationFactory
{
    private readonly ITestOutputHelper _testOutputHelper;

    public DeleteClubTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task DeleteClub_ShouldSucceed_WhenClubExists()
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

        var update = new ClubUpdateDto()
        {
            Id = clubId!.Value,
            Name = "Updated Club",
            Motto = "Updated Motto",
            IsPrivate = true,
            ImageUrl = "www.updated.com"
        };

        var updateRequest = await HttpClient.PatchAsJsonAsync($"api/v1/Club/UpdateClub", update);
        var getRequest = await HttpClient.GetAsync($"api/v1/Club/GetClub?id={clubId}");
        var getResponse = await getRequest.Content.ReadFromJsonAsync<ClubDto>();

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, updateRequest.StatusCode);
        Assert.NotNull(getResponse);
#pragma warning disable xUnit2002
        Assert.NotNull(getResponse.Id);
#pragma warning restore xUnit2002
        Assert.Equal(update.Name, getResponse.Name);
        Assert.Equal(update.Motto, getResponse.Motto);
        Assert.Equal(update.IsPrivate, getResponse.IsPrivate);
        Assert.Equal(update.ImageUrl, getResponse.ImageUrl);
        Assert.Single(getResponse.MembershipIds);
    }

    [Fact]
    public async Task DeleteClub_ShouldReturnForbidden_WhenUserIsNotAnAdmin()
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

        var clubId = await CreateClubAsync(club);

        var update = new ClubUpdateDto()
        {
            Id = clubId!.Value,
            Name = "Updated Club",
            Motto = "Updated Motto",
            IsPrivate = true,
            ImageUrl = "www.updated.com"
        };

        var newUserId = await RegisterNewUser(new UserRegistrationDto
        {
            Username = "DeleteClub_ShouldReturnForbidden_WhenUserIsNotAnAdmin",
            Email = "DeleteClub_ShouldReturnForbidden_WhenUserIsNotAnAdmin@test.com",
            Password = "Test1234!",
            ConfirmPassword = "Test1234!"
        });

        var sendInviteRequest = await HttpClient.PostAsJsonAsync("api/v1/Invitation/SendInvitation", new InvitationCreateDto
        {
            ApplicationUserId = newUserId!.Value,
            ClubId = clubId!.Value
        });

        var invitationId = await sendInviteRequest.Content.ReadFromJsonAsync<EntityIdDto>();

        var loggedInUserId = await LoginAsUser(new UserLoginDto { Email = "DeleteClub_ShouldReturnForbidden_WhenUserIsNotAnAdmin@test.com", Password = "Test1234!" });

        var acceptInvitationRequest = await HttpClient.PatchAsync($"api/v1/Invitation/AcceptInvitation?invitationId={invitationId!.Id}", null);

        var deleteRequest = await HttpClient.DeleteAsync($"api/v1/Club/DeleteClub?id={clubId}");

        // Assert
        Assert.Equal(newUserId, loggedInUserId);
        Assert.Equal(HttpStatusCode.OK, acceptInvitationRequest.StatusCode);

        Assert.Equal(HttpStatusCode.Forbidden, deleteRequest.StatusCode);
    }

    private async Task<Guid?> CreateClubAsync(ClubCreateDto club)
    {
        var request = await HttpClient.PostAsJsonAsync("api/v1/Club/CreateClub", club);
        var response = await request.Content.ReadFromJsonAsync<EntityIdDto>();

        return response!.Id;
    }

    private async Task<Guid?> RegisterNewUser(UserRegistrationDto dto)
    {
        var request = await HttpClient.PostAsJsonAsync("api/v1/Auth/Register", dto);
        var response = await request.Content.ReadFromJsonAsync<EntityIdDto>();

        return response!.Id;
    }

    private async Task<Guid?> LoginAsUser(UserLoginDto dto)
    {
        var request = await HttpClient.PostAsJsonAsync("api/v1/Auth/Login", dto);
        var response = await request.Content.ReadFromJsonAsync<EntityIdDto>();

        return response!.Id;
    }
}