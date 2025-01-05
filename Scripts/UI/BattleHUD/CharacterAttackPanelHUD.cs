using Godot;

public partial class CharacterAttackPanelHUD : Control
{
    [Export] public Label DamageLabel;
    [Export] public Label AccuracyLabel;
    [Export] public Label CriticalHitChanceLabel;
    [Export] public Label EvasionLabel;
    [Export] public Control fireButton;

    public void OnAimUIUpdate()
    {
        if (CameraManager.Instance.AimingMode)
        {
            fireButton.Visible = true;
            UpdateAttackPanel(true);
        }
        else
        {
            fireButton.Visible = false;
            UpdateAttackPanel(false);
        }
    }

    private void UpdateAttackPanel(bool isAiming)
    {
        if (isAiming)
        {
            var character = GridManager.Instance.selectedCharacter;
            var target = character.Target;
            var armorValue = target.Stats.Armor.GetValue();
            // Base damage
            int baseDamage = 7; 
            
            int minDamage = Mathf.Max(1, baseDamage - armorValue);
            int maxDamage = baseDamage;
            
            DamageLabel.Text = $"Damage: {minDamage} - {maxDamage}";
            AccuracyLabel.Text = "Accuracy: " + (character.Stats.Accuracy.GetValue() - target.Stats.Evasion.GetValue()).ToString();
            CriticalHitChanceLabel.Text = "Crit Chance: " + character.Stats.CriticalHitChance.GetValue().ToString();
            EvasionLabel.Text = "Evasion: " + target.Stats.Evasion.GetValue().ToString();
        }
        else
        {
            DamageLabel.Text = "";
            AccuracyLabel.Text = "";
            CriticalHitChanceLabel.Text = "";
            EvasionLabel.Text = "";
        }
    }
}
