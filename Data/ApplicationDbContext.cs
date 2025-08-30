using Data.Models.Dbo;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUserDbo>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :
        base(options)
    { }
    
    public DbSet<ActivityDbo> Activities { get; set; }
    public DbSet<ActivitySectionDbo> ActivitySections { get; set; }
    public new DbSet<ApplicationUserDbo> Users { get; set; }
    public DbSet<ClubDbo> Clubs { get; set; }
    public DbSet<CommentDbo> Comments { get; set; }
    public DbSet<RecordDbo> Records { get; set; }
}