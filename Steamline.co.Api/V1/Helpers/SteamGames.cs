using Newtonsoft.Json;
using Steamline.co.Api.V1.Models.SteamApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Steamline.co.Api.V1.Helpers
{
    public class SteamGames
    {
        public static string SteamGamesString = "";

        private static Dictionary<long, GameDetails> gameDetails;
        public static Dictionary<long, GameDetails> GameDetails
        {
            get
            {
                if (gameDetails == null)
                    gameDetails = JsonConvert.DeserializeObject<List<GameDetails>>(SteamGamesString).Where(g => g != null).ToDictionary(g => g.Id);

                return gameDetails;
            }
        }
    }
}
