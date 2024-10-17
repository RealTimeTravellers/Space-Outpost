using Godot;
using System;

public partial class TestCharacter : CharacterBody3D
{
    private Material _meshMaterial;
    private bool _isSelected = false;

    public override void _Ready()
    {
        InputEvent += OnInputEvent;
        _meshMaterial = GetNode<MeshInstance3D>("Pivot/CapsuleMesh").GetActiveMaterial(0);
    }

    private void OnInputEvent(Node camera, InputEvent @event, Vector3 eventposition, Vector3 normal, long shapeidx)
    {
        if (@event.IsActionReleased("Select"))
        {
            _isSelected = !_isSelected;
            if (_meshMaterial is StandardMaterial3D standardMaterial)
            {
                standardMaterial.AlbedoColor = _isSelected ? Colors.Red : Colors.White;
            }
        }
    }
}
