namespace SeaBattleASP.Models.Constants
{
    using System.Collections.Generic;
    using System;
    using SeaBattleASP.Models.Enums;

    public static class Rules
    {
        public static int FieldHeight = 10;
        public static int FieldWidth = 10;

        public static  Ship CreateShip(int range, ShipType type)
        {
            Ship ship = null;
            switch (type)
            {
                case ShipType.AuxiliaryShip:
                    ship = new AuxiliaryShip();
                    break;
                case ShipType.MilitaryShip:
                    ship = new MilitaryShip();
                    break;
                case ShipType.MixShip:
                    ship = new MixShip();
                    break;
            };
            
            List<DeckCell> decks = new List<DeckCell>();

            for (int i = 0; i < range; i++)
            {
                DeckCell d = new DeckCell
                {
                    Deck = new Deck(Enums.DeckState.Normal, decks.Count == 0)
                };
                decks.Add(d);
            }

            ship.Range = range;
            ship.DeckCells = decks;
         
            ship.IsXDirection = GenerateDirection();
            return ship;
        }

        private static bool GenerateDirection()
        {
            Random random = new Random();
            int direction = random.Next(100);

            return direction <= 49;
        }

        public static Dictionary<int, Ship> CreateShips()
        {
            var a = CreateShip(2, ShipType.AuxiliaryShip);
            var b = CreateShip(3, ShipType.MilitaryShip);
            var c = CreateShip(3, ShipType.MixShip);
            var d = CreateShip(2, ShipType.AuxiliaryShip);
           
           List<Ship> ships = new List<Ship>
           {
                a,
                b,
                c,
                d
           };

            Dictionary<int, Ship> defaultShips = new Dictionary<int, Ship>();
            for(int i = 0; i < ships.Count; i++)
            {
                defaultShips.Add(i, ships[i]);
            }
            return defaultShips;
        } 
    }
}
