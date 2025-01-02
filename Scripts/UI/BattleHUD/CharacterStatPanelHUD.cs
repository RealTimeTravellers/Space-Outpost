using Godot;

public partial class CharacterStatPanelHUD : Control
{
    [Export] public Control StatPanel;
    [Export] public ProgressBar HealthBar;
    [Export] public ProgressBar ArmorBar;
    [Export] public ProgressBar EvasionBar;

    [Export] public ProgressBar AccuracyBar;
    [Export] public ProgressBar DamageBar;
    [Export] public ProgressBar CriticalHitChanceBar;

    private Character selectedCharacter;

    public override void _Ready()
    {
        if (StatPanel != null)  // Add null check
        {
            GridManager.Instance.SelectionChanged += UpdateCharacterUI;
            InitializeStatPanel();
            StatPanel.Visible = false;
        }
        else
        {
            GD.PrintErr("StatPanel not assigned in CharacterStatPanelHUD");
        }
    }

    public void UpdateCharacterUI(GridObject gridObject)
    {
        if (gridObject == null) return;

        selectedCharacter = GridManager.Instance.selectedCharacter;

        if (selectedCharacter == null){
            StatPanel.Visible = false;
            return;
        }

        UpdateStatPanel(selectedCharacter);
        StatPanel.Visible = true;
    }

    public void InitializeStatPanel()
    {
        HealthBar.MaxValue = 12;
        ArmorBar.MaxValue = 6;
        EvasionBar.MaxValue = 25;

        AccuracyBar.MaxValue = 100;
        DamageBar.MaxValue = 7;
        CriticalHitChanceBar.MaxValue = 30;
    }

    public void UpdateStatPanel(Character character)
    {
        HealthBar.Value = character.Stats.Health.GetValue();
        ArmorBar.Value = character.Stats.Armor.GetValue();
        EvasionBar.Value = character.Stats.Evasion.GetValue();
        AccuracyBar.Value = character.Stats.Accuracy.GetValue();
        DamageBar.Value = 4;
        CriticalHitChanceBar.Value = character.Stats.CriticalHitChance.GetValue();
    }
}