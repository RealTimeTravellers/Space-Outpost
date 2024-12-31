using Godot;
using System;

public partial class MainMenu : Control
{
    [Export] public TextureRect background;  
    private bool isSettingsVisible = false;  
    private const float DURATION = 20.0f;
    
    public override void _Ready()
    {
        CreateBackgroundTween();
		GameManager.Instance.OnSettingsVisibilityChanged += (visible) => isSettingsVisible = visible;
    }

    public override void _ExitTree()
    {
        GameManager.Instance.OnSettingsVisibilityChanged -= (visible) => isSettingsVisible = visible;
    }

    private void CreateBackgroundTween()
    {
        var tween = CreateTween();
        tween.SetLoops();
        tween.SetTrans(Tween.TransitionType.Sine);
        tween.SetEase(Tween.EaseType.InOut);
        
        tween.TweenProperty(background, "position", new Vector2(background.Position.X +30, background.Position.Y), DURATION);
        tween.TweenProperty(background, "position", new Vector2(background.Position.X -30, background.Position.Y), DURATION);
    }

    public void OnOptionsPressed()
    {
        isSettingsVisible = !isSettingsVisible;
        GameManager.Instance.ToggleSettingsPanel(isSettingsVisible);
    }

    public void OnNewGamePressed()
    {
        GameManager.ChangeGameState(GameState.Menu, GameState.TeamSelect);
    }

    public void OnExitPressed()
    {
        GetTree().Quit();
    }
}