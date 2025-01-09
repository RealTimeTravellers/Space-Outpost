using Godot;

public enum FireType
{
    Attack,
    SuppressiveFire,
}

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
            fireButton.MouseFilter = Control.MouseFilterEnum.Stop;
            UpdateAttackPanel(true);
        }
        else
        {
            fireButton.Visible = false;
            fireButton.MouseFilter = Control.MouseFilterEnum.Ignore;
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
            var selectedGun = character.gun;
            
            int minDamage = selectedGun.data.MinDamage;
            int maxDamage = selectedGun.data.MaxDamage;
            
            DamageLabel.Text = $"Damage: {minDamage} - {maxDamage}";
            AccuracyLabel.Text = "Accuracy: " + (character.Stats.Accuracy.GetValue() - target.Stats.Evasion.GetValue()).ToString() + selectedGun.data.Accuracy;
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
