using Godot;

public partial class MissionDataCard : Resource
{
    [Export] public string missionNameLabel;
    [Export] public string missionDescriptionLabel;
    [Export] public string missionStatusLabel;
    [Export] public string missionSuccessLabel;
    [Export] public string missionFailureLabel;
    [Export] public MissionType missionType;
}