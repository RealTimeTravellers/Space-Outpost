using Godot;

public partial class MissionBriefingCard : Control
{
    [Export] public Label missionBriefingLabel;

    public void SetMissionBriefing(string briefing)
    {
        missionBriefingLabel.Text = briefing;
    }
}