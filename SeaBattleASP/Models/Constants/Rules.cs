namespace SeaBattleASP.Models.Constants
{
    using SeaBattleASP.Models.Enums;
    using System;
    using System.Collections.Generic;
    using System.Drawing;

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
                    ship = new AuxiliaryShip
                    {
                        ShipType = ShipType.AuxiliaryShip
                    };
                    break;
                case ShipType.MilitaryShip:
                    ship = new MilitaryShip
                    {
                        ShipType = ShipType.MilitaryShip
                    };
                    break;
                case ShipType.MixShip:
                    ship = new MixShip
                    {
                        ShipType = ShipType.MixShip
                    };
                    break;
            };
            
            List<DeckCell> decks = new List<DeckCell>();

            for (int i = 0; i < range; i++)
            {
                DeckCell d = new DeckCell
                {
                    Deck = new Deck(Enums.DeckState.Normal, decks.Count == 0 ? true : false)
                };
                decks.Add(d);
            }

            ship.Range = range;
            ship.DeckCells = decks;
         
            ship.Direction = GenerateDirection();
            return ship;
        }

        private static Point GenerateDirection()
        {
            Point shipDirection = new Point();
            Random random = new Random();
            int x = random.Next(0, 1);

            int y = x == 0 ? 1 : 0;
            shipDirection.X = x;
            shipDirection.Y = y;
            if(shipDirection.X == 0 && shipDirection.Y == 0)
            {
                GenerateDirection();
            }

            return shipDirection;
        }

        public static Dictionary<int, Ship> CreateShips()
        {
            var a = CreateShip(4, ShipType.AuxiliaryShip);
            var b = CreateShip(3, ShipType.MilitaryShip);
            var c = CreateShip(3, ShipType.MixShip);
            var d = CreateShip(1, ShipType.AuxiliaryShip);
           
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
