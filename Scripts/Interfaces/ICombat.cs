using System.Collections.Generic;
using Godot;

public interface ICombat
{
    public int Health {get;}
    public int Damage {get;}

    // methods
    public List<Character> QueryForEnemies(Godot.Collections.Array enemies);
    public void Attack(ICombat enemy, int chance);
    public void TakeDamage(int damage);
}
