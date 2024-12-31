using Godot;


public partial class CharacterStatBoxCard : Control
{
    [Export] public Label statName;
    [Export] public Label statValue;

    public void LoadStatDetails(string statName, int statValue)
    {
        this.statName.Text = statName;
        this.statValue.Text = statValue.ToString();
    }
}