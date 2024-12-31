using Godot;
public partial class MissionSelectMenu : Control
{
    [Export] public Godot.Collections.Array<Button> missionButtons;
    [Export] public Godot.Collections.Array<MissionDataCard> missionDataCards;
    [Export] public Godot.Collections.Array<TextureRect> missionBackgrounds;
    [Export] public Button missionLaunchButton;
    [Export] public Label missionNameLabel;
    [Export] public Label missionDescriptionLabel;

    private int selectedMissionIndex = -1;
    private MissionDataCard selectedMissionDataCard = null;
    private TextureRect selectedMissionTexture = null;
    private const float SELECTED_SCALE = 1.1f;
    private const float NORMAL_SCALE = 1.0f;
    private readonly Color INACTIVE_COLOR = new(0.5f, 0.5f, 0.5f, 1f);
    private readonly Color ACTIVE_COLOR = new(1f, 1f, 1f, 1f);

    public override void _Ready()
    {
        missionLaunchButton.Disabled = true;
        
        foreach (var button in missionButtons)
        {
            // Pivot noktasını butonun ortasına ayarla
            button.PivotOffset = button.Size / 2;
            button.Scale = new Vector2(NORMAL_SCALE, NORMAL_SCALE);
            button.Modulate = INACTIVE_COLOR;
            button.Pressed += () => OnMissionSelected(button);
        }
    }

    private void OnMissionSelected(Button clickedButton)
    {
        if (selectedMissionTexture != null)
        {
            ScaleMissionTexture(selectedMissionTexture, false);
            selectedMissionTexture.Modulate = INACTIVE_COLOR;
        }

        // Yeni texture'ı seç ve büyüt
        var newTexture = clickedButton.GetParent().GetParent<TextureRect>();
        ScaleMissionTexture(newTexture, true);
        newTexture.Modulate = ACTIVE_COLOR;
        
        selectedMissionTexture = newTexture;
        selectedMissionIndex = missionButtons.IndexOf(clickedButton);
        
        missionLaunchButton.Disabled = false;

        if (selectedMissionIndex >= 0 && selectedMissionIndex < missionDataCards.Count)
        {
            UpdateMissionDataCard(missionDataCards[selectedMissionIndex]);
        }
    }

    public void UpdateMissionDataCard(Resource missionDataCard)
    {
        selectedMissionDataCard = (MissionDataCard)missionDataCard;
        missionNameLabel.Text = selectedMissionDataCard.missionNameLabel;
        missionDescriptionLabel.Text = selectedMissionDataCard.missionDescriptionLabel;
    }

    private void ScaleMissionTexture(TextureRect texture, bool enlarge)
    {
        var targetScale = enlarge ? SELECTED_SCALE : NORMAL_SCALE;
        
        var tween = CreateTween();
        tween.TweenProperty(texture, "scale", new Vector2(targetScale, targetScale), 0.2f)
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.Out);
    }

    public void OnMissionLaunchPressed()
    {
        if (selectedMissionIndex != -1)
        {
            GameManager.ChangeGameState(GameState.MissionSelect, GameState.Battle);
        }
    }

    public void OnCancelPressed()
    {
        GameManager.ChangeGameState(GameState.MissionSelect, GameState.TeamSelect);
    }
}