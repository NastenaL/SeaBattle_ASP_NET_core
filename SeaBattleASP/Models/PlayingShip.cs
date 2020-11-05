namespace SeaBattleASP.Models
{
    using SeaBattleASP.Models.Enums;
    using System;

    public class PlayingShip
    {
        public int Id { get; set; }

        public Ship Ship { get; set; }

        public ShipType ShipType { get; set; }

        public static PlayingShip Create(Ship ship)
        {
            var shipType = ship.GetType();
            var type = Enum.Parse(typeof(ShipType), shipType.Name);
            PlayingShip playingShip = new PlayingShip
            {
                Ship = ship,
                ShipType = (ShipType)type
            };
            return playingShip;
        }
    }
}
