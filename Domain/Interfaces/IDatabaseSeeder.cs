using Domain.Models.State;

namespace Domain.Interfaces;

public interface IDatabaseSeeder
{
    Task<ResultState> SeedClubInvitations();
}