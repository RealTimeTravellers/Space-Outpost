using Godot;
using System;

public partial class Character : CharacterBody3D, ICombat// don't really know why is this character body
{
    public GridObject currentGrid = null;

    [Export] public bool move = false; // temp for test only

    [Export] public int firstRange = 10; // test
    [Export] public int secondRange = 10; // test

    public int Health { get; private set; }
    public int Damage { get ; private set ; }

    public override void _Ready()
    {

        base._Ready();
    }

    public override void _Process(double delta)
    {
        if (move) // test
        {
            move = !move;
            GlobalPosition = GridManager.Instance.selectedGrid.GlobalPosition;
        }
        base._Process(delta);
    }

    private void Die()
    {
        throw new NotImplementedException();
    }

    #region ICombat Implementations
    public void TakeDamage(int damage)
    {
        Health -= damage;

        if (Health <= 0)
            Die();

    }
    #endregion
}
