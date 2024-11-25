public class PlayerTacticalState : PlayerState
{
    public override void Enter(Character character)
    {
        // Animasyon değişimi
        // character.GetNode<AnimatorComponent>("AnimatorComponent")?.PlayAnimation("tactical_stance");
    }

    public override PlayerStateType Process(Character character)
    {
        // State değişim mantığı
        if (character.Stats.Health.GetValue() <= 0)
            return PlayerStateType.Death;
            
        if (character.Equipment.CurrentWeapon.NeedsReload())
            return PlayerStateType.Reloading;
            
        if (EnemyInSight(character) && !character.IsInCover)
            return PlayerStateType.TakingCover;
            
        return PlayerStateType.Idle;
    }

    public override void Exit(Character character)
    {

    }
}