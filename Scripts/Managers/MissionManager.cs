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
    [Export] public BattleLogTexts logTexts;
    public MissionType MissionType;
    private LoggingPanelHUD logPanel;


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
    private bool enemySighted = false;
    public override void _Ready()
    {
        Instance = this;
        ResetStatistics();

        GameManager.GameStateChanged += OnGameStateChanged;
    }

    #region Mission Setup

    public void SetMissionDetails(MissionDataCard missionDataCard)
    {
        MissionSuccessBriefing = missionDataCard.missionSuccessLabel;
        MissionFailureBriefing = missionDataCard.missionFailureLabel;
        MissionIntroBriefing = missionDataCard.missionStatusLabel;
        MissionType = missionDataCard.missionType;
    }

    #endregion

    #region Mission Briefing

    public async Task ShowMissionBriefing(string message, bool autoClose = true, float timer = 3.0f)
    {
        if (briefingScene == null)
        {
            GD.PrintErr("Briefing scene is not assigned!");
            return;
        }

        if (currentBriefing != null)
        {
            currentBriefing.QueueFree();
            currentBriefing = null;
        }

        currentBriefing = briefingScene.Instantiate<Node>();
        if (currentBriefing == null)
        {
            GD.PrintErr("Failed to instantiate briefing scene!");
            return;
        }

        GetTree().Root.AddChild(currentBriefing);
        var briefingCard = currentBriefing.GetNode<MissionBriefingCard>(".");
        briefingCard.SetMissionBriefing(message);

        var tween = CreateTween();
        tween.SetTrans(Tween.TransitionType.Sine);
        tween.SetEase(Tween.EaseType.Out);
        tween.TweenProperty(currentBriefing, "modulate:a", .85f, 0.3f).From(0.0f);

        if (autoClose)
        {
            await ToSignal(GetTree().CreateTimer(timer), "timeout");
            
            if (currentBriefing != null)
            {
                tween = CreateTween();
                tween.SetTrans(Tween.TransitionType.Sine);
                tween.SetEase(Tween.EaseType.In);
                tween.TweenProperty(currentBriefing, "modulate:a", 0.0f, 0.3f);
                
                await ToSignal(tween, "finished");
                
                if (currentBriefing != null)
                {
                    currentBriefing.QueueFree();
                    currentBriefing = null;
                }
            }
        }
    }

    private async void OnGameStateChanged(GameState current, GameState newState)
    {
        if (current != GameState.InsideBuilding || current != GameState.Desert)
            ResetStatistics();

        if (newState == GameState.InsideBuilding || newState == GameState.Desert)
            await ShowMissionBriefing(MissionIntroBriefing, true);
    }

    // Public methods to update statistics
    public async void RecordShot(bool hit)
    {
        TotalShotsFired++;
        if (!enemySighted)
        {
            await ShowMissionBriefing(logTexts.MissionEnemySighted, true);
            enemySighted = true;
        }
        if (hit) TotalHits++;
    }

    public async void RecordEnemyKill() {
        EnemiesKilled++;
        if (CalculateShowChance(40))
            await ShowMissionBriefing(logTexts.MissionSuccessfulKill, true);
    }

    public async void RecordAllyLoss() {
        AlliesLost++;
        if (CalculateShowChance(100) && AlliesLost < 2)
            await ShowMissionBriefing(logTexts.MissionAlliesLost, true);
        else if (CalculateShowChance(100) && AlliesLost >= 2)
            await ShowMissionBriefing(logTexts. MissionAlliesLost, true);
    }
    public async void RecordCivilianCasualty(){
        CiviliansCasualties++;
        if (CalculateShowChance(100))
            await ShowMissionBriefing(logTexts.MissionCiviliansCasualties, true);
    }
    public void RecordCoverUse() => CoverUsed++;
    public void RecordTurnComplete() => TurnsCompleted++;
    public async void RecordCriticalHit() {
        CriticalHits++;
        if (CalculateShowChance(50))
            await ShowMissionBriefing(logTexts.MissionCriticalHit, true);
        else if (CalculateShowChance(100))
            await ShowMissionBriefing(logTexts.MissionEnemyCriticalHit, true);
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
        enemySighted = false;
    }

    #endregion

    private bool CalculateShowChance(int chancePercentage)
    {
        if (GD.Randf() < chancePercentage / 100.0f)
            return true;
        return false;
    }

    #region Logger

    public void InitializeLogger(LoggingPanelHUD panel)
    {
        logPanel = panel;
    }

    public void AddBattleLog(string message, bool isEnemy = false, bool isFriendly = false)
    {
        if (logPanel == null) return;
        
        Color color = isEnemy ? new Color(1, 0.3f, 0.3f) : (isFriendly ? new Color(0.3f, 0.7f, 1) : new Color(1, 1, 1));
        logPanel.AddLogEntry(message, color);
    }

    public void AddCharacterLog(string format, bool isEnemy, params object[] args)
    {
        if (logPanel == null) return;
        
        string message = string.Format(format, args);
        Color color = isEnemy ? new Color(1, 0.3f, 0.3f) : new Color(0.3f, 0.7f, 1);
        logPanel.AddLogEntry(message, color);
    }

    #endregion
}