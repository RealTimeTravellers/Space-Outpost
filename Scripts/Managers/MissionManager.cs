using Godot;
using System.Threading.Tasks;
public enum MissionType
{
    Destroy,
    Eliminate,
}

public partial class MissionManager : Node
{
    public static MissionManager Instance { get; private set; }
    [Export] private PackedScene briefingScene;
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

    private Node currentBriefing;
    public string MissionSuccessBriefing { get; private set; } 
    public string MissionFailureBriefing { get; private set; }
    public string MissionIntroBriefing { get; private set; }

    private string MissionSuccessfulKill = "Commander, the enemy has been eliminated. Continue to the next objective.";
    private string MissionEnemySighted = "Commander, the enemy has been sighted. Marking their position.";
    private string MissionAlliesLost = "Commander, an ally has been eliminated. We cannot lose any more allies.";
    private string MissionSevereCasualties = "Commander, we have lost too many allies. We must retreat.";
    private string MissionCiviliansCasualties = "Commander, civilians have been eliminated. Remember our purpose.";
    private string MissionCriticalHit = "Nice shot commander! We knew we could count on you.";
    private string MissionEnemyCriticalHit = "Commander, the enemy has dealt a critical blow. Exercise caution.";
    private bool enemySighted = false;
    public override void _Ready()
    {
        Instance = this;
        ResetStatistics();

        GameManager.GameStateChanged += OnGameStateChanged;
    }

    public void SetMissionDetails(MissionDataCard missionDataCard)
    {
        MissionSuccessBriefing = missionDataCard.missionSuccessLabel;
        MissionFailureBriefing = missionDataCard.missionFailureLabel;
        MissionIntroBriefing = missionDataCard.missionStatusLabel;
        MissionType = missionDataCard.missionType;
    }

    public async Task ShowMissionBriefing(string message, bool autoClose = true, float timer = 3.0f)
    {
        if (currentBriefing != null)
        {
            currentBriefing.QueueFree();
            currentBriefing = null;
        }

        currentBriefing = briefingScene.Instantiate();
        GetTree().Root.AddChild(currentBriefing);

        var briefingCard = currentBriefing.GetNode<MissionBriefingCard>(".");
        briefingCard.SetMissionBriefing(message);

         var tween = CreateTween();
        tween.SetTrans(Tween.TransitionType.Sine);
        tween.SetEase(Tween.EaseType.Out);
        tween.TweenProperty(currentBriefing, "modulate:a", .85f, 0.3f).From(0.0f);

        if (autoClose)
        {
            // 3 seconds
            await ToSignal(GetTree().CreateTimer(timer), "timeout");
            
            // Fade-out
            tween = CreateTween();
            tween.SetTrans(Tween.TransitionType.Sine);
            tween.SetEase(Tween.EaseType.In);
            tween.TweenProperty(currentBriefing, "modulate:a", 0.0f, 0.3f);
            
            // Queue free when animation is finished
            await ToSignal(tween, "finished");
            currentBriefing.QueueFree();
            currentBriefing = null;
        }
    }

    private async void OnGameStateChanged(GameState current, GameState newState)
    {
        if (current != GameState.Battle)
            ResetStatistics();

        if (newState == GameState.Battle)
            await ShowMissionBriefing(MissionIntroBriefing, true);
    }

    // Public methods to update statistics
    public async void RecordShot(bool hit)
    {
        TotalShotsFired++;
        if (!enemySighted)
        {
            await ShowMissionBriefing(MissionEnemySighted, true);
            enemySighted = true;
        }
        if (hit) TotalHits++;
    }

    public async void RecordEnemyKill() {
        EnemiesKilled++;
        if (CalculateShowChance(40))
            await ShowMissionBriefing(MissionSuccessfulKill, true);
    }

    public async void RecordAllyLoss() {
        AlliesLost++;
        if (CalculateShowChance(100) && AlliesLost < 2)
            await ShowMissionBriefing(MissionAlliesLost, true);
        else if (CalculateShowChance(100) && AlliesLost >= 2)
            await ShowMissionBriefing(MissionAlliesLost, true);
    }
    public async void RecordCivilianCasualty(){
        CiviliansCasualties++;
        if (CalculateShowChance(100))
            await ShowMissionBriefing(MissionCiviliansCasualties, true);
    }
    public void RecordCoverUse() => CoverUsed++;
    public void RecordTurnComplete() => TurnsCompleted++;
    public async void RecordCriticalHit() {
        CriticalHits++;
        if (CalculateShowChance(50))
            await ShowMissionBriefing(MissionCriticalHit, true);
        else if (CalculateShowChance(100))
            await ShowMissionBriefing(MissionEnemyCriticalHit, true);
    }

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

    private bool CalculateShowChance(int chancePercentage)
    {
        if (GD.Randf() < chancePercentage / 100.0f)
            return true;
        return false;
    }
}