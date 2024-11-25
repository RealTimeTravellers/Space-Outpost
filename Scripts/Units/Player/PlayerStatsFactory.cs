
using System;

public static class PlayerStatsFactory
{
    public static StatContainer CreateStatsForPlayerType(PlayerType playerType)
    {
        return playerType switch
        {
            PlayerType.Soldier => new SoldierStats(),
            PlayerType.Medic => new MedicStats(),
            PlayerType.Engineer => new EngineerStats(),
            PlayerType.Heavy => new HeavyStats(),
            PlayerType.Sniper => new SniperStats(),
            _ => throw new ArgumentException($"Unknown player type: {playerType}")
        };
    }
}