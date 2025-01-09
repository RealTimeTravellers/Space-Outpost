using Godot;
using Godot.Collections;

public partial class GunData : Resource
{
    [Export] public string Name { get; set; } = "";
	[Export] public string Description { get; set; } = "";
	[Export] public int MagazineCapacity { get; private set; } = 0;
	//[Export] public int CurrentAmmoClip { get; set; } = 0;
	[Export] public int Accuracy { get; private set; } = 0;
	//[Export] public int Range { get; set; } = 0;
	[Export] public int MinDamage { get; private set; } = 0;
	[Export] public int MaxDamage { get; private set; } = 0;

    [Export] public Array<AudioStream> readySounds;
    [Export] public Array<AudioStream> ShootSounds;
    [Export] public Array<AudioStream> SuppressiveShootSounds;
    [Export] public Array<AudioStream> emptySounds;
    [Export] public Array<AudioStream> reloadSounds;

	//[Export] public int Impact { get; set; } = 0;
	//[Export] public int CritChance { get; set; } = 0;
	[Export] public Texture2D Icon { get; set; }
 
    /// <summary>
    /// For godot initialization
    /// </summary>
    public GunData(){}
}
