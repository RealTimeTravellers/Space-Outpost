using Godot;
using Microsoft.VisualBasic;
using System;
using System.Runtime.Serialization;

public partial class GridObject : Node3D
{
    [Export] private CoverType coverType = CoverType.None;
    [Export] public Godot.Collections.Array<bool> coverDirection = new() { false, false, false, false };
    
    [Export] public Material standardMaterial;
    [Export] public Material selectedMaterial;
    [Export] public Material innerMaterial;
    [Export] public Material outerMaterial;
    [Export] public Material blockedMaterial;

    [Export] private float standardTransparency = 0.75f;
    [Export] private float selectedTransparency = 0.25f;

    [Export] private MeshInstance3D meshInstance;

    public override void _Ready()
    {
        SubscribeToEvents();
        meshInstance.SetSurfaceOverrideMaterial(0, standardMaterial);
        base._Ready();
    }

    private void SubscribeToEvents()
    {
        GridManager.Instance.SelectionChanged += OnSelectionChanged;
    }

    private void ChangeGridMaterial(Material mat, float transparency)
    {
        meshInstance.SetSurfaceOverrideMaterial(0, mat);
        meshInstance.Transparency = transparency;
    }

    private void OnSelectionChanged(GridObject gridObject)
    {
        // TODO: if selected colour it as selected (Change Material on Geometry3D)
        if (gridObject == null)
        {   
            ChangeGridMaterial(standardMaterial, standardTransparency);
            return;
        }

        if (gridObject.GetInstanceId() == this.GetInstanceId()) // since I do a null check above, this is a little faster
            ChangeGridMaterial(selectedMaterial, selectedTransparency);
        else
        {
            // TODO: if this is not selected change colour based on range or enemy presence
            if (GridManager.Instance.selectedCharacter != null)
            {
                var character = GridManager.Instance.selectedCharacter;
                if (this.Position.DistanceTo(character.Position) < character.firstRange)
                    ChangeGridMaterial(innerMaterial, standardTransparency);
                else if (this.Position.DistanceTo(character.Position) < character.firstRange + character.secondRange)
                    ChangeGridMaterial(outerMaterial, standardTransparency);
                else
                    ChangeGridMaterial(standardMaterial, 1f);
            }
        }
    }
}
