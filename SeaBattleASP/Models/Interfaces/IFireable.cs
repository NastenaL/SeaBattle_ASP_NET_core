namespace SeaBattleASP.Models.Interfaces
{
    using System.Collections.Generic;

    public interface IFireable
    {
        List<DeckCell> Fire(List<DeckCell> enemyShips);
    }
}
