using BroadcastControl.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;
using MediatR;

// ============================================================
// PROGRAM.CS - Main Application Entry Point
// ============================================================
// This file configures and starts the Blazor Server application.
// Think of it as the "main()" method in Python or other languages.

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// 1. CONFIGURE LOGGING (Serilog)
// ============================================================
// Serilog writes logs to files for debugging and troubleshooting.
// Logs are stored in the "Logs" folder with daily rotation.

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration) // Read from appsettings.json
    .Enrich.FromLogContext() // Add Context info (Who/What Trigger
    .WriteTo.Console() //Also write to Console for Development
    .WriteTo.File(
        path: "Logs/app-.log", //File path with Date Placeholder
        rollingInterval: RollingInterval.Day, // New file each day
        retainedFileCountLimit: 30 // Keep Last 30 days of logs
    )
    .CreateLogger();

builder.Host.UseSerilog(); // Tell ASP.NET to use Serilog

Log.Information("🚀 Starting Broadcast Control Application...");

// ============================================================
// 2. ADD SERVICES TO THE CONTAINER
// ============================================================
// "Dependency Injection" - register services so Blazor pages can use them.
// Services are created automatically when needed and shared across requests.

// --- Blazor & Web Services ---
builder.Services.AddRazorPages(); //Enables Razor page routing
builder.Services.AddServerSideBlazor(); // Enables Blaszor Server (WebSocket-based UI)

// --- Database (Entity Framework Core + SQLite) ---
builder.Services.AddDbContext<AppDbContext>(options =>
{
    // Connect to SQLite database file
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlite(connectionString);

    // Enable detailed loging in Development mode
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging(); //Shows actual SQL parameter values
        options.EnableDetailedErrors(); //More verbose error messages
    }
});

// --- MediatR (Event Bus) ---
// MediatR allows services to publish events and other services to react.
// Example: When roster updates, UI automatically refreshes.
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// --- Application Services (we'll create these) ---
// TODO: Uncomment as you create each service
// builder.Services.AddScoped<IResetService, ResetService>();
// builder.Services.AddScoped<ISheetsIngestService, SheetsIngestService>();
// builder.Services.AddSingleton<IVmixClient, VmixClient>();
// builder.Services.AddScoped<IStatsService, StatsService>();
// builder.Services.AddScoped<ILiveStateService, LiveStateService>();
// builder.Services.AddScoped<IActionLogger, ActionLogger>();

// --- Background Workers (polling, monitoring) ---
// TODO: Uncomment when workers are created
// builder.Services.AddHostedService<SheetPollerWorker>();
// builder.Services.AddHostedService<VmixMonitorWorker>();

// --- http client (for external apis if needed) ---
builder.Services.AddHttpClient();

// ============================================================
// 3. BUILD THE APPLICATION
// ============================================================
var app = builder.Build();

// ============================================================
// 4. APPLY DATABASE MIGRATIONS ON STARTUP
// ============================================================
// Automatically updates the database schema to match our models.
// This ensures the database is always up-to-date when the app starts.

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        Log.Information("📊 Checking database migrations...");
        
        // Apply any pending migrations
        db.Database.Migrate();
        
        //Configure SQLite for optimal performance 
        db.Database.ExecuteSqlRaw("PRAGMA journal_mode=WAL;"); // Write-Ahead Logging
        db.Database.ExecuteSqlRaw("PRAGMA foreign_keys=ON;"); // Enforce foreign keys
        
        Log.Information("✅ Database ready!");
    }
    catch (Exception ex)
    {
        Log.Error(ex, "❌ Database migration failed!");
        throw; // Stop the app if database setup fails
    }
}

// ============================================================
// 5. CONFIGURE HTTP REQUEST PIPELINE
// ============================================================
// This section defines how the app handles incoming HTTP requests.

if (!app.Environment.IsDevelopment())
{
    // Production: Use error page instead of detailed exceptions
    app.UseExceptionHandler("/Error");
    app.UseHsts(); // HTTP Strict Transport Security
}

// Redirect HTTP to HTTPS (security best practice)
// Comment out during development if not using SSL certificates
// app.UseHttpsRedirection();

// Serve static files (CSS, JS, images) from wwwroot folder
app.UseStaticFiles();

// Set up routing (matches URLs to Pages)
app.UseRouting();

// Map Blazor components to routes
app.MapBlazorHub(); //WebSocket endpoint for Blazor Server
app.MapFallbackToPage("/_Host"); //Default Page

// ============================================================
// 6. RUN THE APPLICATION
// ============================================================
Log.Information("🌐 Application starting on http://localhost:5000");
Log.Information("👉 Press Ctrl+C to shut down");

try
{
    app.Run(); // Starts the web server and blocks until shutdown
}
catch (Exception ex)
{
    Log.Fatal(ex, "💥 Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush(); // Ensure all logs are written before exit
}

// ============================================================
// WHAT HAPPENS WHEN YOU RUN THIS?
// ============================================================
// 1. Serilog starts logging to console and files
// 2. All services are registered in the dependency injection container
// 3. Database migrations are applied (creates/updates tables)
// 4. Web server starts on port 5000
// 5. Browser can connect to http://localhost:5000
// 6. Multiple browsers can connect simultaneously (Blazor Server handles this)
//
// To run:
// - In Rider: Click the green ▶ button at top-right
// - In Terminal: dotnet run
