using System.Net;
using IntegrationTests.Factories;
using IntegrationTests.Models.DTO.Actions;
using IntegrationTests.Models.DTO.Objects;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace IntegrationTests.Tests.ClubTests;

[Collection("ClubTests")]
public class UpdateClubTests : InjectableWebApplicationFactory
{
    private readonly ITestOutputHelper _testOutputHelper;

    public UpdateClubTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task UpdateClub_ShouldSucceed_WhenUpdateIsValid()
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
    public async Task UpdateClub_ShouldReturnBadRequest_WhenUpdateIsInvalid()
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
            Name = null
        };

        var updateRequest = await HttpClient.PatchAsJsonAsync($"api/v1/Club/UpdateClub", update);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, updateRequest.StatusCode);
    }

    [Fact]
    public async Task UpdateClub_ShouldReturnForbidden_WhenUserIsNotAnAdmin()
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
        
        var newUserId = await RegisterNewUser(new UserRegistrationDto
        {
            Username = "UpdateClub_ShouldReturnForbidden_WhenUserIsNotAnAdmin",
            Email = "UpdateClub_ShouldReturnForbidden_WhenUserIsNotAnAdmin@test.com",
            Password = "Test1234!",
            ConfirmPassword = "Test1234!"
        });

        var invitationCreateDto = new InvitationCreateDto();
        invitationCreateDto.ApplicationUserId = newUserId!.Value;
        invitationCreateDto.ClubId = clubId!.Value;
        
        var sendInviteRequest = await HttpClient.PostAsJsonAsync("api/v1/Invitation/SendInvitation", invitationCreateDto);

        var invitationId = await sendInviteRequest.Content.ReadFromJsonAsync<EntityIdDto>();

        var loggedInUserId = await LoginAsUser(new UserLoginDto { Email = "UpdateClub_ShouldReturnForbidden_WhenUserIsNotAnAdmin@test.com", Password = "Test1234!" });

        var acceptInvitationRequest = await HttpClient.PatchAsync($"api/v1/Invitation/AcceptInvitation?invitationId={invitationId!.Id}", null);

        var update = new ClubUpdateDto()
        {
            Id = clubId!.Value,
            Name = "Updated Club",
            Motto = "Updated Motto",
            IsPrivate = true,
            ImageUrl = "www.updated.com"
        };
        
        var updateRequest = await HttpClient.PatchAsJsonAsync($"api/v1/Club/UpdateClub?id={clubId}", update);

        var getClubRequest = await HttpClient.GetAsync($"api/v1/Club/GetClub?id={clubId}");
        var getClubResponse = await getClubRequest.Content.ReadFromJsonAsync<ClubDto>();
        
        // Assert
        Assert.Equal(newUserId, loggedInUserId);
        Assert.Equal(HttpStatusCode.OK, acceptInvitationRequest.StatusCode);

        Assert.Equal(HttpStatusCode.Forbidden, updateRequest.StatusCode);
        
        Assert.NotNull(getClubResponse);
        Assert.Equal(2, getClubResponse.MembershipIds.Count);
    }
    
    [Fact]
    public async Task UpdateClub_ShouldReturnNotFound_WhenClubDoesNotExist()
    {
        // Arrange
        var update = new ClubUpdateDto()
        {
            Id = Guid.Empty,
            Name = "Updated Club",
            Motto = "Updated Motto",
            IsPrivate = true,
            ImageUrl = "www.updated.com"
        };

        // Act
        await AuthoriseAdminSUT();
        var updateRequest = await HttpClient.PatchAsJsonAsync($"api/v1/Club/UpdateClub", update);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, updateRequest.StatusCode);
    }

    [Fact]
    public async Task UpdateMemberRole_ShouldSucceed_WhenRequestIsValid()
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
        var newUserId = await RegisterNewUser(new UserRegistrationDto
        {
            Username = "UpdateMemberRole_ShouldSucceed_WhenRequestIsValid",
            Email = "UpdateMemberRole_ShouldSucceed_WhenRequestIsValid@test.com",
            Password = "Test1234!",
            ConfirmPassword = "Test1234!"
        });

        var sendInviteRequest = await HttpClient.PostAsJsonAsync("api/v1/Invitation/SendInvitation", new InvitationCreateDto
        {
            ApplicationUserId = newUserId!.Value,
            ClubId = clubId!.Value
        });

        var invitationId = await sendInviteRequest.Content.ReadFromJsonAsync<EntityIdDto>();

        var loggedInUserId = await LoginAsUser(new UserLoginDto { Email = "UpdateMemberRole_ShouldSucceed_WhenRequestIsValid@test.com", Password = "Test1234!" });

        var acceptInvitationRequest = await HttpClient.PatchAsync($"api/v1/Invitation/AcceptInvitation?invitationId={invitationId!.Id}", null);

        await AuthoriseAdminSUT();
        
        var updateMemberRequest = await HttpClient.PatchAsync($"api/v1/Club/UpdateMemberRole?userId={loggedInUserId}&clubId={clubId}&isAdmin=true", null);

        var getMembersRequest = await HttpClient.GetFromJsonAsync<List<ClubMembershipDto>>($"api/v1/Club/GetClubMemberships?id={clubId}");
        var userMembership = getMembersRequest?.FirstOrDefault(x=>x.User.Id == loggedInUserId);

        // Assert
        Assert.Equal(newUserId, loggedInUserId);
        Assert.Equal(HttpStatusCode.OK, acceptInvitationRequest.StatusCode);
        Assert.Equal(HttpStatusCode.OK, updateMemberRequest.StatusCode);
        Assert.True(userMembership?.IsAdmin);
    }
    
    [Fact]
    public async Task UpdateMemberRole_ShouldReturnBadRequest_WhenRequestIsInvalid()
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
        var newUserId = await RegisterNewUser(new UserRegistrationDto
        {
            Username = "UpdateMemberRole_ShouldReturnBadRequest_WhenRequestIsInvalid",
            Email = "UpdateMemberRole_ShouldReturnBadRequest_WhenRequestIsInvalid@test.com",
            Password = "Test1234!",
            ConfirmPassword = "Test1234!"
        });

        var sendInviteRequest = await HttpClient.PostAsJsonAsync("api/v1/Invitation/SendInvitation", new InvitationCreateDto
        {
            ApplicationUserId = newUserId!.Value,
            ClubId = clubId!.Value
        });

        var invitationId = await sendInviteRequest.Content.ReadFromJsonAsync<EntityIdDto>();

        var loggedInUserId = await LoginAsUser(new UserLoginDto { Email = "UpdateMemberRole_ShouldReturnBadRequest_WhenRequestIsInvalid@test.com", Password = "Test1234!" });

        var acceptInvitationRequest = await HttpClient.PatchAsync($"api/v1/Invitation/AcceptInvitation?invitationId={invitationId!.Id}", null);

        await AuthoriseAdminSUT();
        
        var updateMemberRequest = await HttpClient.PatchAsync($"api/v1/Club/UpdateMemberRole?userId={loggedInUserId}&clubId={clubId}&isAdmin=false", null);

        var getMembersRequest = await HttpClient.GetFromJsonAsync<List<ClubMembershipDto>>($"api/v1/Club/GetClubMemberships?id={clubId}");
        var userMembership = getMembersRequest?.FirstOrDefault(x=>x.User.Id == loggedInUserId);

        // Assert
        Assert.Equal(newUserId, loggedInUserId);
        Assert.Equal(HttpStatusCode.OK, acceptInvitationRequest.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, updateMemberRequest.StatusCode);
        Assert.False(userMembership?.IsAdmin);
    }
    
    [Fact]
    public async Task UpdateMemberRole_ShouldReturnBadRequest_WhenUserIsTheOnlyAdmin()
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
        var adminId = AuthenticatedUserId;
        
        var clubId = await CreateClubAsync(club);
        var newUserId = await RegisterNewUser(new UserRegistrationDto
        {
            Username = "UpdateMemberRole_ShouldReturnBadRequest_WhenUserIsTheOnlyAdmin",
            Email = "UpdateMemberRole_ShouldReturnBadRequest_WhenUserIsTheOnlyAdmin@test.com",
            Password = "Test1234!",
            ConfirmPassword = "Test1234!"
        });

        var sendInviteRequest = await HttpClient.PostAsJsonAsync("api/v1/Invitation/SendInvitation", new InvitationCreateDto
        {
            ApplicationUserId = newUserId!.Value,
            ClubId = clubId!.Value
        });

        var invitationId = await sendInviteRequest.Content.ReadFromJsonAsync<EntityIdDto>();

        var loggedInUserId = await LoginAsUser(new UserLoginDto { Email = "UpdateMemberRole_ShouldReturnBadRequest_WhenUserIsTheOnlyAdmin@test.com", Password = "Test1234!" });

        var acceptInvitationRequest = await HttpClient.PatchAsync($"api/v1/Invitation/AcceptInvitation?invitationId={invitationId!.Id}", null);

        await AuthoriseAdminSUT();
        
        var updateMemberRequest = await HttpClient.PatchAsync($"api/v1/Club/UpdateMemberRole?userId={adminId}&clubId={clubId}&isAdmin=false", null);

        var getMembersRequest = await HttpClient.GetFromJsonAsync<List<ClubMembershipDto>>($"api/v1/Club/GetClubMemberships?id={clubId}");
        var userMembership = getMembersRequest?.FirstOrDefault(x=>x.User.Id == adminId);

        // Assert
        Assert.Equal(newUserId, loggedInUserId);
        Assert.Equal(HttpStatusCode.OK, acceptInvitationRequest.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, updateMemberRequest.StatusCode);
        Assert.True(userMembership?.IsAdmin);
    }
    
    [Fact]
    public async Task UpdateMemberRole_ShouldReturnForbidden_WhenUserIsNotAnAdmin()
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
        var newUserId = await RegisterNewUser(new UserRegistrationDto
        {
            Username = "UpdateMemberRole_ShouldReturnForbidden_WhenUserIsNotAnAdmin",
            Email = "UpdateMemberRole_ShouldReturnForbidden_WhenUserIsNotAnAdmin@test.com",
            Password = "Test1234!",
            ConfirmPassword = "Test1234!"
        });

        var sendInviteRequest = await HttpClient.PostAsJsonAsync("api/v1/Invitation/SendInvitation", new InvitationCreateDto
        {
            ApplicationUserId = newUserId!.Value,
            ClubId = clubId!.Value
        });

        var invitationId = await sendInviteRequest.Content.ReadFromJsonAsync<EntityIdDto>();

        var loggedInUserId = await LoginAsUser(new UserLoginDto { Email = "UpdateMemberRole_ShouldReturnForbidden_WhenUserIsNotAnAdmin@test.com", Password = "Test1234!" });

        var acceptInvitationRequest = await HttpClient.PatchAsync($"api/v1/Invitation/AcceptInvitation?invitationId={invitationId!.Id}", null);

        var updateMemberRequest = await HttpClient.PatchAsync($"api/v1/Club/UpdateMemberRole?userId={loggedInUserId}&clubId={clubId}&isAdmin=true", null);

        var getMembersRequest = await HttpClient.GetFromJsonAsync<List<ClubMembershipDto>>($"api/v1/Club/GetClubMemberships?id={clubId}");
        var userMembership = getMembersRequest?.FirstOrDefault(x=>x.User.Id == loggedInUserId);

        // Assert
        Assert.Equal(newUserId, loggedInUserId);
        Assert.Equal(HttpStatusCode.OK, acceptInvitationRequest.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, updateMemberRequest.StatusCode);
        Assert.False(userMembership?.IsAdmin);
    }
    
    [Fact]
    public async Task RemoveMember_ShouldReturnBadRequest_WhenClubHasOneMember()
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

        var removeMemberFromClubRequest = await HttpClient.DeleteAsync($"api/v1/Club/RemoveMemberFromClub?userId={AuthenticatedUserId}&clubId={clubId}");
        var leaveClubRequest = await HttpClient.DeleteAsync($"api/v1/Account/LeaveClub?clubId={clubId}");

        var clubRequest = await HttpClient.GetAsync($"api/v1/Club/GetClub?id={clubId}");
        var clubResponse = await clubRequest.Content.ReadFromJsonAsync<ClubDto>();
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, removeMemberFromClubRequest.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, leaveClubRequest.StatusCode);
        
        Assert.NotNull(clubResponse);
        Assert.Single(clubResponse.MembershipIds);
    }

    [Fact]
    public async Task RemoveMember_ShouldReturnBadRequest_WhenUserIsTheOnlyAdmin()
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
        var newUserId = await RegisterNewUser(new UserRegistrationDto
        {
            Username = "RemoveMember_ShouldReturnBadRequest_WhenUserIsTheOnlyAdmin",
            Email = "RemoveMember_ShouldReturnBadRequest_WhenUserIsTheOnlyAdmin@test.com",
            Password = "Test1234!",
            ConfirmPassword = "Test1234!"
        });

        var sendInviteRequest = await HttpClient.PostAsJsonAsync("api/v1/Invitation/SendInvitation", new InvitationCreateDto
        {
            ApplicationUserId = newUserId!.Value,
            ClubId = clubId!.Value
        });

        var invitationId = await sendInviteRequest.Content.ReadFromJsonAsync<EntityIdDto>();

        var loggedInUserId = await LoginAsUser(new UserLoginDto { Email = "RemoveMember_ShouldReturnBadRequest_WhenUserIsTheOnlyAdmin@test.com", Password = "Test1234!" });

        var acceptInvitationRequest = await HttpClient.PatchAsync($"api/v1/Invitation/AcceptInvitation?invitationId={invitationId!.Id}", null);

        // Re-login as admin
        await AuthoriseAdminSUT();

        var removeMemberFromClubRequest = await HttpClient.DeleteAsync($"api/v1/Club/RemoveMemberFromClub?userId={AuthenticatedUserId}&clubId={clubId}");
        var leaveClubRequest = await HttpClient.DeleteAsync($"api/v1/Account/LeaveClub?clubId={clubId}");

        var clubRequest = await HttpClient.GetAsync($"api/v1/Club/GetClub?id={clubId}");
        var clubResponse = await clubRequest.Content.ReadFromJsonAsync<ClubDto>();

        // Assert
        Assert.Equal(newUserId, loggedInUserId);
        Assert.Equal(HttpStatusCode.OK, acceptInvitationRequest.StatusCode);

        Assert.Equal(HttpStatusCode.BadRequest, removeMemberFromClubRequest.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, leaveClubRequest.StatusCode);

        Assert.NotNull(clubResponse);
        Assert.Equal(2, clubResponse.MembershipIds.Count);
    }
    
    [Fact]
    public async Task RemoveMember_ShouldReturnForbidden_WhenUserIsNotAnAdmin()
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

        var adminId = AuthenticatedUserId;
        
        var clubId = await CreateClubAsync(club);
        var newUserId = await RegisterNewUser(new UserRegistrationDto
        {
            Username = "RemoveMember_ShouldReturnForbidden_WhenUserIsNotAnAdmin",
            Email = "RemoveMember_ShouldReturnForbidden_WhenUserIsNotAnAdmin@test.com",
            Password = "Test1234!",
            ConfirmPassword = "Test1234!"
        });

        var sendInviteRequest = await HttpClient.PostAsJsonAsync("api/v1/Invitation/SendInvitation", new InvitationCreateDto
        {
            ApplicationUserId = newUserId!.Value,
            ClubId = clubId!.Value
        });

        var invitationId = await sendInviteRequest.Content.ReadFromJsonAsync<EntityIdDto>();

        var loggedInUserId = await LoginAsUser(new UserLoginDto { Email = "RemoveMember_ShouldReturnForbidden_WhenUserIsNotAnAdmin@test.com", Password = "Test1234!" });

        var acceptInvitationRequest = await HttpClient.PatchAsync($"api/v1/Invitation/AcceptInvitation?invitationId={invitationId!.Id}", null);

        var removeMemberFromClubRequest = await HttpClient.DeleteAsync($"api/v1/Club/RemoveMemberFromClub?userId={adminId}&clubId={clubId}");

        var clubRequest = await HttpClient.GetAsync($"api/v1/Club/GetClub?id={clubId}");
        var clubResponse = await clubRequest.Content.ReadFromJsonAsync<ClubDto>();

        // Assert
        Assert.Equal(newUserId, loggedInUserId);
        Assert.Equal(HttpStatusCode.OK, acceptInvitationRequest.StatusCode);

        Assert.Equal(HttpStatusCode.Forbidden, removeMemberFromClubRequest.StatusCode);

        Assert.NotNull(clubResponse);
        Assert.Equal(2, clubResponse.MembershipIds.Count);
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