using System.Diagnostics;
using Godot;

public partial class AudioManager : Node
{
    public static AudioManager Instance { get; private set; }

    [Export] public AudioStreamPlayer BrownNoisePlayer { get; private set; }
    [Export] public AudioStreamPlayer MenuMusicPlayer { get; private set; }

    [Export] public float combatLoopCutoff = 129.7f;
    [Export] public AudioStreamPlayer CombatMusicPlayer { get; private set; }
    [Export] public AudioStreamPlayer CombatMusicLoopPlayer { get; private set; }

    private bool playingCombatMusic = false;
    [Export] public bool combatEnded = false; // mostly for test


    private AudioManager()
    {
        Instance = this;
    }

    public override void _Ready()
    {
        GameManager.GameStateChanged += OnGameStateChanged;

        // this is for if gameStateChanged event fires before subbing
        if (GameManager.Instance.gameState == GameState.Menu)
            PlayMenuMusic();
    }

    public override async void _Process(double delta)
    {
        if (playingCombatMusic)
        {
            if (combatEnded) // change to out of combat
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
            PlayCombatMusic();
        else if (currentstate == GameState.Menu)
            PlayMenuMusic();

    }

    public void PlayBackgroundNoise()
    {
        BrownNoisePlayer.Play();
    }

    public void PlayMenuMusic()
    {
        BrownNoisePlayer.Stop();
        CombatMusicPlayer.Stop();
        CombatMusicLoopPlayer.Stop();

        MenuMusicPlayer.Play();
    }

    public void PlayCombatMusic(float playFrom = -1)
    {
        MenuMusicPlayer.Stop();

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
