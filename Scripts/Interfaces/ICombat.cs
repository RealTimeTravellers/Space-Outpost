using System.Collections.Generic;
using Godot;

public interface ICombat
{
    public bool Friendly {get;}
    public int Health {get;}
    public int Damage {get;}

    // methods
    public Godot.Collections.Array<Character> QueryForEnemies(Godot.Collections.Array<Character> enemies);
    public void Attack(Character enemy, float chance);
    public void TakeDamage(int damage);
}
