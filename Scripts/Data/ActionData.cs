using System;
using Godot;
public partial class ActionData : Resource
{
    [Export] public int defaultActionPoints = 2;
    [Export] public int moveCost = 1; // take from a resource data
    [Export] public int attackCost = 2; // take from a resource data
    [Export] public int takeCoverCost = 2; // take from a resource data
    [Export] public int standToEngageCost = 2; // take from a resource data
    [Export] public int supressiveFireCost = 2; // take from a resource data

    //[Export] public float StandToEngageRange = 10; // maybe dont know how does the work (Hakan looking at you!)
    
    /// <summary>
    /// From the centre to both sides (because vector math), meaning half FOV of watched area.
    /// Meaning if 10, 20 degrees of coverage.
    /// </summary>
    [Export] public float supressiveFireDegree = 10;

    /// <summary>
    /// Need to have an empty constructor for Godot initialization.
    /// Does not matter what you do inside needs to have no parameters, for a data resource.
    /// </summary>
    public ActionData(){}
}
