using Godot;
using System.Collections.Generic;

public partial class LoggingPanelHUD : Control
{
    [Export] private VBoxContainer logContainer;
    private const int MAX_LOG_ENTRIES = 8;
    
    public override void _Ready()
    {
        //this.SelfModulate = new Color(1, 1, 1, 0);
    }

    public void AddLogEntry(string message, Color color)
    {
        var logEntry = new Label
        {
            Text = message,
            Modulate = color,
            AutowrapMode = TextServer.AutowrapMode.Word
        };
        
        logContainer.AddChild(logEntry);
        
        if (logContainer.GetChildCount() > MAX_LOG_ENTRIES)
        {
            logContainer.GetChild(0).QueueFree();
        }
    }

    public void OnMouseEntered()
    {
        this.SelfModulate = new Color(1, 1, 1, 1);
        Raycaster.MouseOverUI = true;
    }

    public void OnMouseExited()
    {
        this.SelfModulate = new Color(1, 1, 1, 0);
        Raycaster.MouseOverUI = false;
    }
}