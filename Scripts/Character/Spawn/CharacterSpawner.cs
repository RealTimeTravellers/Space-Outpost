using Godot;
using System.Collections.Generic;

public partial class CharacterSpawner : Node
{
    [Export] public Godot.Collections.Array<PackedScene> CharacterPrefabs { get; private set; }
    [Export] public Godot.Collections.Array<Node3D> CharacterSpawnPoints { get; private set; }
    [Export] public NavigationRegion3D _navigationRegion;
    private Dictionary<PlayerType, PackedScene> characterPrefabMap = new();

    public override void _Ready()
    {
        PrepareCharacterMap();
        SpawnSelectedTeam();
    }

    private void PrepareCharacterMap()
    {
        if (CharacterPrefabs == null || CharacterPrefabs.Count == 0)
        {
            GD.PrintErr("CharacterPrefabs is null or empty!");
            return;
        }

        foreach (var prefab in CharacterPrefabs)
        {
            var instance = prefab.Instantiate<Node3D>();
            var character = instance.GetNode<Character>(".");
            characterPrefabMap.Add(character.PlayerType, prefab);
            instance.QueueFree();
        }
    }

    private void SpawnSelectedTeam()
    {
        var teamMembers = TeamSelectionManager.Instance.partyMembers;
        GD.Print($"Team members count: {teamMembers.Count}");
        
        for (int i = 0; i < teamMembers.Count && i < CharacterSpawnPoints.Count; i++)
        {
            var spawnPosition = CharacterSpawnPoints[i].GlobalPosition;
            var playerType = teamMembers[i].UnitType;
            var grid = CharacterSpawnPoints[i].GetNode<SpawnPoint>(".").Grid;
            
            if (characterPrefabMap.TryGetValue(playerType, out PackedScene prefab))
            {
                var spawnedCharacter = prefab.Instantiate<Node3D>();
                AddChild(spawnedCharacter);
                spawnedCharacter.Name = $"{teamMembers[i].Name}";
                var character = spawnedCharacter.GetNode<Character>(".");
                character.CharacterController._navigationRegion = _navigationRegion;
                character.gun.SetGun(teamMembers[i].GunType);
                
                // Önce pozisyonu ayarla
                character.GlobalPosition = spawnPosition;
                character.currentGrid = grid;
                grid.IsOccupied = true;
            }
            else
            {
                GD.PrintErr($"No prefab found for PlayerType: {playerType}");
            }
        }
    }
}