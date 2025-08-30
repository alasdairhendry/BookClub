using Data;
using Data.Models.Dbo;
using Microsoft.EntityFrameworkCore;

namespace Domain.DataAccess;

public class UnitOfWork : IDisposable
{
    private ApplicationDbContext context;
    private GenericRepository<ClubActivityDbo> activityRepository = null!;
    private GenericRepository<ClubActivitySectionDbo> activitySectionRepository = null!;
    private GenericRepository<ApplicationUserDbo> applicationUserRepository = null!;
    private GenericRepository<ClubDbo> clubRepository = null!;
    private GenericRepository<CommentDbo> commentRepository = null!;
    private GenericRepository<RecordDbo> recordRepository = null!;

    public UnitOfWork(ApplicationDbContext context)
    {
        this.context = context;
    }
    
    public GenericRepository<ClubActivityDbo> ActivityRepository
    {
        get
        {
            if (this.activityRepository == null)
                this.activityRepository = new GenericRepository<ClubActivityDbo>(context);

            return activityRepository;
        }
    }

    public GenericRepository<ClubActivitySectionDbo> ActivitySectionRepository
    {
        get
        {
            if (this.activitySectionRepository == null)
                this.activitySectionRepository = new GenericRepository<ClubActivitySectionDbo>(context);

            return activitySectionRepository;
        }
    }

    public GenericRepository<ApplicationUserDbo> ApplicationUserRepository
    {
        get
        {
            if (this.applicationUserRepository == null)
                this.applicationUserRepository = new GenericRepository<ApplicationUserDbo>(context);

            return applicationUserRepository;
        }
    }

    public GenericRepository<ClubDbo> ClubRepository
    {
        get
        {
            if (this.clubRepository == null)
                this.clubRepository = new GenericRepository<ClubDbo>(context);

            return clubRepository;
        }
    }

    public GenericRepository<CommentDbo> CommentRepository
    {
        get
        {
            if (this.commentRepository == null)
                this.commentRepository = new GenericRepository<CommentDbo>(context);

            return commentRepository;
        }
    }

    public GenericRepository<RecordDbo> RecordRepository
    {
        get
        {
            if (this.recordRepository == null)
                this.recordRepository = new GenericRepository<RecordDbo>(context);

            return recordRepository;
        }
    }

    public void Save()
    {
        context.SaveChanges();
    }

    public async Task SaveAsync()
    {
        await context.SaveChangesAsync();
    }

    private bool disposed = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                context.Dispose();
            }
        }

        this.disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}