using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

public interface ICombat
{
    public bool IsFriendly {get;}
    public int Health {get;}
    public int Damage {get;}

    // methods
    public Godot.Collections.Array<Character> QueryForEnemies(Godot.Collections.Array<Character> enemies, bool limitedFov = false);
    public Task Attack(Character enemy);
    public void TakeDamage(int damage);
}
