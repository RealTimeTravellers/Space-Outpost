using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class Gun : Node3D
{
    [Export] private GunData data;

    private int currentAmmo = 0;

	[Export] private GpuParticles3D shootHitEffect;
	[Export] private GpuParticles3D shootMissEffect;


    /// <summary>
    /// For Godot initialization
    /// </summary>
    private Gun(){}

    public override void _Ready()
    {
        currentAmmo = data.MagazineCapacity;
        base._Ready();
    }

    private int CalculateDamage()
    {
        // XXX: calculate damage based on range?
        return GD.RandRange(data.MinDamage, data.MaxDamage);
    }

    public int Fire(bool hit)
    {
        int damage = -1;
        if (currentAmmo > 0)
        {
            if (hit)
                shootHitEffect.Restart();
            
            else
                shootMissEffect.Restart();
            
                //TODO: play sound

            currentAmmo--;
            damage = CalculateDamage();        
        }

        return damage;
    }

    public void Reload()
    {
        // guns are unlimited ammo
        currentAmmo = data.MagazineCapacity;
    }
}
