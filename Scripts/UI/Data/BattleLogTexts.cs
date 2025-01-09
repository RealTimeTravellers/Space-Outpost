using Godot;

public partial class BattleLogTexts : Resource
{
    [Export] public string MissionSuccessfulKill = "Commander, the enemy has been eliminated. Continue to the next objective.";
    [Export] public string MissionEnemySighted = "Commander, the enemy has been sighted. Marking their position.";
    [Export] public string MissionAlliesLost = "Commander, an ally has been eliminated. We cannot lose any more allies.";
    [Export] public string MissionSevereCasualties = "Commander, we have lost too many allies. We must retreat.";
    [Export] public string MissionCiviliansCasualties = "Commander, civilians have been eliminated. Remember our purpose.";
    [Export] public string MissionCriticalHit = "Nice shot commander! We knew we could count on you.";
    [Export] public string MissionEnemyCriticalHit = "Commander, the enemy has dealt a critical blow. Exercise caution.";
    [Export] public string MissionReinforcementsArrived = "Commander, enemy reinforcements have arried, proceed with caution.";

    [Export] public string CharacterHitLog { get; set; } = "{0} hit {1} for {2} damage!";
    [Export] public string CharacterMissedLog { get; set; } = "{0} missed the shot at {1}!";
    [Export] public string CharacterDeathLog { get; set; } = "{0} has been eliminated!";
    [Export] public string CharacterCriticalHitLog { get; set; } = "{0} landed a critical hit on {1}!";
    [Export] public string TurnStartLog { get; set; } = "{0}'s turn has started.";
    [Export] public string TurnEndLog { get; set; } = "{0}'s turn has ended.";
    [Export] public string CharacterNoEnemiesInSightLog { get; set; } = "{0} has no enemies in sight.";
}
