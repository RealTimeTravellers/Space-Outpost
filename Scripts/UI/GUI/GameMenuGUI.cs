using Godot;

public partial class GameMenuGUI : Control
{
    private Control menuInstance;
    private bool isMenuOpen = false;
    private const float ANIMATION_DURATION = 0.2f;
    private const float MIN_SCALE = 0.8f;
    private const float MAX_SCALE = 1.0f;

    public override void _Ready()
    {
        this.Visible = false;
        this.Scale = Vector2.One * MIN_SCALE;
        this.Modulate = new Color(1, 1, 1, 0); // Başlangıçta tamamen saydam
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_cancel")) // Escape key
        {
            ToggleMenu();
        }
    }

    public void ToggleMenu()
    {
        if (isMenuOpen)
        {
            CloseMenu();
        }
        else
        {
            OpenMenu();
        }
    }

    private void OpenMenu()
    {
        this.Visible = true;
        isMenuOpen = true;
        Raycaster.MouseOverUI = true;

        var tween = CreateTween();
        tween.SetParallel(true);
        tween.SetTrans(Tween.TransitionType.Sine);
        tween.SetEase(Tween.EaseType.Out);

        // Scale animasyonu
        tween.TweenProperty(this, "scale", Vector2.One * MAX_SCALE, ANIMATION_DURATION)
            .From(Vector2.One * MIN_SCALE);

        // Fade in animasyonu
        tween.TweenProperty(this, "modulate:a", 1.0f, ANIMATION_DURATION)
            .From(0.0f);
    }

    private void CloseMenu()
    {
        isMenuOpen = false;
        Raycaster.MouseOverUI = false;

        var tween = CreateTween();
        tween.SetParallel(true);
        tween.SetTrans(Tween.TransitionType.Sine);
        tween.SetEase(Tween.EaseType.In);

        // Scale animasyonu
        tween.TweenProperty(this, "scale", Vector2.One * MIN_SCALE, ANIMATION_DURATION)
            .From(Vector2.One * MAX_SCALE);

        // Fade out animasyonu
        tween.TweenProperty(this, "modulate:a", 0.0f, ANIMATION_DURATION)
            .From(1.0f);

        // Animasyon bitince görünürlüğü kapat
        tween.TweenCallback(Callable.From(() => this.Visible = false))
            .SetDelay(ANIMATION_DURATION);
    }
    public void OnContinueButtonPressed()
    {
        ToggleMenu();
    }

    public void OnOptionsButtonPressed()
    {
        //BattleHUD.ToggleMenu();
    }

    public void OnExitButtonPressed()
    {
        GetTree().Quit();
    }

    public void OnMainMenuButtonPressed()
    {
        GameManager.ChangeGameState(GameState.Battle, GameState.Menu);
        TeamSelectionManager.Instance.ResetParty();
    }
}
