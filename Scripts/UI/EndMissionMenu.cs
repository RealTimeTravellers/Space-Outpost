using Godot;

public partial class EndMissionMenu : Control
{
    [Export] private GridContainer statsContainer;
    
    public override void _Ready()
    {
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
        if(stats != null)
        {
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
        else
        {
            // tmp for testing.
            AddStatRow("Combat Statistics", "");
            AddStatRow("Shots Fired", "0");
            AddStatRow("Hits", "0");
            AddStatRow("Accuracy", "0%");
            AddStatRow("Enemies Eliminated", "0");
            AddStatRow("Allies Lost", "0");
            AddStatRow("Civilians Casualties", "0");
            AddStatRow("Critical Hits", "0");
            AddStatRow("Turns Completed", "0");
            AddStatRow("Cover Used", "0");
            AddStatRow("Hits Missed", "0");
            AddStatRow("Turns Completed", "0");
        }

    }

    private void AddStatRow(string label, string value = "0")
    {
        var labelNode = new Label 
        { 
            Text = label,
            HorizontalAlignment = HorizontalAlignment.Right,
            CustomMinimumSize = new Vector2(350, 0),
            //ThemeOverrideFontSize = 20
        };
        
        var valueNode = new Label 
        { 
            Text = value,
            HorizontalAlignment = HorizontalAlignment.Left,
            CustomMinimumSize = new Vector2(100, 0),
            //ThemeOverrideFontSize = 20
        };
        
        statsContainer.AddChild(labelNode);
        statsContainer.AddChild(valueNode);
    }

    private void OnContinuePressed()
    {
        GameManager.ChangeGameState(GameState.End, GameState.Menu);
    }
}