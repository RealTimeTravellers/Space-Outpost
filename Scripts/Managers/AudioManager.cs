using Godot;

public partial class AudioManager : Node
{
    public static AudioManager Instance { get; private set; }

    [Export] public AudioStreamPlayer backgorundPlayer { get; private set; }
    [Export] public AudioStream BrownNoise { get; private set; }

    private AudioManager()
    {
        Instance = this;
    }

    public void PlayBackgroundNoise()
    {
        backgorundPlayer.Stream = BrownNoise;
        backgorundPlayer.Play();
    }
}
