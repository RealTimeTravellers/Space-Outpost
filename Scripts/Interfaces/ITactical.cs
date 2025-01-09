using System.Threading.Tasks;

public interface ITactical
{
    public bool IsTakingCover {get;}

    /// <summary>
    /// Movement, from current grid to target grid
    /// </summary>
    public Task Move(GridObject targetGrid);

    /// <summary>
    /// Attack a target
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public Task Attack(Character target);

    /// <summary>
    /// Keeps their head down, reduces change of being hit
    /// </summary>
    public void TakeCover(bool enterCover = true);

    /// <summary>
    /// Overwatch, stays in position and engeages once if enemy moves
    /// </summary>
    public void StandToEngage();

    /// <summary>
    /// Fires at every moving enemy in a limited field of view, empties the magazine.
    /// </summary>
    public Task SuppressiveFire(Character target);
}
