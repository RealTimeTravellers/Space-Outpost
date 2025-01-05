using Godot;
using System.Collections.Generic;
public partial class EnemySpawner : Node
{
    [Export] public Godot.Collections.Array<PackedScene> EnemyPrefabs { get; private set; }
    [Export] public Godot.Collections.Array<Node3D> SpawnPoints { get; private set; }
    private Dictionary<EnemyType, PackedScene> enemyPrefabMap = new();

    public override void _Ready()
    {
        PrepareEnemyMap();
        SpawnEnemies();
    }

    private void PrepareEnemyMap()
    {
        if (EnemyPrefabs == null || EnemyPrefabs.Count == 0)
        {
            GD.PrintErr("EnemyPrefabs is null or empty!");
            return;
        }

        foreach (var prefab in EnemyPrefabs)
        {
            var instance = prefab.Instantiate<Node3D>();
            var character = instance.GetNode<Character>(".");
            enemyPrefabMap.Add(character.EnemyType, prefab);
            instance.QueueFree();
        }
    }

    private void SpawnEnemies()
    {
        foreach (var spawnPoint in SpawnPoints)
        {
            var enemySpawnPoint = spawnPoint.GetNode<EnemySpawnPoint>(".");
            var enemyType = enemySpawnPoint.EnemyType;
            var grid = enemySpawnPoint.Grid;

            if (!enemyPrefabMap.ContainsKey(enemyType))
            {
                GD.PrintErr($"No prefab found for EnemyType: {enemyType}, using Rebel instead");
                enemyType = EnemyType.Rebel;
            }

            var prefab = enemyPrefabMap[enemyType];
            var spawnedEnemy = prefab.Instantiate<Character>(); 
            AddChild(spawnedEnemy);
            spawnedEnemy.Name = $"{enemyType}";
            
            spawnedEnemy.GlobalPosition = spawnPoint.GlobalPosition;
            spawnedEnemy.currentGrid = grid;
            grid.IsOccupied = true;

        }
    }
}