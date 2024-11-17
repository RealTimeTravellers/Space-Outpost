using System.Collections.Generic;
using Godot;

public interface ICombat
{
    public int Health {get;}
    public int Damage {get;}

    // methods
    public Godot.Collections.Array<Character> QueryForEnemies(Godot.Collections.Array<Character> enemies);
    public void Attack(ICombat enemy, float chance);
    public void TakeDamage(int damage);
}
