using Godot;

public class PlayerDeathState : PlayerState
{
    public override void Enter(Character character)
    {
        GD.Print("Entering Death State");
        // Ölüm animasyonunu başlat
        // Input'ları devre dışı bırak
        // Gerekli event'leri tetikle
    }

    public override PlayerStateType Process(Character character)
    {
        return CheckState(character);
    }

    public override PlayerStateType CheckState(Character character)
    {
        // Ölüm state'inden başka bir state'e geçiş yok
        return PlayerStateType.Death;
    }

    public override void Exit(Character character)
    {
        GD.Print("Exiting Death State");
        // Bu state'ten çıkış olmayacak ama yine de implement edelim
    }
}