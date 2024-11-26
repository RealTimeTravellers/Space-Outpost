public class CharacterTacticalState : CharacterState
{
    public override void Enter(Character character)
    {
        // Animasyon değişimi
        // character.GetNode<AnimatorComponent>("AnimatorComponent")?.PlayAnimation("tactical_stance");
    }

    public override CharacterStateType Process(Character character)
    {
        // State değişim mantığı
        if (character.Stats.Health.GetValue() <= 0)
            return CharacterStateType.Death;
            
        if (character.Equipment.CurrentWeapon.NeedsReload())
            return CharacterStateType.Reloading;
            
        if (EnemyInSight(character) && !character.IsInCover)
            return CharacterStateType.TakingCover;
            
        return CharacterStateType.Idle;
    }

    public override void Exit(Character character)
    {

    }
}