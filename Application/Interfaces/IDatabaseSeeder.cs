using Application.Models.State;

namespace Application.Interfaces;

public interface IDatabaseSeeder
{
    Task<ResultState> SeedClubInvitations();
}