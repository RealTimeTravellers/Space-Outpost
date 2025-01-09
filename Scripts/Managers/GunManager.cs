using Godot;
using System.Collections.Generic;

public enum GunType
{
    Cerebus,
    Phantom,
}

public partial class GunManager : Node
{
    [Export] public Godot.Collections.Array<PackedScene> gunModels;
    [Export] public Godot.Collections.Array<GunData> gunData;

    private Dictionary<GunType, (PackedScene Model, GunData Data)> _guns = new();

    private static GunManager _instance;
    public static GunManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = new GunManager();
            return _instance;
        }
    }

    public override void _Ready()
    {
        _instance = this;
        InitializeGunDictionary();
    }

    private void InitializeGunDictionary()
    {
        // Sıralama önemli - GunType enum sırası ile aynı olmalı
        for (int i = 0; i < gunModels.Count; i++)
        {
            _guns[(GunType)i] = (gunModels[i], gunData[i]);
        }
    }

    public (PackedScene Model, GunData Data) GetGun(GunType type)
    {
        return _guns[type];
    }
}