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
                case ShipType.Auxiliary:
                    ship = new AuxiliaryShip();
                    break;
                case ShipType.Military:
                    ship = new MilitaryShip();
                    break;
                case ShipType.Mix:
                    ship = new MixShip();
                    break;
            };
            
            List<Deck> decks = new List<Deck>();

            for (int i = 0; i < range; i++)
            {
                decks.Add(new Deck( decks.Count+1, Enums.DeckState.Normal, decks.Count == 0? true:false));
            }

            ship.Range = range;
            ship.Decks = decks;
         
            ship.Direction = GenerateDirection(range);
            return ship;
        }

        private static Point GenerateDirection(int range)
        {
            Point shipDirection = new Point();
            Random random = new Random();
            int x = random.Next(0, range);

            int y = x == 0 ? random.Next(0, range) : 0;
            shipDirection.X = x;
            shipDirection.Y = y;
            if(shipDirection.X == 0 && shipDirection.Y == 0)
            {
                GenerateDirection(range);
            }

            return shipDirection;
        }

        public static List<Ship> CreateShips()
        {
           var a = CreateShip(4, ShipType.Auxiliary);
           a.Id = 1;
           var b = CreateShip(3, ShipType.Military);
           b.Id = 2;
           var c = CreateShip(3, ShipType.Mix);
           c.Id = 3;
            List<Ship> ships = new List<Ship>
            {
                a,
                b,
                c
            };
            return ships;
        } 
    }
}
