using Godot;

public class CharacterDeathState : CharacterState
{
    public override void Enter(Character character)
    {
        character.CharacterController._stateMachine.RequestAnimation("death");
        character.Velocity = Vector3.Zero;
   }
    public override CharacterStateType Process(Character character)
   {
       return CheckState(character);
   }
    public override CharacterStateType CheckState(Character character)
   {
       return CharacterStateType.Death;
   }
    public override void Exit(Character character)
   {

   }
}
