using Microsoft.EntityFrameworkCore;
using BroadcastControl.Data.Entities;
using BroadcastControl.Data.Entitites;
using Google.Apis.Sheets.v4.Data;
using Action = System.Action;

namespace BroadcastControl.Data;

// ============================================================
// APP DB CONTEXT - Database Configuration
// ============================================================
// This class is the "connection" to your SQLite database.
// Think of it as a Python class that manages your database tables.
//
// DbContext does three main things:
// 1. Defines which tables exist (DbSet<T> properties)
// 2. Configures table relationships and constraints (OnModelCreating)
// 3. Provides methods to query and save data (LINQ queries)

public class AppDbContext : DbContext
{
    // ============================================================
    // CONSTRUCTOR
    // ============================================================
    // The "options" parameter is provided by dependency injection.
    // It contains the SQLite connection string from appsettings.json.

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    // ============================================================
    // DATABASE TABLES (DbSet Properties)
    // ============================================================
    // Each DbSet<T> represents a table in the database.
    // Example: DbSet<Session> becomes a "Session" table.
    //
    // You query these like lists:
    //   var sessions = await _db.Session.ToListAsync();
    //   var session = await _db.Session.FindAsync(sessionId);

    // --- Core Tables ---
    public DbSet<Session> Session { get; set; } = null!;
    public DbSet<Setting> Settings { get; set; } = null!;
    public DbSet<Roster> Roster { get; set; } = null!;
    public DbSet<SheetSource> SheetSource { get; set; } = null!;
    public DbSet<ImportLog> ImportLogs { get; set; } = null!;

    // --- State Tables---
    public DbSet<LiveState> LiveState { get; set; } = null!;
    public DbSet<VmixStatus> VmixStatus { get; set; } = null!;
    public DbSet<SystemStatus> SystemStatus { get; set; } = null!;

    // --- Action Logging ---
    public DbSet<Action> Actions { get; set; } = null!;

    // --- Per-Sport Stats Table ---

    public DbSet<BasketballStats> BasketballStats { get; set; } = null!;
    public DbSet<FootballStats> FootballStats { get; set; } = null!;
    public DbSet<VolleyballStats> VolleyballStats { get; set; } = null!;
    public DbSet<SoccerStats> SoccerStats { get; set; } = null!;
    public DbSet<TennisStats> TennisStats { get; set; } = null!;
    public DbSet<SwimmingStats> SwimmingStats { get; set; } = null!;
    public DbSet<TrackFieldStats> TrackFieldStats { get; set; } = null!;
    public DbSet<TrackFieldStatsHS> TrackFieldStatsHS { get; set; } = null!;
    public DbSet<WrestlingStats> WrestlingStats { get; set; } = null!;
    public DbSet<CrossCountryStats> CrossCountryStats { get; set; } = null!;
    public DbSet<GolfStats> GolfStats { get; set; } = null!;
    public DbSet<BowlingStats> BowlingStats { get; set; } = null!;
    public DbSet<BaseballStats> BaseballStats { get; set; } = null!;
    public DbSet<SoftballStats> SoftballStats { get; set; } = null!;

    // ============================================================
    // MODEL CONFIGURATION (OnModelCreating)
    // ============================================================
    // This method is called when EF Core creates the database schema.
    // Here we configure:
    // - Primary keys (especially composite keys)
    // - Foreign key relationships
    // - Indexes for fast queries
    // - Table constraints (CHECK, UNIQUE, etc.)
    // - Default values

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ========================================
        // SESSION TABLE
        // ========================================
        modelBuilder.Entity<Session>(entity =>
        {
            entity.HasKey(e => e.SessionId); // Primary Key

            // Index for fast lookups by sport
            entity.HasIndex(e => e.Sport);

            // Index for finding active sessions (Where StoppedUtc is Null
            entity.HasIndex(e => e.StoppedUtc);
        });

        // ========================================
        // SETTINGS TABLE
        // ========================================
        modelBuilder.Entity<Setting>(entity =>
        {
            entity.HasKey(e => e.Key); // Primary Key

            // Index by category for grouping setings
            entity.HasIndex(e => e.Category);
        });

        // ========================================
        // ROSTER TABLE
        // ======================================== 
        modelBuilder.Entity<Roster>(entity =>
        {
            entity.HasKey(e => e.DisplayId); // Primary Key "A001", "B023", etc.

            // Composite Index for fast team+number lookups
            entity.HasIndex(e => new { e.TeamCode, e.Number })
                .HasDatabaseName("IX_Roster_Team_Number");

            // Index for filtering active players
            entity.HasIndex(e => e.IsActive);

            // Constraint: TeamCode must be A-H
            entity.HasCheckConstraint("CK_Roster_TeamCode",
                "TeamCode IN ('A', 'B', 'C', 'D', 'E', 'F', 'G', 'H')"
            );

            // Constraint: Number must be positive
            entity.HasCheckConstraint(
                "CK_Roster_Number",
                "Number > 0 AND Number < 1000"
            );
        });

        // ========================================
        // SHEET SOURCE TABLE
        // ========================================        
        modelBuilder.Entity<SheetSource>(entity =>
        {
            entity.HasKey(e => e.DisplayId); // Primary Key

            // Foreign key: DisplayID refrences Roster.DisplayID
            // On DELETE CASCADE : If Roster Row is deleted, delete
            entity.HasOne<Roster>()
                .WithOne()
                .HasForeignKey<SheetSource>(e => e.DisplayId)
                .OnDelete(DeleteBehavior.Cascade);

            // Index for finding rows by sheet name
            entity.HasIndex(e => e.SheetName);
        });

        // ========================================
        // IMPORT LOG TABLE
        // ========================================  
        modelBuilder.Entity<ImportLog>(entity =>
        {
            entity.HasKey(e => e.Id); // Primary Key (auto-increment)
            // Index for filtering by log level  (Info, Warn, Error)
            entity.HasIndex(e => e.Level);

            // Index for time based queries (most recent logs)
            entity.HasIndex(e => e.CreatedUtc);
        });

        // ========================================
        // LIVE STATE TABLE (Singleton)
        // ========================================
        modelBuilder.Entity<LiveState>(entity =>
        {
            entity.HasKey(e => e.SessionId); // Primary Key 

            // Foreign key to Session
            entity.HasOne<Session>()
                .WithOne()
                .HasForeignKey<LiveState>(e => e.SessionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ========================================
        // VMIX STATUS TABLE (Singleton)
        // ========================================
        modelBuilder.Entity<VmixStatus>(entity =>
        {
            entity.HasKey(e => e.Id); // Primary Key (always 1)

            // Enforce single row: only Id=1 is allowed
            entity.HasCheckConstraint(
                "CK_VmixStatus_Singleton", "Id = 1"
            );
        });

        // ========================================
        // ACTIONS TABLE
        // ========================================

        modelBuilder.Entity<Action>(entity =>
        {
            entity.HasKey(e => e.Id);

            // Foreign key to Session
            entity.HasOne<Session>()
                .WithMany()
                .HasForeignKey(e => e.SessionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Index for querying actions by session
            entity.HasIndex(e => e.SessionId);

            // Index for time-based queries (Most recent first)

            entity.HasIndex(e => e.WhenUtc)
                .IsDescending(); // Optimize for ORDER BY WhenUtc DESC


        });

        // ========================================
        // BASKETBALL STATS TABLE
        // ========================================
        modelBuilder.Entity<BasketballStats>(entity =>
        {
            // Composite Primary Key: (SessionId, DisplayId)
            entity.HasKey(e => new { e.SessionId, e.DisplayId });

            // Foreign keys
            entity.HasOne<Session>()
                .WithMany()
                .HasForeignKey(e => e.SessionId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<Roster>()
                .WithMany()
                .HasForeignKey(e => e.DisplayId)
                .OnDelete(DeleteBehavior.Cascade);

        });
        // ========================================
        // FOOTBALL STATS TABLE
        // ========================================
        modelBuilder.Entity<FootballStats>(entity =>
        {
            // Composite Primary Key: (SessionId, DisplayId)
            entity.HasKey(e => new { e.SessionId, e.DisplayId });

            // Foreign keys
            entity.HasOne<Session>()
                .WithMany()
                .HasForeignKey(e => e.SessionId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<Roster>()
                .WithMany()
                .HasForeignKey(e => e.DisplayId)
                .OnDelete(DeleteBehavior.Cascade);

        });
        // ========================================
        // SOCCER STATS TABLE
        // ========================================
        modelBuilder.Entity<SoccerStats>(entity =>
        {
            // Composite Primary Key: (SessionId, DisplayId)
            entity.HasKey(e => new { e.SessionId, e.DisplayId });

            // Foreign keys
            entity.HasOne<Session>()
                .WithMany()
                .HasForeignKey(e => e.SessionId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<Roster>()
                .WithMany()
                .HasForeignKey(e => e.DisplayId)
                .OnDelete(DeleteBehavior.Cascade);

        });
        // ========================================
        // VOLLEYBALL STATS TABLE
        // ========================================
        modelBuilder.Entity<VolleyballStats>(entity =>
        {
            // Composite Primary Key: (SessionId, DisplayId)
            entity.HasKey(e => new { e.SessionId, e.DisplayId });

            // Foreign keys
            entity.HasOne<Session>()
                .WithMany()
                .HasForeignKey(e => e.SessionId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<Roster>()
                .WithMany()
                .HasForeignKey(e => e.DisplayId)
                .OnDelete(DeleteBehavior.Cascade);

        });
        // ========================================
        // TENNIS STATS TABLE
        // ========================================
        modelBuilder.Entity<TennisStats>(entity =>
        {
            // Composite Primary Key: (SessionId, DisplayId)
            entity.HasKey(e => new { e.SessionId, e.DisplayId });

            // Foreign keys
            entity.HasOne<Session>()
                .WithMany()
                .HasForeignKey(e => e.SessionId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<Roster>()
                .WithMany()
                .HasForeignKey(e => e.DisplayId)
                .OnDelete(DeleteBehavior.Cascade);

        });
        // ========================================
        // SWIMMING STATS TABLE
        // ========================================
        modelBuilder.Entity<SwimmingStats>(entity =>
        {
            // Composite Primary Key: (SessionId, DisplayId)
            entity.HasKey(e => new { e.SessionId, e.DisplayId });

            // Foreign keys
            entity.HasOne<Session>()
                .WithMany()
                .HasForeignKey(e => e.SessionId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<Roster>()
                .WithMany()
                .HasForeignKey(e => e.DisplayId)
                .OnDelete(DeleteBehavior.Cascade);

        });
        // ========================================
        // TRACK & FIELD STATS TABLE
        // ========================================
        modelBuilder.Entity<TrackFieldStats>(entity =>
        {
            // Composite Primary Key: (SessionId, DisplayId)
            entity.HasKey(e => new { e.SessionId, e.DisplayId });

            // Foreign keys
            entity.HasOne<Session>()
                .WithMany()
                .HasForeignKey(e => e.SessionId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<Roster>()
                .WithMany()
                .HasForeignKey(e => e.DisplayId)
                .OnDelete(DeleteBehavior.Cascade);

        });
        // ========================================
        // TRACK & FIELD HS STATS TABLE
        // ========================================
        modelBuilder.Entity<TrackFieldStatsHS>(entity =>
        {
            // Composite Primary Key: (SessionId, DisplayId)
            entity.HasKey(e => new { e.SessionId, e.DisplayId });

            // Foreign keys
            entity.HasOne<Session>()
                .WithMany()
                .HasForeignKey(e => e.SessionId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<Roster>()
                .WithMany()
                .HasForeignKey(e => e.DisplayId)
                .OnDelete(DeleteBehavior.Cascade);

        });

        // ========================================
        // WRESTLING STATS TABLE
        // ========================================
        modelBuilder.Entity<WrestlingStats>(entity =>
        {
            // Composite Primary Key: (SessionId, DisplayId)
            entity.HasKey(e => new { e.SessionId, e.DisplayId });

            // Foreign keys
            entity.HasOne<Session>()
                .WithMany()
                .HasForeignKey(e => e.SessionId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<Roster>()
                .WithMany()
                .HasForeignKey(e => e.DisplayId)
                .OnDelete(DeleteBehavior.Cascade);

        });

        // ========================================
        // CROSS COUNTRY STATS TABLE
        // ========================================
        modelBuilder.Entity<CrossCountryStats>(entity =>
        {
            // Composite Primary Key: (SessionId, DisplayId)
            entity.HasKey(e => new { e.SessionId, e.DisplayId });

            // Foreign keys
            entity.HasOne<Session>()
                .WithMany()
                .HasForeignKey(e => e.SessionId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<Roster>()
                .WithMany()
                .HasForeignKey(e => e.DisplayId)
                .OnDelete(DeleteBehavior.Cascade);

        });

        // ========================================
        // GOLF STATS TABLE
        // ========================================
        modelBuilder.Entity<GolfStats>(entity =>
        {
            // Composite Primary Key: (SessionId, DisplayId)
            entity.HasKey(e => new { e.SessionId, e.DisplayId });

            // Foreign keys
            entity.HasOne<Session>()
                .WithMany()
                .HasForeignKey(e => e.SessionId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<Roster>()
                .WithMany()
                .HasForeignKey(e => e.DisplayId)
                .OnDelete(DeleteBehavior.Cascade);

        });
        // ========================================
        //  BOWLING STATS TABLE
        // ========================================
        modelBuilder.Entity<BowlingStats>(entity =>
        {
            // Composite Primary Key: (SessionId, DisplayId)
            entity.HasKey(e => new { e.SessionId, e.DisplayId });

            // Foreign keys
            entity.HasOne<Session>()
                .WithMany()
                .HasForeignKey(e => e.SessionId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<Roster>()
                .WithMany()
                .HasForeignKey(e => e.DisplayId)
                .OnDelete(DeleteBehavior.Cascade);

        });
        // ========================================
        // BASEBALL STATS TABLE
        // ========================================
        modelBuilder.Entity<BaseballStats>(entity =>
        {
            // Composite Primary Key: (SessionId, DisplayId)
            entity.HasKey(e => new { e.SessionId, e.DisplayId });

            // Foreign keys
            entity.HasOne<Session>()
                .WithMany()
                .HasForeignKey(e => e.SessionId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<Roster>()
                .WithMany()
                .HasForeignKey(e => e.DisplayId)
                .OnDelete(DeleteBehavior.Cascade);

        });

        // ========================================
        //  SOFTBALL STATS TABLE
        // ========================================
        modelBuilder.Entity<SoftballStats>(entity =>
        {
            // Composite Primary Key: (SessionId, DisplayId)
            entity.HasKey(e => new { e.SessionId, e.DisplayId });

            // Foreign keys
            entity.HasOne<Session>()
                .WithMany()
                .HasForeignKey(e => e.SessionId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<Roster>()
                .WithMany()
                .HasForeignKey(e => e.DisplayId)
                .OnDelete(DeleteBehavior.Cascade);

        });
    }
}

// ============================================================
// HOW TO USE THIS IN YOUR CODE
// ============================================================
// The AppDbContext is injected into your services and pages:
//
// Example in a Blazor page:
// @inject AppDbContext _db
//
// protected override async Task OnInitializedAsync()
// {
//     var sessions = await _db.Session.ToListAsync();
// }
//
// Example in a service:
// public class ResetService
// {
//     private readonly AppDbContext _db;
//     
//     public ResetService(AppDbContext db)
//     {
//         _db = db;
//     }
//     
//     public async Task ResetAsync()
//     {
//         _db.Roster.RemoveRange(_db.Roster); // Delete all
//         await _db.SaveChangesAsync(); // Commit to database
//     }
// }




