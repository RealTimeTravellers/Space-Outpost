using Godot;
using System;
using System.Data;
using System.Diagnostics;

public partial class Settings : Node
{
	public static Action GraphicSettingsChanged;
	public static Action MouseSettingsChanged;

	#region Frame Rate variables
	public static bool FpsVisible = true;
	[Export] public bool seeFps = true;
	[Export] public bool vsync = false;
	[Export] public bool adaptive = false;
	[Export] public int desiredFps = 60;

	[Export] public DisplayServer.WindowMode displayMode = DisplayServer.WindowMode.Windowed;
	[Export] public Vector2I windowSize = new(900, 600);
	#endregion

	#region Sound Variables
	[Export] public float masterVolume = 100f;
	[Export] public float effectsVolume = 100f;
	[Export] public float bgmVolume = 100f;
	[Export] public float musicVolume = 100f;
	#endregion

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ConfigHandler.LoadConfigFile();
		SetFpsVisibility();
		SetGraphicSettings();

		GraphicSettingsChanged += OnGraphicSettingsChanged;
		MouseSettingsChanged += OnMouseSettingsChanged;
	}

	private void SetFpsVisibility()
	{
		FpsVisible = seeFps;
	}

	public void SetVsync()
	{
		if(!vsync)
		{
			DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Disabled);
			return;
		}

		if (adaptive)
			DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Adaptive);
		else
			DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Enabled);
	}
	
	private void SetDisplaySettings()
	{
		switch (displayMode)
		{
			case DisplayServer.WindowMode.Fullscreen:
				DisplayServer.WindowSetSize(DisplayServer.WindowGetMaxSize());
				break;
			case DisplayServer.WindowMode.ExclusiveFullscreen:
				DisplayServer.WindowSetSize(DisplayServer.WindowGetMaxSize());
				break;
			default:
				DisplayServer.WindowSetSize(windowSize);
				break;
		}
		DisplayServer.WindowSetMode(displayMode);
	}

	private void SetGraphicSettings()
	{
		desiredFps += 1;
		Engine.MaxFps = desiredFps;

		SetVsync();
		SetDisplaySettings();
	}

	private void SetVolume()
	{
		// TODO: get audiomixer instance and set values to tracks
	}

	private void OnGraphicSettingsChanged()
	{
		SetGraphicSettings();
	}

	private void OnMouseSettingsChanged()
	{
		// TODO: need to make a custom cursor for this
	}
}