using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Kaisa.LeagueApi.Client.BasicApi;

namespace Kaisa.LeagueApi.Client {
    /// <summary>
    /// This class handles any API request and provides curated methods that return data in the form of classes and structs ready to be used.
    /// </summary>
    public class LeagueApi {
        private BasicLeagueApi basicApi;

        /// <summary>
        /// If "useTestSamples" is assigned to true, this instance will be used for testing purposes:
        /// Rather than call to the API, this instance will 'simulate' a call to the API and return
        /// a pre-defined .json file (located in .\res\samples folder). This call will have a delay of 25ms,
        /// simulated possible latency when using the actual API. This is useful to test the validity your code
        /// without an instance of League of Legends actually running in your computer.
        /// </summary>
        /// <param name="useTestSamples">Determines whether this instance of LeagueApiManager is just for testing purposes.</param>
        public LeagueApi(bool useTestSamples = false) {
            //Allow insecure calls since we'll call localhost.
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            basicApi = new BasicLeagueApi(useTestSamples);
        }

        /// <summary>
        /// Returns true if the API is active, which should only happen while the player is in game.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsPlayerInGame() => await basicApi.IsPlayerInGame();

        /// <summary>
        /// Returns a class holding various stats about the active player.
        /// </summary>
        /// <returns></returns>
        public async Task<PlayerStats> GetActivePlayerStats() {
            JToken json = await basicApi.CallApiForJson(Requests.ACTIVE_PLAYER_DATA);

            if (json == null) return null;

            JToken jsonAbilities = json["abilities"];
            JToken jsonChampion = json["championStats"];
            JToken jsonFullRunes = json["fullRunes"];

            Ability[] abilities = ReadAbilitiesFromJson(jsonAbilities);
            Champion champion = ReadFromJson<Champion>(jsonChampion);
            double currentGold = (double)json["currentGold"];
            FullRunes fullRunes = ReadFullRunesFromJson(jsonFullRunes);
            int level = (int)json["level"];
            string summonerName = (string)json["summonerName"];

            return new PlayerStats(abilities, champion, currentGold, fullRunes, level, summonerName);
        }

        /// <summary>
        /// Returns the name of the active player as a string.
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetActivePlayerName() {
            return await basicApi.CallApiForString(Requests.ACTIVE_PLAYER_NAME);
        }

        /// <summary>
        /// Returns information for each ability of the active player as an array.
        /// The order of this array is: passive, Q, W, E, R.
        /// Note that the passive ability always has a level of 0, and no id.
        /// </summary>
        /// <returns></returns>
        public async Task<Ability[]> GetActivePlayerAbilities() {
            JToken json = await basicApi.CallApiForJson(Requests.ACTIVE_PLAYER_ABILITIES);
            return ReadAbilitiesFromJson(json);
        }

        /// <summary>
        /// Returns all the information about the runes of the active player.
        /// This includes: all the regular runes (including the keystone), the keystone,
        /// the primary and secondary rune trees, and the three stat runes.
        /// </summary>
        /// <returns></returns>
        public async Task<FullRunes> GetActivePlayerFullRunes() {
            JToken json = await basicApi.CallApiForJson(Requests.ACTIVE_PLAYER_RUNES);
            return ReadFullRunesFromJson(json);
        }

        /// <summary>
        /// Returns the basic information of every player in the game.
        /// </summary>
        /// <returns></returns>
        public async Task<Player[]> GetAllPlayers() {
            JArray json = (JArray)await basicApi.CallApiForJson(Requests.ALL_PLAYERS_LIST);
            Player[] playerList = new Player[json.Count];

            for (int i = 0; i < playerList.Length; i++) {
                playerList[i] = ReadPlayerFromJson(json[i]);
            }

            return playerList;
        }

        /// <summary>
        /// Returns the basic information of a given player. If no player is found with the name given, it'll return null.
        /// </summary>
        /// <param name="summonerName">The summoner name of the player.</param>
        /// <returns></returns>
        public async Task<Player> GetPlayer(string summonerName) {
            JArray json = (JArray)await basicApi.CallApiForJson(Requests.ALL_PLAYERS_LIST);
            var playerJson =
                from c in json.Children()
                where (string)c["summonerName"] == summonerName
                select (JToken)c;

            if (playerJson.Count() == 0 || playerJson == null) return null;

            return ReadPlayerFromJson(playerJson.ElementAt(0));
        }

        public async Task<Score> GetPlayerScore(string summonerName) {
            JToken json = await basicApi.CallApiForJson(Requests.TARGET_PLAYER_SCORES + summonerName);
            return ReadFromJson<Score>(json);
        }

        public async Task<SummonerSpell[]> GetPlayerSumms(string summonerName) {
            JToken json = await basicApi.CallApiForJson(Requests.TARGET_PLAYER_SUMMS + summonerName);
            return ReadSummsFromJson(json);
        }

        public async Task<BasicRunes> GetPlayerBasicRunes(string summonerName) {
            JToken json = await basicApi.CallApiForJson(Requests.TARGET_PLAYER_BASIC_RUNES + summonerName);
            return ReadBasicRunesFromJson(json);
        }

        public async Task<Item[]> GetPlayerItems(string summonerName) {
            JToken json = await basicApi.CallApiForJson(Requests.TARGET_PLAYER_ITEM_LIST + summonerName);
            return ReadItemsFromJson(json);
        }

        public async Task<LeagueEvent[]> GetEvents() {
            JToken json = await basicApi.CallApiForJson(Requests.EVENT_LIST);
            JArray eventJson = (JArray)json["Events"];
            if (eventJson.Count == 0 || eventJson == null) return null;

            LeagueEvent[] events = new LeagueEvent[eventJson.Count];

            for (int i = 0; i < events.Length; i++) {
                events[i] = ParseEvent(eventJson[i]);
                Console.WriteLine("parse " + i);
            }

            return events;
        }

        public async Task<LeagueEvent> GetLastEvent() {
            JToken json = await basicApi.CallApiForJson(Requests.EVENT_LIST);
            JArray eventJson = (JArray)json["Events"];
            if (eventJson.Count == 0 || eventJson == null) return null;

            return ParseEvent(eventJson[eventJson.Count - 1]);
        }

        public async Task<Game> GetGame() {
            JToken json = await basicApi.CallApiForJson(Requests.GAME_STATS);
            return ReadFromJson<Game>(json);
        }

        public async Task<bool> IsPlayerDead(string summonerName) {
            JArray json = (JArray)await basicApi.CallApiForJson(Requests.ALL_PLAYERS_LIST);
            var playerJson =
                from c in json.Children()
                where (string)c["summonerName"] == summonerName
                select (bool)c["isDead"];

            if (playerJson.Count() == 0 || playerJson == null) return false;

            return playerJson.ElementAt(0);
        }

        public async Task<int> GetActivePlayerLevel() {
            JToken json = await basicApi.CallApiForJson(Requests.ACTIVE_PLAYER_DATA);
            return (int)json["level"];
        }

        private Player ReadPlayerFromJson(JToken json) {
            string summonerName = (string)json["summonerName"];
            string team = (string)json["team"];
            int skinId = (int)json["skinID"];
            string position = (string)json["position"];
            Score score = ReadFromJson<Score>(json["scores"]);

            string championName = (string)json["championName"];
            int level = (int)json["level"];
            Item[] items = ReadItemsFromJson(json["items"]);
            SummonerSpell[] summonerSpells = ReadSummsFromJson(json["summonerSpells"]);
            summonerSpells[0] = ReadFromJson<SummonerSpell>(json["summonerSpells"]["summonerSpellOne"]);
            summonerSpells[1] = ReadFromJson<SummonerSpell>(json["summonerSpells"]["summonerSpellTwo"]);
            BasicRunes basicRunes = ReadBasicRunesFromJson(json["runes"]);

            bool isDead = (bool)json["isDead"];
            double respawnTimer = (double)json["respawnTimer"];
            bool isBot = (bool)json["isBot"];

            string rawChamp = (string)json["rawChampionName"];

            return new Player(
                summonerName, team, skinId, position, score,
                championName, level, items, summonerSpells, basicRunes,
                isDead, respawnTimer, isBot, rawChamp
                );
        }

        private Item[] ReadItemsFromJson(JToken json) {
            if (!(json is JArray jsonArray)) return null;

            Item[] items = new Item[jsonArray.Count];

            for (int i = 0; i < items.Length; i++) {
                items[i] = ReadFromJson<Item>(jsonArray[i]);
            }

            return items;
        }

        private Ability[] ReadAbilitiesFromJson(JToken json) {
            return new Ability[] {
                ReadFromJson<Ability>(json["Passive"]),
                ReadFromJson<Ability>(json["Q"]),
                ReadFromJson<Ability>(json["W"]),
                ReadFromJson<Ability>(json["E"]),
                ReadFromJson<Ability>(json["R"])
            };
        }

        private FullRunes ReadFullRunesFromJson(JToken json) {
            JToken jsonRunes = json["generalRunes"];
            JToken jsonKeystone = json["keystone"];
            JToken jsonPrimaryTree = json["primaryRuneTree"];
            JToken jsonSecondaryTree = json["secondaryRuneTree"];
            JToken jsonStatRunes = json["statRunes"];

            Rune[] runes = ReadFromJson<Rune[]>(jsonRunes);
            Rune keystone = ReadFromJson<Rune>(jsonKeystone);
            RuneTree primaryTree = ReadFromJson<RuneTree>(jsonPrimaryTree);
            RuneTree secondaryTree = ReadFromJson<RuneTree>(jsonSecondaryTree);
            StatRune[] statRunes = ReadFromJson<StatRune[]>(jsonStatRunes);

            return new FullRunes(runes, keystone, primaryTree, secondaryTree, statRunes);
        }

        private BasicRunes ReadBasicRunesFromJson(JToken json) {
            JToken jsonKeystone = json["keystone"];
            JToken jsonPrimaryTree = json["primaryRuneTree"];
            JToken jsonSecondaryTree = json["secondaryRuneTree"];
            Rune keystone = ReadFromJson<Rune>(jsonKeystone);
            RuneTree primaryTree = ReadFromJson<RuneTree>(jsonPrimaryTree);
            RuneTree secondaryTree = ReadFromJson<RuneTree>(jsonSecondaryTree);

            return new BasicRunes(keystone, primaryTree, secondaryTree);
        }

        private SummonerSpell[] ReadSummsFromJson(JToken json) {
            return new SummonerSpell[2] {
                ReadFromJson<SummonerSpell>(json["summonerSpellOne"]),
                ReadFromJson<SummonerSpell>(json["summonerSpellTwo"])
            };
        }

        private LeagueEvent ParseEvent(JToken json) {
            string eventName = (string)json["EventName"];
            switch (eventName) {
                case "GameStart": return ReadFromJson<Event_GameStart>(json);
                case "MinionsSpawning": return ReadFromJson<Event_MinionsFirstSpawn>(json);
                case "FirstBrick": return ReadFromJson<Event_FirstTurretKill>(json);
                case "TurretKilled": return ReadFromJson<Event_TurretKill>(json);
                case "InhibKilled": return ReadFromJson<Event_InhibKill>(json);
                case "DragonKill": return ReadFromJson<Event_DragonKill>(json);
                case "HeraldKill": return ReadFromJson<Event_HeraldKill>(json);
                case "BaronKill": return ReadFromJson<Event_BaronKill>(json);
                case "ChampionKill": return ReadFromJson<Event_ChampionKill>(json);
                case "Multikill": return ReadFromJson<Event_MultiKill>(json);
                case "Ace": return ReadFromJson<Event_Ace>(json);
                case "FirstBlood": return ReadFromJson<Event_FirstBlood>(json);
                case "GameEnd": return ReadFromJson<Event_GameEnd>(json);
                default: return ReadFromJson<LeagueEvent>(json);
            }
        }

        private T ReadFromJson<T>(object json) => ReadFromJson<T>(json.ToString());
        private T ReadFromJson<T>(string json) {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
