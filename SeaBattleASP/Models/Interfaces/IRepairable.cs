namespace SeaBattleASP.Models.Interfaces
{
    using System.Collections.Generic;
    public interface IRepairable
    {
        List<DeckCell> Repair(List<Ship> allShips);
    }
}
