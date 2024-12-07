using Godot;

public class PatrolState : EnemyState
{
    public override void Enter(Character enemy)
    {
        //GD.Print("Entering Patrol State");
    }

    public override AIState Process(Character enemy)
    {
        // Devriye gezme mantığı
        var nextState = base.CheckState(enemy);
        if (nextState != AIState.Patrol)
            return nextState;
            
        // Devriye noktaları arasında hareket et
        if (!enemy.IsInCover)
            enemy.TakeCover();
            
        return AIState.Patrol;
    }

    public override void Exit(Character enemy)
    {
        //GD.Print("Exiting Patrol State");
    }
}
