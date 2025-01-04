using Godot;

public partial class EndMissionMenu : Control
{
    [Export] private GridContainer statsContainer;
    
    public override void _Ready()
    {
        statsContainer = GetNode<GridContainer>("CenterContainer/StatsPanel/VBoxContainer/StatsContainer");
        
        // Fade in animasyonu için GameMenuGUI'den referans alıyoruz
        this.Modulate = new Color(1, 1, 1, 0);
        var tween = CreateTween();
        tween.SetTrans(Tween.TransitionType.Sine);
        tween.SetEase(Tween.EaseType.Out);
        tween.TweenProperty(this, "modulate:a", 1.0f, 0.3f);
        
        UpdateStats();
    }

    private void UpdateStats()
    {
        var stats = MissionManager.Instance;
        AddStatRow("Combat Statistics", "");
        AddStatRow("Shots Fired", stats.TotalShotsFired.ToString());
        AddStatRow("Hits", stats.TotalHits.ToString());
        AddStatRow("Accuracy", $"{stats.Accuracy:F1}%");
        AddStatRow("Enemies Eliminated", stats.EnemiesKilled.ToString());
        AddStatRow("Allies Lost", stats.AlliesLost.ToString());
        AddStatRow("Civilians Casualties", stats.CiviliansCasualties.ToString());
        AddStatRow("Critical Hits", stats.CriticalHits.ToString());
        AddStatRow("Turns Completed", stats.TurnsCompleted.ToString());
        AddStatRow("Cover Used", stats.CoverUsed.ToString());
        AddStatRow("Hits Missed", stats.TotalHitsMissed.ToString());
        AddStatRow("Turns Completed", stats.TurnsCompleted.ToString());
    }

    private void AddStatRow(string label, string value)
    {
        var labelNode = new Label { Text = label };
        var valueNode = new Label { Text = value };
        
        statsContainer.AddChild(labelNode);
        statsContainer.AddChild(valueNode);
    }

    private void OnContinuePressed()
    {
        GameManager.ChangeGameState(GameState.End, GameState.Menu);
    }
}