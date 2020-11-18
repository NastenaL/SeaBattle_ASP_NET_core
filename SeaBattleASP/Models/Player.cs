namespace SeaBattleASP.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using SeaBattleASP.Helpers;

    public class Player
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public static List<Player> GetPlayersNotInGame(MapModel mapModel)
        {
            mapModel.Players = DbManager.GetPlayers();
            var games = DbManager.GetGames();
            List<Player> allplayers = mapModel.Players;
            if (games.Count > 0)
            {
                var busyPLayers = CheckPlayersInNotGame(games, mapModel);
                foreach (var busyPlayer in busyPLayers.ToList())
                {
                    allplayers.Remove(busyPlayer);
                }    
            }

            return allplayers;
        }

        public static List<Player> GetAll()
        {
            return DbManager.GetPlayers();
        }

        private static List<Player> CheckPlayersInNotGame(List<Game> games, MapModel mapModel)
        {
            List<Player> ingame = new List<Player>();
            foreach (Game g in games)
            {
                if (g.Player1 != null && g.Player2 != null)
                {
                    var players1 = mapModel.Players.Where(i => i.Id == g.Player1.Id).ToList();
                    var players2 = mapModel.Players.Where(i => i.Id == g.Player2.Id).ToList();
                    if (players1.Count > 0 && players2.Count > 0)
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