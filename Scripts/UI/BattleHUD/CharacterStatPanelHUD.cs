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

    [Export] public Texture2D ammoIcon;
    [Export] public Control ammoBoxContainer;

    private Character selectedCharacter;

    public override void _Ready()
    {
        if (StatPanel != null)  // Add null check
        {
            GridManager.Instance.SelectionChanged += UpdateCharacterUI;
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
        UpdateAmmoBox(selectedCharacter);
        PrepareStatPanel(selectedCharacter);
        StatPanel.Visible = true;
    }

    private void PrepareStatPanel(Character character)
    {
        HealthBar.MaxValue = character.MaxHealth;
        ArmorBar.MaxValue = 4;
        EvasionBar.MaxValue = 40;

        AccuracyBar.MaxValue = 100;
        DamageBar.MaxValue = 10;
        CriticalHitChanceBar.MaxValue = 40;
    }

    private void UpdateAmmoBox(Character character)
    {
        foreach (Node child in ammoBoxContainer.GetChildren())
        {
            child.QueueFree();
        }

        for (int i = 0; i < character.gun.currentAmmo; i++)
        {
            var textureRect = new TextureRect
            {
                Texture = ammoIcon,
                CustomMinimumSize = new Vector2(20, 20), // Icon
                ExpandMode = TextureRect.ExpandModeEnum.FitHeight,
                StretchMode = TextureRect.StretchModeEnum.KeepCentered
            };
            
            ammoBoxContainer.AddChild(textureRect);
        }
    }

    private void UpdateStatPanel(Character character)
    {
        HealthBar.Value = character.Stats.Health.GetValue();
        ArmorBar.Value = character.Stats.Armor.GetValue();
        EvasionBar.Value = character.Stats.Evasion.GetValue();
        AccuracyBar.Value = character.Stats.Accuracy.GetValue();
        DamageBar.Value = character.Damage;
        CriticalHitChanceBar.Value = character.Stats.CriticalHitChance.GetValue();
    }


}