using Godot;

public class CharacterDeathState : CharacterState
{
    public override void Enter(Character character)
    {
        GD.Print("Entering Death State");
        // Ölüm animasyonunu başlat
        // Input'ları devre dışı bırak
        // Gerekli event'leri tetikle
    }

    public override CharacterStateType Process(Character character)
    {
        return CheckState(character);
    }

    public override CharacterStateType CheckState(Character character)
    {
        // Ölüm state'inden başka bir state'e geçiş yok
        return CharacterStateType.Death;
    }

    public override void Exit(Character character)
    {
        GD.Print("Exiting Death State");
        // Bu state'ten çıkış olmayacak ama yine de implement edelim
    }
}