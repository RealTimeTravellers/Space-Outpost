using Godot;

public enum MissionType
{
    Destroy,
    Eliminate,
}

public partial class MissionManager : Node
{
    public static MissionManager Instance { get; private set; }
    public string MissionSuccessBriefing;
    public string MissionFailureBriefing;
    public MissionType MissionType;

    // Combat Statistics
    public int TotalShotsFired { get; private set; } = 0;
    public int TotalHits { get; private set; } = 0;
    public float Accuracy => TotalShotsFired > 0 ? (float)TotalHits / TotalShotsFired * 100 : 0;
    public int TotalHitsMissed => TotalShotsFired - TotalHits;
    public int CriticalHits { get; private set; } = 0;
    
    // Casualty Statistics
    public int EnemiesKilled { get; private set; } = 0;
    public int AlliesLost { get; private set; } = 0;
    public int CiviliansCasualties { get; private set; } = 0;

    // Tactical Statistics
    public int CoverUsed { get; private set; } = 0;
    public int TurnsCompleted { get; private set; } = 0;

    public override void _Ready()
    {
        Instance = this;
        ResetStatistics();
    }

    public void SetMissionDetails(MissionDataCard missionDataCard)
    {
        MissionSuccessBriefing = missionDataCard.missionSuccessLabel;
        MissionFailureBriefing = missionDataCard.missionFailureLabel;
        MissionType = missionDataCard.missionType;
    }

    // Public methods to update statistics
    public void RecordShot(bool hit)
    {
        TotalShotsFired++;
        if (hit) TotalHits++;
    }

    public void RecordEnemyKill() => EnemiesKilled++;
    public void RecordAllyLoss() => AlliesLost++;
    public void RecordCivilianCasualty() => CiviliansCasualties++;
    public void RecordCoverUse() => CoverUsed++;
    public void RecordTurnComplete() => TurnsCompleted++;
    public void RecordCriticalHit() => CriticalHits++;

    // Reset statistics for new mission
    public void ResetStatistics()
    {
        TotalShotsFired = 0;
        TotalHits = 0;
        EnemiesKilled = 0;
        AlliesLost = 0;
        CiviliansCasualties = 0;
        CoverUsed = 0;
        TurnsCompleted = 0;
        CriticalHits = 0;
    }
}
