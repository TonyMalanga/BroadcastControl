using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace BroadcastControl.Data.Entities;

// ============================================================
// PER-SPORT STATS ENTITIES
// ============================================================
// Each sport gets its own table with sport-specific statistics.
// All use composite primary key: (SessionId, DisplayId)
//
// Pattern:
// 1. No [Key] attribute on individual properties
// 2. Composite key is defined in AppDbContext.OnModelCreating()
// 3. All stat fields default to 0
// 4. Foreign keys to Session and Roster are configured in DbContext

// ============================================================
// BASKETBALL STATS
// ============================================================
public class BasketballStats
{
  // Part of composite primary key
  [Required] [MaxLength(100)] public string SessionId { get; set; } = string.Empty;

  // Part of composite primary key
  [Required] [MaxLength(10)] public string DisplayId { get; set; } = string.Empty;

  // ===== Scoring =====
  public int Points { get; set; } = 0;
  public int Assists { get; set; } = 0;

  // ===== Rebounds =====
  public int ReboundsOff { get; set; } = 0;
  public int ReboundsDef { get; set; } = 0;

  // ===== Defense =====
  public int Steals { get; set; } = 0;
  public int Blocks { get; set; } = 0;

  // ===== Turnovers & Fouls =====
  public int Turnovers { get; set; } = 0;
  public int Fouls { get; set; } = 0;

  // ===== Shooting Stats =====
  public int TwoPM { get; set; } = 0; // Two Pointers Made
  public int TwoPA { get; set; } = 0; // Two Pointers Attempted

  public int ThreePM { get; set; } = 0; // 3-Pointers Made
  public int ThreePA { get; set; } = 0; // 3-Pointers Attempted

  public int FTM { get; set; } = 0; // Free Throws Made
  public int FTA { get; set; } = 0; // Free Throws Attempted

  // ===== Computed Properties =====

  [NotMapped]
  public double FGPrecentage => (TwoPA + ThreePA) > 0 ? (double)(TwoPM + ThreePM) / (TwoPA + ThreePA) * 100 : 0;
  
  [NotMapped] public int TotalRebounds => ReboundsOff + ReboundsDef;
  
  [NotMapped] public double TwoPointPercentage => TwoPA > 0 ? (double)TwoPM / TwoPA * 100 : 0;

  [NotMapped] public double ThreePointPercentage => ThreePA > 0 ? (double)ThreePM / ThreePA * 100 : 0;

  [NotMapped] public double FTPercentage => FTA > 0 ? (double)FTM / FTA * 100 : 0;
}

// ============================================================
// FOOTBALL STATS
// ============================================================
public class FootballStats
{
  // Part of composite primary key
  [Required] [MaxLength(100)] public string SessionId { get; set; } = string.Empty;

  // Part of composite primary key
  [Required] [MaxLength(10)] public string DisplayId { get; set; } = string.Empty;

  // ===== Passing =====
  public int PassYds { get; set; } = 0; // Passing yards
  public int PassTD { get; set; } = 0; // Passing touchdowns
  public int PassINT { get; set; } = 0; // Interceptions thrown
  public int PassComp { get; set; } = 0; // Completions
  public int PassAtt { get; set; } = 0; // Attempts

  // ===== Rushing =====
  public int RushYds { get; set; } = 0; // Rushing yards
  public int RushTD { get; set; } = 0; // Rushing touchdowns
  public int RushAtt { get; set; } = 0; // Rushing attempts

  // ===== Receiving =====
  public int RecYds { get; set; } = 0; // Receiving yards
  public int RecTD { get; set; } = 0; // Receiving touchdowns
  public int Receptions { get; set; } = 0; // Receptions

  // ===== Turnovers =====
  public int Fumbles { get; set; } = 0;
  public int FumblesLost { get; set; } = 0;

  // ===== Defense =====
  public int Tackles { get; set; } = 0;
  public int TFL { get; set; } = 0; //Tackle for Loss
  public int Sacks { get; set; } = 0;
  public int INT { get; set; } = 0; //Interceptions
  public int PassDeflections { get; set; } = 0;
  public int ForcedFumbles { get; set; } = 0;
  public int FR { get; set; } = 0; //Fumble Recoveries  

  // ===== Special Teams =====
  public int KRYds { get; set; } = 0; //kick return yards
  public int KRTD { get; set; } = 0; //kick return touchdowns
  public int PRYds { get; set; } = 0; //punt return yards
  public int PRTD { get; set; } = 0; //punt return touchdowns

  // ===== Kicking =====
  public int FGM { get; set; } = 0; // Field Goals Made
  public int FGA { get; set; } = 0; // Field Goals Attempted
  public int XPM { get; set; } = 0; // Extra Points Made
  public int XPA { get; set; } = 0; // Extra Points Attempted


  //===== Punting =====
  public int Punts { get; set; } = 0;
  public int PuntYds { get; set; } = 0;

  // ===== Computed Properties =====
  [NotMapped]
  public double PasserRating
  {

    get
    {
      if (PassAtt == 0) return 0;
      //Simplified Passer Rating Calculation
      double compPct = (double)PassComp / PassAtt * 100;
      double ydsPerAtt = (double)PassYds / PassAtt;
      double tdPct = (double)PassTD / PassAtt * 100;
      double intPct = (double)PassINT / PassAtt * 100;
      return (comPct + ydsPerAtt + tdPct - intPct) / 4;


    }
  }

  [NotMapped] public double RushAvg => RushAtt > 0 ? (double)RushYds / RushAtt : 0;

  [NotMapped] public double RecAvg => Receptions > 0 ? (double)RecYds / Receptions : 0;

  [NotMapped] public double FGPercentage => FGA > 0 ? (double)FGM / FGA * 100 : 0;
}

// ============================================================
// VOLLEYBALL STATS
// ============================================================
public class VolleyballStats
{
    // Composite primary key components
    [Required]
    [MaxLength(100)]
    public string SessionId { get; set; } = string.Empty;

    [Required]
    [MaxLength(10)]
    public string DisplayId { get; set; } = string.Empty;

    // ===== Attacking =====
    public int Kills { get; set; } = 0;
    public int Attempts { get; set; } = 0;      // Attack attempts
    public int HittingErrors { get; set; } = 0;

    // ===== Setting =====
    public int Assists { get; set; } = 0;       // Sets that led to kills

    // ===== Serving =====
    public int Aces { get; set; } = 0;
    public int ServiceErrors { get; set; } = 0;

    // ===== Defense =====
    public int Digs { get; set; } = 0;

    // ===== Blocking =====
    public int BlocksSolo { get; set; } = 0;
    public int BlocksAssist { get; set; } = 0;
    public int BlockingErrors { get; set; } = 0;

    // ===== Passing/Receiving =====
    public int Receptions { get; set; } = 0;    // Successful passes
    public int ReceptionErrors { get; set; } = 0;

    // ===== Computed Properties =====
    [NotMapped]
    public double HittingPercentage
    {
        get
        {
            if (Attempts == 0) return 0;
            return (double)(Kills - HittingErrors) / Attempts;
        }
    }

    [NotMapped]
    public int TotalBlocks => BlocksSolo + BlocksAssist;

    [NotMapped]
    public double PassEfficiency
    {
        get
        {
            int total = Receptions + ReceptionErrors;
            if (total == 0) return 0;
            return (double)Receptions / total * 100;
        }
    }
}

// ============================================================
// SOCCER STATS
// ============================================================
public class SoccerStats
{
    // Composite primary key components
    [Required] [MaxLength(100)] public string SessionId { get; set; } = string.Empty;

    [Required] [MaxLength(10)] public string DisplayId { get; set; } = string.Empty;

    // ===== Offense =====
    public int Goals { get; set; } = 0;
    public int Assists { get; set; } = 0;
    public int Shots { get; set; } = 0;
    public int ShotsOnTarget { get; set; } = 0;
    public int KeyPasses { get; set; } = 0; // Passes leading to shots

    // ===== Defense =====
    public int Tackles { get; set; } = 0;
    public int Interceptions { get; set; } = 0;
    public int Clearances { get; set; } = 0;

    // ===== Fouls & Cards =====
    public int FoulsCommitted { get; set; } = 0;
    public int FoulsDrawn { get; set; } = 0;
    public int YellowCards { get; set; } = 0;
    public int RedCards { get; set; } = 0;

    // ===== Goalkeeper Stats =====
    public int Saves { get; set; } = 0;
    public int GoalsAgainst { get; set; } = 0;

    // ===== Computed Properties =====
    [NotMapped] public double ShotAccuracy => Shots > 0 ? (double)ShotsOnTarget / Shots * 100 : 0;

    [NotMapped]
    public double SavePercentage
    {
        get
        {
            int totalShots = Saves + GoalsAgainst;
            return totalShots > 0 ? (double)Saves / totalShots * 100 : 0;
        }
    }
}

// ============================================================
// TENNIS STATS
// ============================================================
public class TennisStats
{
    // Composite primary key components
    [Required] [MaxLength(100)] public string SessionId { get; set; } = string.Empty;
    [Required] [MaxLength(10)] public string DisplayId { get; set; } = string.Empty;

    // ===== Serving =====
    public int Aces { get; set; } = 0;
    public int DoubleFaults { get; set; } = 0;
    public int FirstServesIn { get; set; } = 0;
    public int FirstServesAttempted { get; set; } = 0;
    public int SecondServesIn { get; set; } = 0;
    public int SecondServesAttempted { get; set; } = 0;
    public int ServiceGamesWon { get; set; } = 0;
    public int ServiceGamesPlayed { get; set; } = 0;

    // ===== Returning =====
    public int BreakPointsWon { get; set; } = 0;
    public int BreakPointsOpportunities { get; set; } = 0;
    public int ReturnGamesWon { get; set; } = 0;
    public int ReturnGamesPlayed { get; set; } = 0;

    // ===== Shot Quality =====
    public int Winners { get; set; } = 0;
    public int UnforcedErrors { get; set; } = 0;
    public int ForcedErrors { get; set; } = 0;

    // ===== Match Stats =====
    public int TotalPointWon { get; set; } = 0;
    public int TotalPointPlayed { get; set; } = 0;
    public int GamesWon { get; set; } = 0;
    public int SetWon { get; set; } = 0;

    // ===== Match Results =====
    [MaxLength(20)] public string? MatchResult { get; set; } // "Won", "Lost", "Retired"

    // ===== Computed Properties =====
    [NotMapped]
    public double FirstServePercentage =>
        FirstServesAttempted > 0 ? (double)FirstServesIn / FirstServesAttempted * 100 : 0;

    [NotMapped]
    public double BreakPointConversion =>
        BreakPointsOpportunities > 0 ? (double)BreakPointsWon / BreakPointsOpportunities * 100 : 0;

    [NotMapped] public int WinnerToErrorRatio => UnforcedErrors > 0 ? Winners / UnforcedErrors : 0;
}


// ============================================================
// SWIMMING STATS 
// ============================================================
public class SwimmingStats
{
    // Composite primary key components
    [Required] [MaxLength(100)] public string SessionId { get; set; } = string.Empty;
    [Required] [MaxLength(10)] public string DisplayId { get; set; } = string.Empty;
    
    // ====== Event Info ======
    [MaxLength(50)] 
    public string? EventName { get; set;  } // "100m Freestyle"", "200m IM" ETC.

    [MaxLength(20)]
    public string? EventType { get; set; } // "Swiming or Diving"
    
    // ===== Swimming Times (Stored as milliseconds) =====
    
    public int? ReactionTimeMs { get; set;  }
    public int? FinalTimeMs { get; set; } //Final Race time in Milliseconds
    public int? PersonalBestMs { get; set; }
    
    //Split Times stored as JSON string "[ 28500, 52900, 91300]"
    public string? SplitTimesJson { get; set; }
    
    // ===== Meet Scoring =====
    public int MeetPoints { get; set; } = 0;
    public int? Placement { get; set; } //1st, 2nd, 3rd, etc.
    
    // ===== Diving Specific =====
    public double? DegreeOfDifficulty { get; set; }
    public string? JudgesScoresJSON { get; set; } // "[8.5, 9.0 8.5, 9.0 8.0]" 
    public double TotalDiveScore { get; set; }
    
    // ===== Computed Properties =====
    [NotMapped]
    public string FinalTimeFormatted
    {
        get
        {
            if (!FinalTimeMs.HasValue) return "--:--:--";
            var ts = TimeSpan.FromMilliseconds(FinalTimeMs.Value);
            return ts.ToString(@"mm\:ss");
        }
    }
    [NotMapped]
    public bool IsPersonalBest => FinalTimeMs.HasValue && FinalTimeMs.HasValue && FinalTimeMs <= PersonalBestMs;
}
// ============================================================
// Track & Field Stats
// ============================================================
public class TrackFieldStats
{
    // Composite primary key components
    [Required] [MaxLength(100)] public string SessionId { get; set; } = string.Empty;
    [Required] [MaxLength(10)] public string DisplayId { get; set; } = string.Empty;

    // ===== Event Info =====
    [MaxLength(50)] public string? EventName { get; set; } // "100m Dash", "Long Jump", "Shot Put"

    [MaxLength(20)] public string? EventCategory { get; set; } // "Sprint", "Distance", "Field", "Jump", "Throw"

    // ===== Timing (for running events) - stored as Milliseconds =====
    public int? ReactionTimeMs { get; set; }
    public int? FinalTimeMs { get; set; }
    public string? SplitTimesJson { get; set; } // "[ 28500, 52900, 91300]"

    // ===== Field Events (Distance/Height in centimeters ===== 
    public int? BestDistanceCm { get; set; } // For throws and Horizontal Jumps
    public int? BestHeightCm { get; set; } // For  High Jump and Pole Vault

    // individual attemps stored as JSON: "[1825,1910, 1875]"
    public string? AttemptsJSON { get; set; }
    public int Fouls { get; set; } = 0;

    //===== Meet Info =====
    public int? Placement { get; set; } // 1st, 2nd, 3rd
    public int MeetPoints { get; set; } = 0;
    public double? WindSpeed { get; set; } // m/s (for Sprints/jumps)

    // ===== Personal Records =====
    public int? PersonalBestMs { get; set; } // for Running Events
    public int? PersonalBestCm { get; set; } // for Field Events

    // ===== Computed Properties =====
    [NotMapped]
    public string BestDistanceFormatted
    {
        get
        {
            if (!BestDistanceCm.HasValue) return "-";
            double meters = BestDistanceCm.Value / 100;
            return $"{meters:F2}m";
        }
    }

    [NotMapped]
    public string BestHeightFormatted
    {
        get
        {
            if (!BestHeightCm.HasValue) return "-";
            double meters = BestDistanceCm.Value / 100.0;
            return $"{meters:F2}m";
        }
    }

    [NotMapped]
    public bool IsPersonalBest
    {
        get
        {
            if (FinalTimeMs.HasValue && PersonalBestMs.HasValue)
                return FinalTimeMs.Value <= PersonalBestMs.Value;
            if (BestDistanceCm.HasValue && PersonalBestCm.HasValue)
                return BestDistanceCm.Value >= PersonalBestCm.Value;
            if (BestHeightCm.HasValue && PersonalBestCm.HasValue)
                return BestHeightCm.Value >= PersonalBestCm.Value;
            return false;
        }
    }
}

// ============================================================
// Track & Field HS STATS
// ============================================================
public class TrackFieldStatsHS
{
    // Composite primary key components
    [Required] [MaxLength(100)] public string SessionId { get; set; } = string.Empty;
    [Required] [MaxLength(10)] public string DisplayId { get; set; } = string.Empty;

    // ===== Event Info =====
    [MaxLength(50)] public string? EventName { get; set; } // "100m Dash", "Long Jump", "Shot Put"

    [MaxLength(20)] public string? EventCategory { get; set; } // "Sprint", "Distance", "Field", "Jump", "Throw"

    // ===== Timing (for running events) - stored as Milliseconds =====
    public int? ReactionTimeMs { get; set; }
    public int? FinalTimeMs { get; set; }
    public string? SplitTimesJson { get; set; } // "[ 28500, 52900, 91300]"

    // ===== Field Events (Distance/Height in centimeters ===== 
    public double? BestDistanceInches { get; set; } // For throws and Horizontal Jumps
    public double? BestHeightInches { get; set; } // For  High Jump and Pole Vault

    // individual attemps stored as JSON: "[1825,1910, 1875]"
    public string? AttemptsJSON { get; set; }
    public int Fouls { get; set; } = 0;

    //===== Meet Info =====
    public int? Placement { get; set; } // 1st, 2nd, 3rd
    public int MeetPoints { get; set; } = 0;
    public double? WindSpeed { get; set; } // m/s (for Sprints/jumps)

    // ===== Personal Records =====
    public int? PersonalBestMs { get; set; } // for Running Events
    public double? PersonalBestInches { get; set; } // for Field Events

    // ===== Computed Properties =====
    [NotMapped]
    public string BestDistanceFormatted
    {
        get
        {
            if (!BestDistanceInches.HasValue) return "—";
        
            int feet = (int)(BestDistanceInches.Value / 12);        // Integer division
            double inches = BestDistanceInches.Value % 12;          // Remainder (modulo)
        
            return $"{feet}'-{inches:F2}\"";  // e.g., "6'-2.00""
        }
    }

    [NotMapped]
    public string BestHeightFormatted
    {
        get
        {
            if (!BestHeightInches.HasValue) return "—";
        
            int feet = (int)(BestHeightInches.Value / 12);        // Integer division
            double inches = BestHeightInches.Value % 12;          // Remainder (modulo)
        
            return $"{feet}'-{inches:F2}\"";  // e.g., "6'-2.00""
        }
    } 
    
    [NotMapped]
    public bool IsPersonalBest
    {
        get
        {
            if (FinalTimeMs.HasValue && PersonalBestMs.HasValue)
                return FinalTimeMs.Value <= PersonalBestMs.Value;
            if (BestDistanceInches.HasValue && PersonalBestInches.HasValue)
                return BestDistanceInches.Value >= PersonalBestInches.Value;
            if (BestHeightInches.HasValue && PersonalBestInches.HasValue)
                return BestHeightInches.Value >= PersonalBestInches.Value;
            return false;
        }
    }
}

// ============================================================
// WRESTLING STATS
// ============================================================
public class WrestlingStats
{
    // Composite primary key components
    [Required] [MaxLength(100)] public string SessionId { get; set; } = string.Empty;
    [Required] [MaxLength(10)] public string DisplayId { get; set; } = string.Empty;

    //Match Info
    [MaxLength(20)] public string? WeightClass { get; set; } //"120 lbs", "152 lbs", "Heavyweight"

    [MaxLength(20)] public string? MatchResult { get; set; } //"Win", "Loss", "Draw"

    [MaxLength(30)] public string? VictoryMethod { get; set; } //"Pin", "Tech Fall", "Major Decision", "Decision"

    public int? MatchTimeSeconds { get; set; } // Time when match ended

    // ===== Scoring Actions =====
    public int Takedowns { get; set; } = 0; // 2 Points Each
    public int Escapes { get; set; } = 0; // 1 Point Each
    public int Reversals { get; set; } = 0; // 2 Points Each
    public int NearFalls2 { get; set; } = 0; // 2-point NearFalls
    public int NearFalls3 { get; set; } = 0; // 3-point NearFalls
    public int Penalties { get; set; } = 0; // Points Awarded to opponent

    // ===== Riding Time =====
    public int RidingTimeSeconds { get; set; } = 0; // Total time in control
    public bool RidingTimePointAwarded { get; set; } = false; // > 1 minute advantage

    // ===== Team Scoring =====
    public int TeamPointsEarned { get; set; } = 0; // 6 for pin, 5 for tech fall, etc.

    // ===== Season Totals =====
    public int SeasonWins { get; set; } = 0;
    public int SeasonLosses { get; set; } = 0;
    public int SeasonPins { get; set; } = 0;

    // ===== Computed Properties =====
    [NotMapped]
    public int TotalPointsScored => (Takedowns * 2) + (Escapes * 1) + (Reversals * 2) + (NearFalls2 * 2) +
                                    (NearFalls3 * 3) + (RidingTimePointAwarded ? 1 : 0);

    [NotMapped]
    public string RidingTimeFormatted
    {
        get
        {
            var ts = TimeSpan.FromSeconds(RidingTimeSeconds);
            return $"{ts.Minutes:D2}:{ts.Seconds:D2}";
        }
    }

    [NotMapped]
    public double WinPercentage =>
        SeasonWins + SeasonLosses > 0 ? (double)SeasonWins / (SeasonWins + SeasonLosses) * 100 : 0;
}

// ============================================================
// CROSS COUNTRY STATS
// ============================================================
public class CrossCountryStats
{
    // Composite primary key components
    [Required] [MaxLength(100)] public string SessionId { get; set; } = string.Empty;
    [Required] [MaxLength(10)] public string DisplayId { get; set; } = string.Empty;
    
    // ===== Race Info =====
    [MaxLength(50)] public string? RaceName { get; set; } // Conference Championship, Regionals
     public double? RaceDistanceKm { get; set; } //5.0, 8.0 etc.
     
     // ===== Timing (stored as milliseconds) =====
     public int? FinalTimeMs { get; set; }
     public int? PersonalBestMs { get; set; }
     
    // Split times at mile markers stored as JSON string "[ 28500, 52900, 91300]"
    public string? SplitTimesJson { get; set; }
    
    // ===== Placement =====
    public int? FinishPostion {get; set; } // 1, 2, 3, etc.
    public int? TotalRunners { get; set; } // How many finished the race
    
    // ===== Team Scoring =====
    public int TeamPoints { get; set; } // Top 5 runners score
    
    // ===== Season Stats =====
    public int RacesRun { get; set; } = 0; 
    public int Top10Finishes { get; set; } = 0;
    
    // ===== Computed Properties =====
    [NotMapped]
    public string FinalTimeFormatted
    {
        get
        {
            if (!FinalTimeMs.HasValue) return "--:--:--";
            var ts = TimeSpan.FromMilliseconds(FinalTimeMs.Value);
            return $"{ts.Seconds:D2}.{ts.Milliseconds / 10:D2}";
        }
    }

    [NotMapped]
    public string AveragePaceFormatted
    {
        get
        {
            if (!FinalTimeMs.HasValue || !RaceDistanceKm.HasValue || RaceDistanceKm.Value == 0)
                return "--:--";
            
            // Calculate pace per kilometer
            double totalMinutes = FinalTimeMs.Value / 60000.0;
            double pacePerKm = totalMinutes / RaceDistanceKm.Value;
            int minutes = (int)pacePerKm;
            int seconds = (int)((pacePerKm - minutes) * 60);
            return $"{minutes}:{seconds:D2}/km";
        }
    }
    [NotMapped]
    public bool IsPersonalBest => FinalTimeMs.HasValue && FinalTimeMs.HasValue && FinalTimeMs <= PersonalBestMs;
    
    [NotMapped]
    public string PlacementFormatted => FinishPostion.HasValue ? $"{FinishPostion}/{TotalRunners}" : "-";
    
}

// ============================================================
// GOLF STATS
// ============================================================
public class GolfStats
{
    // Composite primary key components
    [Required] [MaxLength(100)] public string SessionId { get; set; } = string.Empty;
    [Required] [MaxLength(10)] public string DisplayId { get; set; } = string.Empty;

    // ===== Round Info =====
    [MaxLength(50)] public string? CourseName { get; set; }
    public int? CoursePar { get; set; }
    public int? RoundScore { get; set; } // total Strokes

    //  ===== Score Breakdown =====
    public int Eagles { get; set; } = 0; //-2
    public int Birdies { get; set; } = 0; // -1
    public int Pars { get; set; } = 0; // Even
    public int Bogeys { get; set; } = 0; // +1
    public int DoubleBogeys { get; set; } = 0; // +2
    public int TripleBogeysorWorse { get; set; } = 0; // +3 or More

    // ===== Accuracy Stats =====
    public int FairwaysHit { get; set; } = 0;
    public int FairwaysTotal { get; set; } = 0;
    public int GreensInRegulation { get; set; } = 0; // GIR
    public int GreensInRegulationTotal { get; set; } = 0;

    // ===== Putting =====
    public int TotalPutts { get; set; } = 0;
    public int OnePutts { get; set; } = 0;
    public int TwoPutts { get; set; } = 0;
    public int ThreePutts { get; set; } = 0;

    // ===== Sand Play ===== 
    public int SandSaves { get; set; } = 0;
    public int SandSaveOpportunities { get; set; } = 0;

    // ===== Driving =====
    public int AverageDriveYards { get; set; } = 0;
    public int LongestDriveYards { get; set; } = 0;

    // ===== Handicap & Scoring =====
    public double Handicap { get; set; } = 0;
    public int NetScore { get; set; } = 0; // Score - Handicap

    // ===== Tournament Placement =====
    public int? Placement { get; set; }
    public int? TotalPlayers { get; set; }

    // ===== Computed Properties =====
    [NotMapped]
    public int? ScoreToPar => RoundScore.HasValue && CoursePar.HasValue ? RoundScore.Value - CoursePar.Value : null;

    [NotMapped] public double FairwayAccuracy => FairwaysTotal > 0 ? (double)FairwaysHit / FairwaysTotal * 100 : 0;

    [NotMapped]
    public double GIRPercentage =>
        GreensInRegulationTotal > 0 ? (double)GreensInRegulation / GreensInRegulationTotal * 100 : 0;

    [NotMapped] public double PuttsPerRound => TotalPutts;

    [NotMapped] public double PuttsPerGIR => GreensInRegulation > 0 ? (double)TotalPutts / GreensInRegulation : 0;

    [NotMapped]
    public double SandSavePercentage => SandSaveOpportunities > 0 ? (double)SandSaves / SandSaveOpportunities * 100 : 0;

}

// ============================================================
// BOWLING STATS
// ============================================================

public class BowlingStats
{
    // Composite Primary Components 
    [Required] [MaxLength(100)] public string SessionId { get; set; } = string.Empty;
    [Required] [MaxLength(10)] public string DisplayId { get; set; } = string.Empty;
    
    // ===== Game/Series Info ====-
    public int Game1Score { get; set; } = 0;
    public int Game2Score { get; set; } = 0;
    public int Game3Score { get; set; } = 0;
    
    // Frame-by-Frame scores stored as JSON: "[10, 20, 39, 58, ...]"
    public string? Game1FramesJSON { get; set; }
    public string? Game2FramesJSON { get; set; }
    public string? Game3FramesJSON { get; set; }
    
    // ===== Performance Metrics =====
    public int Strikes {get; set; } = 0;
    public int Spares {get; set; } = 0;
    public int Splits {get; set; } = 0;
    public int SplitsAttempted { get; set; } = 0;
    public int OpenFrames {get; set; } = 0;
    
    // ===== Pinfall =====
    public int TotalPinfall { get; set; } = 0; 
    
    // ===== Season Stats =====
    public double SeasonAverage { get; set; } = 0;
    public int HighGame {get; set; } = 0;
    public int HighSeries { get; set; } = 0;
    public int GamesPlayed { get; set; } = 0;
    
    // ===== Computed Properties =====
    [NotMapped]
    public int SeriesTotal => Game1Score + Game2Score + Game3Score;

    [NotMapped] public double SeriesAverage => SeriesTotal / 3.0;

    [NotMapped]
    public double StrikePercentage
    {
        get
        {
            int totalFrames = 30; // 10 Frames x 3 games
            return totalFrames > 0 ? (double)Strikes / 100 : 0;
        }
    }

    [NotMapped]
    public double SpareConversionRate
    {
        get
        {
            // Opportunities = Frames that aren't strikes
            int TotalFrames = 30;
            int spareOpportunities = TotalFrames - Strikes;
            return spareOpportunities > 0 ? (double)Spares / spareOpportunities * 100 : 0;
        }
    }

    [NotMapped]
    public double SplitConversionRate => SplitsAttempted > 0 ? (double)Splits / SplitsAttempted * 100 : 0;
}

// ============================================================
// BASEBALL STATS
// ============================================================

public class BaseballStats
{
    // Composite Primary Key Components
    [Required] [MaxLength(100)] public string SessionId { get; set; } = string.Empty;
    [Required] [MaxLength(10)] public string DisplayId { get; set; } = string.Empty;
    
    // ===== BATTING STATS =====
    public int AtBats { get; set; } = 0; // AB
    public int Hits { get; set; } = 0; // H
    public int Singles  { get; set; } = 0; // 1B
    public int Doubles  { get; set; } = 0; // 2B
    public int Triples  { get; set; } = 0; // 3B
    public int HomeRuns { get; set; } = 0; // HR
    public int Runs { get; set; } = 0; // R
    public int RBI { get; set; } = 0; // Runs Batted In
    public int Walks { get; set; } = 0; // BB
    public int Strikeouts  { get; set; } = 0; // K
    public int HitByPitch   { get; set; } = 0; // HBP
    public int SacarificeFlies  { get; set; } = 0; // SF
    public int SacarificeHits{ get; set; } = 0; // SH (bunts)
    public int StolenBases  { get; set; } = 0; // SB
    public int CaughtStealing { get; set; } = 0; // CS
    
    // ===== PITCHING STATS =====
    public int Wins {get; set; } = 0; // W
    public int Losses { get; set; } = 0; // L
    public int Saves  { get; set; } = 0; // SV
    public int GamesStarted { get; set; } = 0; // GS
    public int GamesFinished { get; set; } = 0; // GF
    public int CompleteGames { get; set; } = 0; // CG
    public int Shutouts { get; set; } = 0; // SHO
    
    // Innings Pitched stored as outs (multiply by 3 for innings
    
    public int OutsRecorded {get; set; } = 0; // IP X 3
    
    public int HitsAllowed {get; set; } = 0; // H
    public int RunsAllowed {get; set; } = 0; // R
    public int EarnedRuns { get; set; } = 0; // ER
    public int WalksAllowed {get; set; } = 0; // BB
    public int StrikeoutsPitched  { get; set; } = 0; //K
    public int HitBatsmen { get; set; } = 0; // HBP
    public int WildPitches  { get; set; } = 0; // WP
    public int Balks { get; set; } = 0; 
    
    // ===== FIELDING STATS =====
    public int Putouts { get; set; } = 0; //PO
    public int Assists  { get; set; } = 0; //A
    public int Errors { get; set; } = 0; // E
    public int DoublePlays { get; set; } = 0; // DP
    
    // ===== Computed Properties =====
    [NotMapped] public double BattingAverage => AtBats > 0 ? (double)Hits / AtBats : 0; // AVG

    [NotMapped]
    public double OnBasePercentage
    {
        get
        {
            int totalPlateAppearances = AtBats + Walks + HitByPitch + SacarificeFlies;
            if (totalPlateAppearances == 0) return 0;
            return (double)(Hits + Walks + HitByPitch) /  totalPlateAppearances;
            
        }
    }
    [NotMapped]
    public double StolenBasesPercentage
    {
        get
        {
            int attempts = StolenBases + CaughtStealing;
            return attempts > 0 ? (double)StolenBases / attempts * 100 : 0;
        }
    }
    // Pitching 
    [NotMapped] public double InningsPitched => OutsRecorded / 3.0;
    
    [NotMapped]
    public double ERA
    {
        get
        {
            if (OutsRecorded == 0) return 0;
            return (EarnedRuns * 27.0) / OutsRecorded; // 27 outs = 9 innings
        }
    }

    [NotMapped]
    public double StrikeoutsPerNine
    {
        get
        {
            if (OutsRecorded == 0) return 0;
            return (StrikeoutsPitched*27.0) / OutsRecorded;
        }
    }
    
    [NotMapped]
    public double WalksPerNine
    {
        get
        {
            if (OutsRecorded == 0) return 0;
            return (WalksAllowed*27.0) / OutsRecorded;
        }
    }
    
    // FIELDING
    [NotMapped] public int TotalChances => Putouts + Assists + Errors;
    
    [NotMapped] double FieldingPercentage => TotalChances > 0 ? (double)(Putouts + Assists) / TotalChances  : 0;
}

// ============================================================
// SOFTBALL STATS
// ============================================================

public class SoftballStats
{
    // Composite Primary Key Components
    [Required] [MaxLength(100)] public string SessionId { get; set; } = string.Empty;
    [Required] [MaxLength(10)] public string DisplayId { get; set; } = string.Empty;

    // ===== BATTING STATS =====
    public int AtBats { get; set; } = 0; // AB
    public int Hits { get; set; } = 0; // H
    public int Singles { get; set; } = 0; // 1B
    public int Doubles { get; set; } = 0; // 2B
    public int Triples { get; set; } = 0; // 3B
    public int HomeRuns { get; set; } = 0; // HR
    public int Runs { get; set; } = 0; // R
    public int RBI { get; set; } = 0; // Runs Batted In
    public int Walks { get; set; } = 0; // BB
    public int Strikeouts { get; set; } = 0; // K
    public int HitByPitch { get; set; } = 0; // HBP
    public int SacarificeFlies { get; set; } = 0; // SF
    public int SacarificeHits { get; set; } = 0; // SH (bunts)
    public int StolenBases { get; set; } = 0; // SB
    public int CaughtStealing { get; set; } = 0; // CS

    // ===== PITCHING STATS =====
    public int Wins { get; set; } = 0; // W
    public int Losses { get; set; } = 0; // L
    public int Saves { get; set; } = 0; // SV
    public int GamesStarted { get; set; } = 0; // GS
    public int GamesFinished { get; set; } = 0; // GF
    public int CompleteGames { get; set; } = 0; // CG
    public int Shutouts { get; set; } = 0; // SHO

    // Innings Pitched stored as outs (multiply by 3 for innings

    public int OutsRecorded { get; set; } = 0; // IP X 3

    public int HitsAllowed { get; set; } = 0; // H
    public int RunsAllowed { get; set; } = 0; // R
    public int EarnedRuns { get; set; } = 0; // ER
    public int WalksAllowed { get; set; } = 0; // BB
    public int StrikeoutsPitched { get; set; } = 0; //K
    public int HitBatsmen { get; set; } = 0; // HBP
    public int WildPitches { get; set; } = 0; // WP
    public int Balks { get; set; } = 0;

    // ===== FIELDING STATS =====
    public int Putouts { get; set; } = 0; //PO
    public int Assists { get; set; } = 0; //A
    public int Errors { get; set; } = 0; // E
    public int DoublePlays { get; set; } = 0; // DP

    // ===== Computed Properties =====
    [NotMapped] public double BattingAverage => AtBats > 0 ? (double)Hits / AtBats : 0; // AVG

    [NotMapped]
    public double OnBasePercentage
    {
        get
        {
            int totalPlateAppearances = AtBats + Walks + HitByPitch + SacarificeFlies;
            if (totalPlateAppearances == 0) return 0;
            return (double)(Hits + Walks + HitByPitch) / totalPlateAppearances;

        }
    }

    [NotMapped]
    public double StolenBasesPercentage
    {
        get
        {
            int attempts = StolenBases + CaughtStealing;
            return attempts > 0 ? (double)StolenBases / attempts * 100 : 0;
        }
    }

    // Pitching 
    [NotMapped] public double InningsPitched => OutsRecorded / 3.0;

    [NotMapped]
    public double ERA
    {
        get
        {
            if (OutsRecorded == 0) return 0;
            return (EarnedRuns * 27.0) / OutsRecorded; // 27 outs = 9 innings
        }
    }

    [NotMapped]
    public double StrikeoutsPerNine
    {
        get
        {
            if (OutsRecorded == 0) return 0;
            return (StrikeoutsPitched * 27.0) / OutsRecorded;
        }
    }

    [NotMapped]
    public double WalksPerNine
    {
        get
        {
            if (OutsRecorded == 0) return 0;
            return (WalksAllowed * 27.0) / OutsRecorded;
        }
    }

    // FIELDING
    [NotMapped] public int TotalChances => Putouts + Assists + Errors;

    [NotMapped] double FieldingPercentage => TotalChances > 0 ? (double)(Putouts + Assists) / TotalChances : 0;
}
