using Godot;
public class TacticalState : EnemyState
{
    private GridObject _targetCover;

    public override async void Enter(Character enemy)
    {
        enemy.CharacterController.IsEnemyAlerted = true;
        enemy.Target = enemy.enemiesInLos[0];
        await enemy.enemyController.HandleTactical();
    }

    public override AIState Process(Character enemy)
    {
        return base.Process(enemy);
    }

    public override void Exit(Character enemy)
    {
        GD.Print($"[AI] {enemy.Name} Exiting Tactical State");
        _targetCover = null;
    }
}