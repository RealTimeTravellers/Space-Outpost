using Godot;
using System;

public static class ConfigHandler
{
	private static ConfigFile Config = new ConfigFile();
	private const string userPrefix = "user://";
	private const string configFileName = "settings.cfg";

	#region Section Strings
	private const string GraphicsSection = "Graphics";
	private const string VolumeSection = "Volume";
	#endregion

	#region Keys
	private const string FpsVisible = "FpsVisible";
	private const string Vsync = "Vsync";
	private const string Adaptive = "Adaptive";
	private const string DesiredFps = "DesiredFps";
	private const string DisplayMode = "DisplayMode";
	private const string WindowSize = "WindowSize";

	private const string MasterVolume = "MasterVolume";
	private const string EffectsVolume = "EffectsVolume";
	private const string BgmVolume = "BgmVolume";
	private const string MusicVolume = "MusicVolume";
	#endregion

	public static void LoadConfigFile()
	{
		if(Config.Load(userPrefix + configFileName) != Error.Ok)
		{
			Settings settings = GameManager.Instance.settings;

			// === Graphical Methods ===
			SetFpsVisibility(settings.seeFps);
			SetVsync(settings.vsync, settings.adaptive);
			SetDesiredFps(settings.desiredFps);
			SetDisplayMode(settings.displayMode);
			SetWindowSize(settings.windowSize);

			// === Volume Methods ===
			SetMasterVolume(settings.masterVolume);
			SetEffectsVolume(settings.effectsVolume);
			SetBgmVolume(settings.bgmVolume);
			SetMusicVolume(settings.musicVolume);
		
			SaveConfig();
		}
		else
			LoadConfig();
	}

	private static void SaveConfig()
	{
		// Config saving here
		Config.Save(userPrefix + configFileName);
		GD.Print("Saving Config To: " + OS.GetUserDataDir() + "/" + configFileName);
	}

	private static void LoadConfig()
	{
		GD.Print("Loading Data From Here: " + OS.GetUserDataDir() + "/" + configFileName);

		// Call Load Methods Here
		Settings settings = GameManager.Instance.settings;
		
		// === Graphical Methods ===
		settings.seeFps = GetFpsVisibility();
		(settings.vsync, settings.adaptive) = GetVsync();
		settings.desiredFps = GetDesiredFps();
		settings.displayMode = GetDisplayMode();
		settings.windowSize = GetWindowSize();

		// === Volume Methods ===
		settings.masterVolume = GetMasterVolume();
		settings.effectsVolume = GetEffectsVolume();
		settings.bgmVolume = GetBgmVolume();
		settings.musicVolume = GetMusicVolume();
	}

	#region Setting preferences Methods
	// === GRAPHICAL SETTINGS ===
	public static void SetFpsVisibility(bool visibility)
	{
		Config.SetValue(GraphicsSection, FpsVisible, visibility);
	}
	public static bool GetFpsVisibility()
	{
		if (Config.HasSectionKey(GraphicsSection, FpsVisible))
		{
			return (bool) Config.GetValue(GraphicsSection, FpsVisible);
		}
		else
		{
			SetFpsVisibility(false);
			return false;
		}
	}

	public static void SetVsync(bool vsync, bool adaptive)
	{
		Config.SetValue(GraphicsSection, Vsync, vsync);
		Config.SetValue(GraphicsSection, Adaptive, adaptive);
	}
	public static (bool, bool) GetVsync()
	{
		bool vsync;
		bool adaptive;

		if (!Config.HasSectionKey(GraphicsSection, Vsync) || !Config.HasSectionKey(GraphicsSection, Adaptive))
		{
			SetVsync(false, false);
			vsync = false;
			adaptive = false;
		}
		else
		{
			vsync = (bool) Config.GetValue(GraphicsSection, Vsync);
			adaptive = (bool) Config.GetValue(GraphicsSection, Adaptive);
		}
	
		return (vsync, adaptive);
	}
 
	public static void SetDesiredFps(int targetFps)
	{
		Config.SetValue(GraphicsSection, DesiredFps, targetFps);
	}
	public static int GetDesiredFps()
	{
		if (Config.HasSectionKey(GraphicsSection, DesiredFps))
			return (int) Config.GetValue(GraphicsSection, DesiredFps);
		else
		{
			SetDesiredFps(60);
			return 60;
		}
	}

	public static void SetDisplayMode(DisplayServer.WindowMode mode)
	{
		// Use int for enums
		Config.SetValue(GraphicsSection, DisplayMode, (int) mode);
	}
	public static DisplayServer.WindowMode GetDisplayMode()
	{
		if (Config.HasSectionKey(GraphicsSection, DisplayMode))
			return (DisplayServer.WindowMode) (int) Config.GetValue(GraphicsSection, DisplayMode);
		else
		{
			SetDisplayMode(DisplayServer.WindowMode.Windowed);
			return DisplayServer.WindowMode.Windowed;
		}
	}

	public static void SetWindowSize(Vector2I size)
	{
		Config.SetValue(GraphicsSection, WindowSize, size);
	}
	public static Vector2I GetWindowSize()
	{
		if(Config.HasSectionKey(GraphicsSection, WindowSize))
			return (Vector2I) Config.GetValue(GraphicsSection, WindowSize);
		else
		{
			SetWindowSize(new Vector2I(900, 600));
			return new Vector2I(900, 600);
		}
	}

	// === VOLUME SETTINGS ===
	public static void SetMasterVolume(float volume)
	{
		Config.SetValue(VolumeSection, MasterVolume, volume);
	}
	public static float GetMasterVolume()
	{
		if (Config.HasSectionKey(VolumeSection, MasterVolume))
			return (float) Config.GetValue(VolumeSection, MasterVolume);
		else
		{
			SetMasterVolume(100f);
			return 100f;
		}
	}
	
	public static void SetEffectsVolume(float volume)
	{
		Config.SetValue(VolumeSection, EffectsVolume, volume);
	}
	public static float GetEffectsVolume()
	{
		if (Config.HasSectionKey(VolumeSection, EffectsVolume))
			return (float) Config.GetValue(VolumeSection, EffectsVolume);
		else
		{
			SetEffectsVolume(100f);
			return 100f;
		}
	}

	public static void SetBgmVolume(float volume)
	{
		Config.SetValue(VolumeSection, BgmVolume, volume);
	}
	public static float GetBgmVolume()
	{
		if (Config.HasSectionKey(VolumeSection, BgmVolume))
			return (float) Config.GetValue(VolumeSection, BgmVolume);
		else
		{
			SetBgmVolume(100f);
			return 100f;
		}
	}

	public static void SetMusicVolume(float volume)
	{
		Config.SetValue(VolumeSection, MusicVolume, volume);
	}
	public static float GetMusicVolume()
	{
		if (Config.HasSectionKey(VolumeSection, MusicVolume))
			return (float) Config.GetValue(VolumeSection, MusicVolume);
		else
		{
			SetMusicVolume(100f);
			return 100f;
		}
	}
	#endregion
}