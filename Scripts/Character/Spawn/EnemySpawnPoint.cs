using Godot;

public partial class EnemySpawnPoint : SpawnPoint
{
    [Export] public EnemyType EnemyType;
    [Export] public GunType GunType;
    [Export] public bool IsSpecialEnemy;
}
