namespace SeaBattleASP.Models.Interfaces
{
    using System.Collections.Generic;
    public interface IRepairable
    {
        void Repair(Ship ship, List<Ship> allShips);
    }
}
