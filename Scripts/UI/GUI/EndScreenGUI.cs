using Godot;
public partial class EndScreenGUI : Control
{
    [Export] private Label missionBriefingLabel;
    [Export] private string victoryText = "MISSION COMPLETE!";
    [Export] private string defeatText = "MISSION FAILED!";


    [Export] private Texture2D missionSuccessImage;
    [Export] private Texture2D missionFailureImage;
    [Export] private TextureRect missionStatusImage;

    [Export] private Label missionStatusLabel;

    private const float ANIMATION_DURATION = 0.3f;
    private const float MIN_SCALE = 0.8f;
    private const float MAX_SCALE = 1.0f;

    public override void _Ready()
    {
        this.Scale = Vector2.One * MIN_SCALE;
        this.Modulate = new Color(1, 1, 1, 0);
        
        var tween = CreateTween();
        tween.SetParallel(true);
        tween.SetTrans(Tween.TransitionType.Sine);
        tween.SetEase(Tween.EaseType.Out);

        tween.TweenProperty(this, "scale", Vector2.One * MAX_SCALE, ANIMATION_DURATION)
            .From(Vector2.One * MIN_SCALE);
        tween.TweenProperty(this, "modulate:a", .7f, ANIMATION_DURATION)
            .From(0.0f);
    }

    public void InitEndScreen(bool isVictory)
    {
        UpdateEndScreen(isVictory);
    }

    public override void _ExitTree()
    {
    }

    private void UpdateEndScreen(bool isVictory)
    {
        missionStatusImage.Texture = isVictory ? missionSuccessImage : missionFailureImage;
        missionStatusLabel.Text = isVictory ? victoryText : defeatText;
        missionBriefingLabel.Text = isVictory ? MissionManager.Instance.MissionSuccessBriefing : MissionManager.Instance.MissionFailureBriefing;
    }

    public void OnEndMissionClicked()
    {
        GameManager.ChangeGameState(GameState.Battle, GameState.End);
    }
}