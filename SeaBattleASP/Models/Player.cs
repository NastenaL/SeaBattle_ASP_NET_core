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
            List<Player> ingame = new List<Player>();
            if (games.Count > 0)
            {
                ingame = CheckPlayersInNotGame(games, Model);
            }
            return ingame;
        }

        private static List<Player> CheckPlayersInNotGame(List<Game> games, MapModel Model)
        {
            List<Player> ingame = new List<Player>();
            foreach (Game g in games)
            {
                if(g.Player1 != null && g.Player2 != null)
                {
                    var players = Model.Players.Where(i => i.Id != g.Player1.Id || i.Id != g.Player2.Id).ToList();
                    if (players.Count > 0)
                    {
                        ingame.AddRange(players);
                    }
                }
                
            }
            return ingame;
        }
    }
}
