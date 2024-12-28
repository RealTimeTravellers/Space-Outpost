using System.Diagnostics;
using Godot;

public partial class AudioManager : Node
{
    public static AudioManager Instance { get; private set; }

    [Export] public AudioStreamPlayer BrownNoisePlayer { get; private set; }

    [Export] public float combatLoopCutoff = 129.7f;
    [Export] public AudioStreamPlayer CombatMusicPlayer { get; private set; }
    [Export] public AudioStreamPlayer CombatMusicLoopPlayer { get; private set; }

    private bool playingCombatMusic = false;
    [Export] private bool stopLoop = false;


    private AudioManager()
    {
        Instance = this;
    }

    public override void _Ready()
    {
        GameManager.GameStateChanged += OnGameStateChanged;
    }

    public override async void _Process(double delta)
    {
        if (playingCombatMusic)
        {
            if (stopLoop) // change to out of combat
            {
                playingCombatMusic = false;
                StopLoopedCombatMusic();
            }
            else if (CombatMusicPlayer.GetPlaybackPosition() >= combatLoopCutoff)
            {
                CombatMusicPlayer.Stop();
                PlayLoopedCombatMusic();
            }
        }

        base._Process(delta);
    }

    private void OnGameStateChanged(GameState oldstate, GameState currentstate)
    {
        if (currentstate == GameState.Battle)
        {
            GD.Print("gameState battle");
            //PlayBackgroundNoise();
            PlayCombatMusic();
        }
    }

    public void PlayBackgroundNoise()
    {
        BrownNoisePlayer.Play();
    }

    public void PlayCombatMusic(float playFrom = -1)
    {
        CombatMusicPlayer.Play();
        playingCombatMusic = true;

        if (playFrom != -1)
        {
            CombatMusicPlayer.Seek(playFrom);
            playingCombatMusic = false;
        }
    }

    public void PlayLoopedCombatMusic()
    {
        CombatMusicLoopPlayer.Play();
        playingCombatMusic = true;
    }

    public void StopLoopedCombatMusic()
    {
        PlayCombatMusic(CombatMusicLoopPlayer.GetPlaybackPosition() + combatLoopCutoff);
        CombatMusicLoopPlayer.Stop();
    }
}
