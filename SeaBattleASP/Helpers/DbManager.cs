namespace SeaBattleASP.Helpers
{
    using SeaBattleASP.Models;
    using SeaBattleASP.Models.Enums;
    using System;
    using System.Collections.Generic;

    public static class DbManager
    {
        public static ApplicationContext db;
        public static void SaveShipToDB(Ship ship)
        {
            var shipType = ship.GetType();
            var type = Enum.Parse(typeof(ShipType), shipType.Name);
            switch ((ShipType)type)
            {
                case ShipType.AuxiliaryShip:
                    db.AuxiliaryShips.Add((AuxiliaryShip)ship);
                    break;
                case ShipType.MilitaryShip:
                    db.MilitaryShips.Add((MilitaryShip)ship);
                    break;
                case ShipType.MixShip:
                    db.MixShips.Add((MixShip)ship);
                    break;
            };
        }

        public static void SaveDeckCellAndPlayingFieldToDB(List<DeckCell> shipCoordinates, PlayingField PlayingField)
        {
            foreach (DeckCell deckCell in shipCoordinates)
            {
                db.Cells.Add(deckCell.Cell);
                db.Decks.Add(deckCell.Deck);
                db.DeckCells.Add(deckCell);
                db.SaveChanges();
            }

            db.PlayingField.Add(PlayingField);
            db.SaveChanges();
        }
    }
}
