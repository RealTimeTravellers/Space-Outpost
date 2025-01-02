using Godot;
using System.Collections.Generic;
public partial class EnemyStatusPanelHUD : Control
{
    [Export] public HBoxContainer EnemyListContainer;
    [Export] public PackedScene EnemyStatusTexture;
    private Dictionary<Character, Node> enemyNodes = new();

    public override void _Ready()
    {
        TurnManager.Instance.CharacterDied += OnEnemyDeath;
        InitializeEnemiesAsync();
    }

    private async void InitializeEnemiesAsync()
    {
        await ToSignal(GetTree().CreateTimer(0.1f), Timer.SignalName.Timeout);
        if (TurnManager.Instance != null)
        {
            foreach (var enemy in TurnManager.Instance.enemyCharacters)
            {
                AddEnemyStatus(enemy);
            }
        }
    }

    private void AddEnemyStatus(Character enemy)
    {
        if (enemy == null || EnemyStatusTexture == null) return;

        var enemyStatus = EnemyStatusTexture.Instantiate();
        EnemyListContainer.AddChild(enemyStatus);
        
        enemyNodes[enemy] = enemyStatus;
        enemyStatus.GetNode<EnemyStatusTextureHUD>(".").InitializeEnemyTexture(enemy);
    }

    private void OnEnemyDeath(Character enemy)
    {
        RemoveEnemyStatus(enemy);
    }

    private void RemoveEnemyStatus(Character enemy)
    {
        if (enemyNodes.TryGetValue(enemy, out Node statusNode))
        {
            statusNode.QueueFree();
            enemyNodes.Remove(enemy);
        }
    }

    public override void _ExitTree()
    {
        foreach (var node in enemyNodes.Values)
        {
            node.QueueFree();
        }
        enemyNodes.Clear();
    }
}
