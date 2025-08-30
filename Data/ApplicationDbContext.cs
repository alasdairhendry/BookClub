using Data.Models.Dbo;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUserDbo, IdentityRole<Guid>, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :
        base(options)
    { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.Entity<ClubMembershipDbo>()
            .HasOne(e => e.Club)
            .WithMany(e => e.ClubMemberships)
            .HasForeignKey(e => e.ClubId)
            .IsRequired();
    }

    public DbSet<ClubActivityDbo> Activities { get; set; }
    public DbSet<ClubActivitySectionDbo> ActivitySections { get; set; }
    public new DbSet<ApplicationUserDbo> Users { get; set; }
    public DbSet<ClubDbo> Clubs { get; set; }
    public DbSet<ClubMembershipDbo> ClubMemberships { get; set; }
    public DbSet<CommentDbo> Comments { get; set; }
    public DbSet<InvitationDbo> Invitations { get; set; }
    public DbSet<RecordDbo> Records { get; set; }
}