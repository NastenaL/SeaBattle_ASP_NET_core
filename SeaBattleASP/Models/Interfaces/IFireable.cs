namespace SeaBattleASP.Models.Interfaces
{
    using System.Collections.Generic;

    public interface IFireable
    {
        void Fire(List<DeckCell> enemyShips, List<DeckCell> selectedShip);
    }
}
