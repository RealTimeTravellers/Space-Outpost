using Godot;

public partial class CharacterStatPanelHUD : Control
{
    [Export] public ProgressBar HealthBar;
    [Export] public ProgressBar ArmorBar;
    [Export] public ProgressBar EvasionBar;

    [Export] public ProgressBar AccuracyBar;
    [Export] public ProgressBar DamageBar;
    [Export] public ProgressBar CriticalHitChanceBar;

    private Character selectedCharacter;

    public override void _Ready()
    {
        GridManager.Instance.SelectionChanged += UpdateCharacterUI;
        InitializeStatPanel();
    }

    public void UpdateCharacterUI(GridObject gridObject)
    {
        if (gridObject == null) return;

        selectedCharacter = GridManager.Instance.selectedCharacter;
        if (selectedCharacter == null) return;

        UpdateStatPanel(selectedCharacter);
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