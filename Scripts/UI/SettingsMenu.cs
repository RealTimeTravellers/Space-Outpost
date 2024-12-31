

using Godot;
using System;

public partial class SettingsMenu : Control
{
    private readonly Vector2I[] resolutions = new[]
    {
        new Vector2I(1920, 1080),
        new Vector2I(1600, 900),
        new Vector2I(1366, 768),
        new Vector2I(1280, 720),
        new Vector2I(900, 600)
    };

    private Settings settings => GameManager.Instance.settings;
    [Export] public ScrollContainer graphicsContainer;
    [Export] public ScrollContainer soundContainer;
    [Export] public ScrollContainer mouseContainer;
    [Export] public OptionButton displayMode;
    [Export] public OptionButton resolutionButton;
    [Signal] public delegate void BackPressedEventHandler();
    public event Action<bool> OnSettingsVisibilityChanged;

    private ScrollContainer currentActiveSettings;

    public override void _Ready()
    {     
        graphicsContainer.Visible = false;
        soundContainer.Visible = false;
        mouseContainer.Visible = false;

        displayMode.Selected = (int)settings.displayMode;
        displayMode.ItemSelected += OnDisplayModeSelected;

        foreach (var resolution in resolutions)
            resolutionButton.AddItem($"{resolution.X}x{resolution.Y}");
        
        int currentIndex = Array.FindIndex(resolutions, r => r == settings.windowSize);
        if (currentIndex != -1)
            resolutionButton.Selected = currentIndex;
            
        resolutionButton.ItemSelected += OnWindowSizeChanged;
    }

    public void OnBackButtonPressed()
    {
        GameManager.Instance.ToggleSettingsPanel(false);
    }

    public void OnGraphicsTogglePressed()
    {
        ToggleSettingsPanel(graphicsContainer);
    }

    public void OnSoundTogglePressed()
    {
        ToggleSettingsPanel(soundContainer);
    }

    public void OnInputTogglePressed()
    {
        ToggleSettingsPanel(mouseContainer);
    }

    private void ToggleSettingsPanel(ScrollContainer newPanel)
    {
        // Eğer aynı panele tıklandıysa bir şey yapma
        if (currentActiveSettings == newPanel)
            return;

        // Aktif paneli gizle
        if (currentActiveSettings != null)
            currentActiveSettings.Visible = false;

        // Yeni paneli göster
        newPanel.Visible = true;
        currentActiveSettings = newPanel;
    }

    #region Graphics Settings
    public void OnFpsVisibilityChanged(bool visible)
    {
        settings.seeFps = visible;
        Settings.GraphicSettingsChanged?.Invoke();
    }

    public void OnVsyncChanged(bool enabled)
    {
        settings.vsync = enabled;
        Settings.GraphicSettingsChanged?.Invoke();
    }

    public void OnAdaptiveVsyncChanged(bool enabled)
    {
        settings.adaptive = enabled;
        Settings.GraphicSettingsChanged?.Invoke();
    }

    public void OnDesiredFpsChanged(int value)
    {
        settings.desiredFps = value;
        Settings.GraphicSettingsChanged?.Invoke();
    }

    private void OnDisplayModeSelected(long index)
    {
        DisplayServer.WindowMode selectedMode = (DisplayServer.WindowMode)index;
        settings.displayMode = selectedMode;
        Settings.GraphicSettingsChanged?.Invoke();
    }

    public void OnWindowSizeChanged(long index)
    {
        settings.windowSize = resolutions[index];
        Settings.GraphicSettingsChanged?.Invoke();
    }
    #endregion

    #region Sound Settings
    public void OnMasterVolumeChanged(float value)
    {
        settings.masterVolume = value;
        // Ses ayarları için özel bir event eklenebilir
    }

    public void OnEffectsVolumeChanged(float value)
    {
        settings.effectsVolume = value;
        // Ses ayarları için özel bir event eklenebilir
    }

    public void OnBGMVolumeChanged(float value)
    {
        settings.bgmVolume = value;
        // Ses ayarları için özel bir event eklenebilir
    }

    public void OnMusicVolumeChanged(float value)
    {
        settings.musicVolume = value;
        // Ses ayarları için özel bir event eklenebilir
    }
    #endregion

    #region Mouse Settings
    public void OnMouseSettingsChanged()
    {
        Settings.MouseSettingsChanged?.Invoke();
    }
    #endregion
}

