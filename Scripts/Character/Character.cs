using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Character : CharacterBody3D, ICombat// don't really know why is this character body
{
    public GridObject currentGrid = null;

    [Export] public bool move = false; // temp for test only

    [Export] public int firstRange = 10; // test
    [Export] public int secondRange = 10; // test

    [Export] public float range = 25; // test

    // More of an idea, make the non identified chracters show up but black
    // only meaning full if there are civilians in the combat zone
    [Export] public float visualRange = 35;

    public int Health { get; private set; }
    public int Damage { get ; private set ; }

    public override void _Ready()
    {
        // read data here?
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
    // sub this at turn ends or enemy movements
    public List<Character> QueryForEnemies(Godot.Collections.Array enemies)
    {
        List<Character> enemiesWithLos = new();

        foreach (Character enemy in enemies.Select(v => (Character)v))
        {
            if (enemy.Position.DistanceTo(this.Position) < range) // is in identification range
            {
                CastHit hit = PhysicsCasts.CastLine(this, enemy.Position, this.Position, PhysicsCasts.GetCollisionMask(10), true); // Make enemy 10
                
                if (hit.NonEmpty)
                    enemiesWithLos.Add(enemy);
            }
        }
        return enemiesWithLos;
    }

    public void Attack(ICombat enemy, int chance)
    {
        throw new NotImplementedException();
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;

        if (Health <= 0)
            Die();

    }
    #endregion
}
