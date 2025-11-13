using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BroadcastControl.Data.Entitites;

// ============================================================
// ENTITY CLASSES - Database Models
// ============================================================
// These classes represent tables in the SQLite database.
// Each property becomes a column in the table.
//
// Key concepts:
// - [Key] = Primary key
// - [Required] = Column cannot be NULL
// - [MaxLength] = String length limit
// - [Column(TypeName)] = Specific SQL type
// - Default values are set in constructor

// ============================================================
// SESSION - Represents one broadcast event
// ============================================================

public class Session
{
    // Primary Key - Format: "BoysBasketball_2025-10-28_143022"
    [Key] [Required] [MaxLength(100)] public string SessionId { get; set; } = string.Empty;

    //Sport name: "Boys Basketball, Football, Etc.
    [Required] [MaxLength(50)] public string Sport { get; set; } = string.Empty;

    //When session started (UTC Time)
    [Required] public DateTime StartedUtc { get; set; }

    //When session ended (NULL if still Active)
    public DateTime? StoppedUtc { get; set; }

    //Optional notes: "Game vs Central High - Senior Night"
    public string? Notes { get; set; }

    //Active Teams as JSON: ["A","B"] or ["A","B","C","D"]
    public string? ActiveTeams { get; set; }

    // Constructor with default values
    public Session()
    {
        SessionId = string.Empty;
        Sport = string.Empty;
        StartedUtc = DateTime.UtcNow;
    }
}

// ============================================================
// SETTING - Key-value configuration storage
// ============================================================
public class Setting
{
    //primary Key - Examples:
    // "BoysBasketball_SheetURL", "VmixHost", "PollIntervalSec"
    [Key] [Required] [MaxLength(100)] public string Key { get; set; } = string.Empty;

    // The actual settings value (can be URL, number, JSON, etc.)
    [Required] public string Value { get; set; } = string.Empty;

    // Category for grouping: GoogleSheets", "Vmix", "system"
    [MaxLength(50)] public string? Category { get; set; }

    // When this setting was last changed
    public DateTime LastUpdatedUtc { get; set; }

    //Constructor
    public Setting()
    {
        Key = String.Empty;
        Value = string.Empty;
        LastUpdatedUtc = DateTime.UtcNow;
    }
}

// ============================================================
// ROSTER - Player information from Google Sheets
// ============================================================
public class Roster
{
    //Primary Key - Format: "A001", "B023", "C015"
    [Key] [Required] [MaxLength(10)] public string DisplayId { get; set; } = string.Empty;

    //Team Code: "A", "B", "C", "D"
    [Required] [MaxLength(1)] public string TeamCode { get; set; } = string.Empty;

    //Jersey Number (1-999)
    [Required] public int Number { get; set; }

    //Player Names
    [MaxLength(50)] public string? FirstName { get; set; }

    [MaxLength(50)] public string? LastName { get; set; }

    //Positions (From Google Sheets)
    [MaxLength(20)] public string? POST_OF { get; set; } //Position Offense

    [MaxLength(20)] public string? POST_DEF { get; set; } //Position Defense

    //GRADE: "FR", "SO", "JR", "SR"
    [MaxLength(10)] public string Grade { get; set; }

    // Generic info fields (from Google Sheets)
    public string? Info1 { get; set; }
    public string? Info2 { get; set; }
    public string? Info3 { get; set; }
    public string? Info4 { get; set; }

    // Generic stat fields (season totals from Sheets)
    public string? Stat1 { get; set; }
    public string? Stat2 { get; set; }
    public string? Stat3 { get; set; }
    public string? Stat4 { get; set; }
    public string? Stat5 { get; set; }
    public string? Stat6 { get; set; }

    // Physical Attributes
    [MaxLength(10)] public string? HT { get; set; } // Height: "6'2"\" or "6'2\""

    [MaxLength(10)] public string? WT { get; set; } // Weight: "180 lbs" or "180 lbs"

    //Is this player Currently in the roster? (false if remobed from sheet)
    public bool IsActive { get; set; } = true;

    //When this row was last updated from Google Sheets
    public DateTime LastUpdatedUtc { get; set; }

    //Constructor
    public Roster()
    {
        DisplayId = string.Empty;
        TeamCode = string.Empty;
        LastUpdatedUtc = DateTime.UtcNow;
    }

    // Helper Property: Full Name
    [NotMapped] //Don't Create a Column for this
    public string FullName => $"{FirstName} {LastName}".Trim();
}

// ============================================================
// SHEET SOURCE - Tracks where roster data came from
// ============================================================
public class SheetSource
{
    //Primary Key (same as roster.DisplayId)
    [Key] [Required] [MaxLength(10)] public string DisplayId { get; set; } = string.Empty;

    //Which Google Sheet Tab: "HomeTeamRosterGFX", "AwayTeamRosterGFX"
    [Required] [MaxLength(50)] public string SheetName { get; set; } = string.Empty;

    //Row Number in Google Sheet (1-based, Including Header)
    [Required] public int RowNumber { get; set; }

    //SHA256 Hash of row content to (For Change Detection)
    [Required] [MaxLength(64)] public string RowHash { get; set; } = string.Empty;

    //When this row was last Imported
    public DateTime LastImportedUtc { get; set; }

    //Constructor
    public SheetSource()
    {
        DisplayId = string.Empty;
        SheetName = string.Empty;
        RowHash = string.Empty;
        LastImportedUtc = DateTime.UtcNow;
    }
}

// ============================================================
// IMPORT LOG - Audit trail for Google Sheets imports
// ============================================================
public class ImportLog
{
    // Primary Key (auto-increment)
    [Key] public int Id { get; set; }

    // Log level: "Info", "Warn", "Error"
    [Required] [MaxLength(10)] public string Level { get; set; } = string.Empty;

    // Log message: "Imported 12 players from HomeTeamRosterGFX"
    [Required] public string Message { get; set; } = string.Empty;

    // Additional structured data as JSON
    public string? ContextJSON { get; set; }

    // When this log entry was created
    [Required] public DateTime CreatedUtc { get; set; }

    // Constructor
    public ImportLog()
    {
        Level = string.Empty;
        Message = string.Empty;
        CreatedUtc = DateTime.UtcNow;
    }
}

// ============================================================
// LIVE STATE - Current scoreboard/game state
// ============================================================
public class LiveState
{
    // Primary Key (references Session.SessionId)
    [Key] [Required] [MaxLength(100)] public string SessionId { get; set; } = string.Empty;

    // ===== Universal Fields (all sports) =====
    public int HomeScore { get; set; } = 0;
    public int AwayScore { get; set; } = 0;

    [MaxLength(10)] public string? PeriodOrQuarter { get; set; } // "Q1", "3rd", "Set 1"

    [MaxLength(10)] public string? ClockTime { get; set; } // "12:34"

    public bool ClockRunning { get; set; } = false;

    // ===== Football-Specific Fields =====
    [MaxLength(1)] public string? Possession { get; set; } // "A" or "B"

    public int? Down { get; set; } // 1, 2, 3, 4

    [MaxLength(10)] public string? Distance { get; set; } // "10", "Goal", "Inches"

    public int HomeTO { get; set; } = 0; // Timeouts remaining
    public int AwayTO { get; set; } = 0;

    public bool Flag { get; set; } = false; // Flag on the play

    // ===== Volleyball-Specific Fields =====
    public int HomeSets { get; set; } = 0;
    public int AwaySets { get; set; } = 0;
    public int CurrentSet { get; set; } = 1;

    // When this state was last updated
    [Required] public DateTime LastUpdatedUtc { get; set; }

    // Constructor
    public LiveState()
    {
        SessionId = string.Empty;
        LastUpdatedUtc = DateTime.UtcNow;
    }
}

// ============================================================
// VMIX STATUS - vMix connection status (singleton table)
// ============================================================
public class VmixStatus
{
    // Primary Key (always 1 - only one row exists)
    [Key] public int Id { get; set; } = 1;

    // vMix host: "192.168.1.100"
    [Required] [MaxLength(50)] public string Host { get; set; } = string.Empty;

    // vMix port: 8099 (default)
    public int Port { get; set; } = 8099;

    // Is currently connected?
    public bool Connected { get; set; } = false;

    // Last round-trip time in milliseconds
    public int? LastRttMs { get; set; }

    // When we last checked the connection
    [Required] public DateTime LastCheckedUtc { get; set; }

    // Last error message (if any)
    public string? LastError { get; set; }

    // Constructor
    public VmixStatus()
    {
        Host = "192.168.1.100";
        LastCheckedUtc = DateTime.UtcNow;
    }
}

// ============================================================
// SYSTEM STATUS - General system health (singleton table)
// ============================================================
public class SystemStatus
{
    // Primary Key (always 1 - only one row exists)
    [Key] public int Id { get; set; } = 1;

    // Is Google Sheets API responding?
    public bool GoogleOk { get; set; } = false;

    // Poll interval in seconds
    public int PollIntervalSec { get; set; } = 7;

    // Is network connectivity OK?
    public bool NetworkOk { get; set; } = false;

    // Last check timestamps
    public DateTime? LastGoogleCheckUtc { get; set; }
    public DateTime? LastNetworkCheckUtc { get; set; }
}

// ============================================================
// ACTION - Operator action log with undo capability
// ============================================================
public class Action
{
    // Primary Key (auto-increment)
    [Key] public int Id { get; set; }

    // Which session this action belongs to
    [Required] [MaxLength(100)] public string SessionId { get; set; } = string.Empty;

    // When the action occurred
    [Required] public DateTime WhenUtc { get; set; }

    // Who performed the action: "admin", "operator"
    [MaxLength(50)] public string? User { get; set; }

    // Type of action: "ScoreChange", "ClockStart", "VmixTrigger"
    [Required] [MaxLength(50)] public string ActionType { get; set; } = string.Empty;

    // Current state after action (JSON)
    [Required] public string PayloadJSON { get; set; } = string.Empty;

    // Previous state for undo (JSON)
    public string? UndoPayloadJSON { get; set; }

    // Constructor
    public Action()
    {
        SessionId = string.Empty;
        ActionType = string.Empty;
        PayloadJSON = string.Empty;
        WhenUtc = DateTime.UtcNow;
    }
}

// ============================================================
// HELPER: How to generate DisplayId
// ============================================================
// Example utility class (create this in a separate file later):
//
// public static class DisplayIdHelper
// {
//     public static string Generate(string teamCode, int number)
//     {
//         // "A" + 23 → "A023"
//         return $"{teamCode}{number:D3}"; // D3 = zero-pad to 3 digits
//     }
// }