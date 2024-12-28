using Godot;
using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

public partial class Gun : Node3D
{
    [Export] private GunData data;

    private int currentAmmo = 0;

	[Export] private GpuParticles3D shootHitEffect;
	[Export] private GpuParticles3D shootMissEffect;

    [Export] private AudioStreamPlayer3D audioPlayer;


    /// <summary>
    /// For Godot initialization
    /// </summary>
    private Gun(){}

    public override void _Ready()
    {
        PlaySound(GunActionState.Ready);
        Reload();
        base._Ready();
    }

    /// <summary>
    /// Plays based on mode. (should make it enum)
    /// 0 is ready
    /// 1 is fire
    /// 2 is reload
    /// </summary>
    /// <param name="mode"></param>
    private void PlaySound(GunActionState state)
    {
        switch (state)
        {
            case GunActionState.Ready:
                if(data.readySounds.Count == 0)
                    return;
                audioPlayer.Stream = data.readySounds[GD.RandRange(0, data.readySounds.Count -1)];
                break;
            case GunActionState.Shoot:
                if(data.ShootSounds.Count == 0)
                    return;
                audioPlayer.Stream = data.ShootSounds[GD.RandRange(0, data.ShootSounds.Count -1)];
                break;
            case GunActionState.Empty:
                if(data.emptySounds.Count == 0)
                    return;
                audioPlayer.Stream = data.emptySounds[GD.RandRange(0, data.emptySounds.Count -1)];
                break;
            case GunActionState.Reload:
                if(data.reloadSounds.Count == 0)
                    return;
                audioPlayer.Stream = data.reloadSounds[GD.RandRange(0, data.reloadSounds.Count -1)];
                break;
        }
        audioPlayer.Play();
    }

    private int GetDamage()
    {
        // XXX: calculate damage based on range?
        return GD.RandRange(data.MinDamage, data.MaxDamage);
    }

    public int Fire(bool hit, bool critical = false)
    {
        int damage = -1;
        if (currentAmmo > 0)
        {
            if (hit)
                shootHitEffect.Emitting = true;
            else
                shootMissEffect.Emitting = true;
            
            PlaySound(GunActionState.Shoot);

            currentAmmo--;
            if (critical)
                damage = data.MaxDamage;
            else 
                damage = GetDamage();        
        }

        return damage;
    }

    public void Reload()
    {
        // guns are unlimited ammo
        currentAmmo = data.MagazineCapacity;
        PlaySound(GunActionState.Reload);
    }
}
