namespace SeaBattleASP.Models
{
    using System;
    using System.Collections.Generic;

    public class MilitaryShip : Ship
    {
        public override void Fire()
        {
  
        }

        public override void Repair(List<DeckCell> shipDecks)
        {
            throw new NotImplementedException();
        }
    }
}
