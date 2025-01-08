using Godot;
using System.Collections.Generic;
using System;
public partial class EnemySpawner : Node
{
    [Export] public Godot.Collections.Array<PackedScene> EnemyPrefabs { get; private set; }
    [Export] public Godot.Collections.Array<Node3D> SpawnPoints { get; private set; }
    [Export] public Godot.Collections.Array<Node3D> ReinforcementSpawnPoints { get; private set; }
    [Export] public NavigationRegion3D _navigationRegion;
    private Dictionary<EnemyType, PackedScene> enemyPrefabMap = new();
    public Action<Character> EnemySpawned;

    public override void _Ready()
    {
        EnemyManager.Instance.enemySpawner = this;
        PrepareEnemyMap();
        SpawnStartingEnemies();
        EnemyManager.Instance.SpecialEnemyDied += SpawnEnemyReinforcements;
    }

    private void ExitTree()
    {
        if (EnemyManager.Instance != null)
            EnemyManager.Instance.SpecialEnemyDied -= SpawnEnemyReinforcements;
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

    private void SpawnStartingEnemies()
    {
        foreach (var spawnPoint in SpawnPoints)
        {
            var enemySpawnPoint = spawnPoint.GetNode<EnemySpawnPoint>(".");
            var enemyType = enemySpawnPoint.EnemyType;
            var grid = enemySpawnPoint.Grid;

            SpawnEnemy(enemyType, grid, spawnPoint.GlobalPosition, enemySpawnPoint.IsSpecialEnemy, false);
        }
    }

    private async void SpawnEnemyReinforcements()
    {
        foreach (var spawnPoint in ReinforcementSpawnPoints)
        {
            var enemySpawnPoint = spawnPoint.GetNode<EnemySpawnPoint>(".");
            var enemyType = enemySpawnPoint.EnemyType;
            var grid = enemySpawnPoint.Grid;

            if (grid.IsOccupied || grid.IsBlocked) 
                continue;

            var enemy = SpawnEnemy(enemyType, grid, spawnPoint.GlobalPosition, enemySpawnPoint.IsSpecialEnemy, true);
            enemy.ApplyHologramEffect();
        }
        await MissionManager.Instance.ShowMissionBriefing(MissionManager.Instance.logTexts.MissionReinforcementsArrived, true);
    }

    private Character SpawnEnemy(EnemyType enemyType, GridObject grid, Vector3 position, bool isSpecialEnemy = false, bool isReinforcement = false)
    {
        if (!enemyPrefabMap.ContainsKey(enemyType))
        {
            GD.PrintErr($"No prefab found for EnemyType: {enemyType}, using Rebel instead");
            enemyType = EnemyType.Rebel;
        }

        var prefab = enemyPrefabMap[enemyType];
        var spawnedEnemy = prefab.Instantiate<Character>();
        AddChild(spawnedEnemy);
        spawnedEnemy.Name = $"{enemyType}";
        spawnedEnemy.CharacterController._navigationRegion = _navigationRegion;
        spawnedEnemy.IsSpecialEnemy = isSpecialEnemy;
        spawnedEnemy.GlobalPosition = position;
        spawnedEnemy.currentGrid = grid;
        grid.IsOccupied = true;

        if (isReinforcement)
            EnemyManager.Instance.OnEnemyReinforcementSpawned(spawnedEnemy);
        
        return spawnedEnemy;
    }
}