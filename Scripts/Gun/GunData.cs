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

    [Export] Array<AudioStream> readySounds;
    [Export] Array<AudioStream> ShootSounds;
    [Export] Array<AudioStream> reloadSounds;

	//[Export] public int Impact { get; set; } = 0;
	//[Export] public int CritChance { get; set; } = 0;
	[Export] public Texture2D Icon { get; set; }
 
    /// <summary>
    /// For godot initialization
    /// </summary>
    public GunData(){}
}
