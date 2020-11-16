namespace SeaBattleASP.Models
{
    using Microsoft.EntityFrameworkCore;
    using SeaBattleASP.Helpers;
    using System.Collections.Generic;
    using System.Linq;

    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public static List<Player> GetPlayersNotInGame(MapModel Model)
        {
            Model.Players = DbManager.db.Players.ToListAsync<Player>().Result;
            var games = DbManager.db.Games.ToListAsync<Game>().Result;
            List<Player> allplayers = Model.Players;
            if (games.Count > 0)
            {
                var busyPLayers = CheckPlayersInNotGame(games, Model);
                foreach(var busyPlayer in busyPLayers.ToList())
                {
                    allplayers.Remove(busyPlayer);
                }     
            }
            return allplayers;
        }

        public static List<Player> GetAllPlayers()
        {
            return DbManager.db.Players.ToListAsync<Player>().Result;
        }

        private static List<Player> CheckPlayersInNotGame(List<Game> games, MapModel Model)
        {
            List<Player> ingame = new List<Player>();
            foreach (Game g in games)
            {
                if(g.Player1 != null && g.Player2 != null)
                {
                    var players1 = Model.Players.Where(i => i.Id == g.Player1.Id).ToList();
                    var players2 = Model.Players.Where(i => i.Id == g.Player2.Id).ToList();
                    if(players1.Count > 0 && players2.Count > 0)
                    {
                        ingame.AddRange(players1);
                        ingame.AddRange(players2);
                    }
                }
                
            }
            return ingame;
        }
    }
}
