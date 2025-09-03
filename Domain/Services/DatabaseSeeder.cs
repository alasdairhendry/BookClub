using Data;
using Domain.DataAccess;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Models.DTO.Actions;
using Domain.Models.State;

namespace Domain.Services;

public class DatabaseSeeder : IDatabaseSeeder
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IUserService _userService;
    private readonly IClubService _clubService;
    private readonly IInvitationService _invitationService;

    public DatabaseSeeder(ApplicationDbContext dbContext, IUserService userService, IClubService clubService, IInvitationService invitationService)
    {
        _dbContext = dbContext;
        _userService = userService;
        _clubService = clubService;
        _invitationService = invitationService;
    }

    public async Task<ResultState> SeedClubInvitations()
    {
        try
        {
            // return ResultState.Success();
            using var work = new UnitOfWork(_dbContext);

            var davidId = Guid.Parse("1234f747-48c6-4e89-b3b2-35c8d3701234");
            var johnId = Guid.Parse("abcd6616-1729-4178-9d35-12b093acabcd");
            await _userService.RegisterWithSpecificId(new UserRegistrationDto { Username = "David", Email = "david@gmail.com", Password = "Test1234!", ConfirmPassword = "Test1234!" }, davidId);
            await _userService.RegisterWithSpecificId(new UserRegistrationDto { Username = "John", Email = "john@gmail.com", Password = "Test1234!", ConfirmPassword = "Test1234!" }, johnId);

            // await _userService.Login(new UserLoginModel { Email = "david@gmail.com", Password = "Test1234!" });

            // var clubId = await _clubService.CreateClub(new ClubCreateDto { Name = "My New Club" });

            // if (clubId?.Data is null)
                // throw new Exception("Club is null");
            
            // var invitationId = await _invitationService.SendInvitation(new InvitationCreateDto { ApplicationUserId = johnId, ClubId = clubId.Data.Value });

            // Console.WriteLine("Invitation Id");
            // Console.WriteLine(invitationId.Data);
            
            return ResultState.Success();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return ResultState.Failed(ResultErrorType.Exception,e.Message);
        }
    }
}