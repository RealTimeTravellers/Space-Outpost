using System.Collections.Generic;
using Godot;

public interface ICombat
{
    public int Health {get;}
    public int Damage {get;}

    public List<Character>QueryForEnemies();
    public void TakeDamage(int damage);
}
