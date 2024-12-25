using Godot;

public class CharacterDeathState : CharacterState
{
    private bool _deathAnimationStarted = false;
    private bool _deathProcessed = false;

    public override void Enter(Character character)
    {
        if (!_deathAnimationStarted)
        {
            _deathAnimationStarted = true;
            character.CharacterController._stateMachine.RequestAnimation("death");
        }
    }

    public override CharacterStateType Process(Character character)
    {
        // Animasyon bittiğinde Die() metodunu çağır
        if (_deathAnimationStarted && !_deathProcessed && 
            !character.AnimatorController.IsAnimationPlaying("death"))
        {
            _deathProcessed = true;
            character.Die();
        }
        return CharacterStateType.Death;
    }

    public override CharacterStateType CheckState(Character character)
    {
        return CharacterStateType.Death;
    }

    public override void Exit(Character character)
    {
        // Hiçbir zaman çıkış yapılmayacak
    }
}

