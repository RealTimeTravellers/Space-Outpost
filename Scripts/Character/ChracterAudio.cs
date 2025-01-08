using Godot;

public partial class ChracterAudio : Node
{
    [Export] public AudioStreamPlayer3D DeathSoundPlayer;
    [Export] public AudioStream FriendlyDeathSound;
    [Export] public AudioStream EnemyDeathSound;

    public void PlayDeathSound(bool isFriendly)
    {
        DeathSoundPlayer.Stream = isFriendly ? FriendlyDeathSound : EnemyDeathSound;
        DeathSoundPlayer.Play();
    }
}