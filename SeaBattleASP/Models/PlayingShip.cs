using SeaBattleASP.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeaBattleASP.Models
{
    public class PlayingShip
    {
        public int Id { get; set; }
        public Ship Ship { get; set; }
        public ShipType ShipType { get; set; }
    }
}
