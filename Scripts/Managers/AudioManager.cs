using Godot;

public partial class AudioManager : Node
{
    public static AudioManager Instance { get; private set; }

    [Export] public AudioStreamPlayer backgroundPlayer { get; private set; }
    [Export] public AudioStream BrownNoise { get; private set; }

    private AudioManager()
    {
        Instance = this;
    }

    public override void _Ready()
    {
        GameManager.GameStateChanged += OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState oldstate, GameState currentstate)
    {
        if (currentstate == GameState.Battle)
        {
            GD.Print("gameState battle");
            PlayBackgroundNoise();
        }
    }

    public void PlayBackgroundNoise()
    {
        backgroundPlayer.Stream = BrownNoise;
        backgroundPlayer.PitchScale = 0.5f;
        backgroundPlayer.Play();
    }
}
