using Godot;

public partial class CharacterUIUnitCard : Control
{
    [Export] public PlayerType UnitType;
    [Export] public TextureRect unitIcon;
    [Export] public TextureRect abilityIcon;
    [Export] public TextureRect abilityIcon2;
    [Export] public Label unitName;
    [Export] public Label unitClass;
    private int slotIndex;
    private TeamSelectionMenu teamSelectionMenu;
    public void InitCard(TeamSelectionMenu teamSelectionMenu, ClassInfo card, int index, string name)
    {
        this.teamSelectionMenu = teamSelectionMenu;
        UnitType = card.UnitType;
        unitIcon.Texture = card.ClassIcon;
        abilityIcon.Texture = card.AbilityIcon;
        abilityIcon2.Texture = card.AbilityIcon2;
        unitName.Text = name;
        unitClass.Text = card.DetailsText;
        slotIndex = index;
    }

    private void OnRemovePressed()
    {
        teamSelectionMenu.RemovePartyMember(this, slotIndex);
        QueueFree();
    }
}
