using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaisa.LeagueApi.Client {
    /// <summary>
    /// This class holds all the possible requests that can be made to RIOT's api.
    /// Requests starting with "TARGET_" must be provided a valid summoner name immediately afterwards.
    /// </summary>
    public static class Requests {
        public const string OPEN_API = "https://127.0.0.1:2999/swagger/v3/openapi.json";
        public const string SWAGGER = "https://127.0.0.1:2999/swagger/v2/swagger.json";
        public const string ALL_GAME_DATA = "https://127.0.0.1:2999/liveclientdata/allgamedata";
        public const string ACTIVE_PLAYER_DATA = "https://127.0.0.1:2999/liveclientdata/activeplayer";
        public const string ACTIVE_PLAYER_NAME = "https://127.0.0.1:2999/liveclientdata/activeplayername";
        public const string ACTIVE_PLAYER_ABILITIES = "https://127.0.0.1:2999/liveclientdata/activeplayerabilities";
        public const string ACTIVE_PLAYER_RUNES = "https://127.0.0.1:2999/liveclientdata/activeplayerrunes";
        public const string ALL_PLAYERS_LIST = "https://127.0.0.1:2999/liveclientdata/playerlist";
        public const string TARGET_PLAYER_SCORES = "https://127.0.0.1:2999/liveclientdata/playerscores?summonerName=";
        public const string TARGET_PLAYER_SUMMS = "https://127.0.0.1:2999/liveclientdata/playersummonerspells?summonerName=";
        public const string TARGET_PLAYER_BASIC_RUNES = "https://127.0.0.1:2999/liveclientdata/playermainrunes?summonerName=";
        public const string TARGET_PLAYER_ITEM_LIST = "https://127.0.0.1:2999/liveclientdata/playeritems?summonerName=";
        public const string EVENT_LIST = "https://127.0.0.1:2999/liveclientdata/eventdata";
        public const string GAME_STATS = "https://127.0.0.1:2999/liveclientdata/gamestats";
    }
}
