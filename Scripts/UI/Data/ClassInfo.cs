using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Godot;

public enum WeaponDisplayType
{
    NoWeapon,
    WithWeapon
}

[Tool]
public partial class ClassInfo : Resource
{
    [Export] public PlayerType UnitType { get; set; } = PlayerType.Soldier;
    [Export] public WeaponDisplayType WeaponDisplayType { get; set; } = WeaponDisplayType.NoWeapon;
    [Export] public string DetailsText { get; set; } = "";
    [Export] public string DescriptionText { get; set; } = "";
    [Export] public Texture2D ClassIcon { get; set; }
    [Export] public Texture2D AbilityIcon { get; set; }
    [Export] public Texture2D AbilityIcon2 { get; set; }
}