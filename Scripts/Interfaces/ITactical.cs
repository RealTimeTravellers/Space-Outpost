
public interface ITactical
{
    /// <summary>
    /// Movement, from current grid to target grid
    /// </summary>
    public void Move(GridObject targetGrid);

    /// <summary>
    /// Keeps their head down, reduces change of being hit
    /// </summary>
    public void TakeCover();

    /// <summary>
    /// Overwatch, stays in position and engeages once if enemy moves
    /// </summary>
    public void StandToEngage();

    /// <summary>
    /// Fires at every moving enemy in a limited field of view, every shot uses ammo, if out of ammo this action is ceased.
    /// </summary>
    public void SupressiveFire();
}
