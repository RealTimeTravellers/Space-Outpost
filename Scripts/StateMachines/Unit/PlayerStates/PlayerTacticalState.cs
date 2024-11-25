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
        if (character.Stats.CurrentHealth <= 0)
            return PlayerStateType.Death;
            
        if (character.Equipment.CurrentWeapon.NeedsReload())
            return PlayerStateType.Reloading;
            
        if (EnemyInSight(character) && !character.IsInCover())
            return PlayerStateType.TakingCover;
            
        return PlayerStateType.Tactical;
    }

    public override void Exit(Character character)
    {
        // Normal hareket hızına dön
        character.Stats.MovementSpeed /= 0.7f;
    }
}