using Godot;

public partial class ChracterAudio : Node
{
    [Export] private AudioStreamPlayer3D DeathSoundPlayer;

    public void PlayDeathSound()
    {
        DeathSoundPlayer.Play();
    }
}
