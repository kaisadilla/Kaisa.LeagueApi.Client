using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Kaisa.LeagueApi.Client.BasicApi {
    /// <summary>
    /// This class contains direct calls to the API and returns raw data.
    /// Constants for each possible url are contained in the <see cref="Requests"/> class.
    /// </summary>
    class BasicLeagueApi {
        private readonly bool useTestSamples;

        /// <summary>
        /// If "useTestSamples" is assigned to true, this instance will be used for testing purposes:
        /// Rather than call to the API, this instance will 'simulate' a call to the API and return
        /// a pre-defined .json file (located in .\res\samples folder). This call will have a delay of 25ms,
        /// simulated possible latency when using the actual API. This is useful to test the validity your code
        /// without an instance of League of Legends actually running in your computer.
        /// </summary>
        /// <param name="useTestSamples">Determines whether this instance of LeagueApiManager is just for testing purposes.</param>
        public BasicLeagueApi(bool useTestSamples = false) {
            this.useTestSamples = useTestSamples;
        }

        /// <summary>
        /// Returns true if the API is active, which should only happen while the player is in game.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsPlayerInGame() {
            WebRequest request = WebRequest.Create(Requests.ACTIVE_PLAYER_NAME);
            try {
                using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync()) {
                    return response.StatusCode == HttpStatusCode.OK;
                }
            }
            catch (WebException ex) {
                return false;
            }
        }

        /// <summary>
        /// Sends a request to the api and returns a string containing the result. If the request fails, the string will be empty.
        /// For most purposes, consider using GetJsonData() to get a JObject version of the result instead.
        /// </summary>
        /// <param name="url">The request being sent.</param>
        /// <returns></returns>
        public async Task<string> CallApiForString(string url) {
            string jsonString = "";

            if (!useTestSamples) {
                WebRequest request = WebRequest.Create(url);
                try {
                    using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
                    using (Stream stream = response.GetResponseStream())
                    using (StreamReader reader = new StreamReader(stream)) {
                        jsonString = reader.ReadToEnd();
                    }
                }
                catch (Exception ex) {
                    Console.WriteLine("Unhandled exception!");
                    return "";
                }
            }
            else {
                await Task.Delay(25);
                return File.ReadAllText(GetTestSamplePath(url));
            }

            return jsonString;
        }

        /// <summary>
        /// Sends a request to the api and returns the result as a JObject. If the request fails, it'll return null.
        /// </summary>
        /// <param name="url">The request being sent.</param>
        /// <returns></returns>
        public async Task<JToken> CallApiForJson(string url) {
            string dataString = await CallApiForString(url);

            try {
                JToken json = JToken.Parse(dataString);
                return json;
            }
            catch (JsonException ex) {
                Console.WriteLine("An exception occurred when trying to parse the response.");
                return null;
            }
        }
        /// <summary>
        /// Transforms the url request into the local path of the sample json file.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private string GetTestSamplePath(string url) {
            switch (url) {
                case "https://127.0.0.1:2999/swagger/v3/openapi.json":
                    return @".\res\samples\openapi.json";
                case "https://127.0.0.1:2999/swagger/v2/swagger.json":
                    return @".\res\samples\swagger.json";
                case "https://127.0.0.1:2999/liveclientdata/allgamedata":
                    return @".\res\samples\allgamedata.json";
                case "https://127.0.0.1:2999/liveclientdata/activeplayer":
                    return @".\res\samples\activeplayer.json";
                case "https://127.0.0.1:2999/liveclientdata/activeplayername":
                    return @".\res\samples\activeplayername.json";
                case "https://127.0.0.1:2999/liveclientdata/activeplayerabilities":
                    return @".\res\samples\activeplayerabilities.json";
                case "https://127.0.0.1:2999/liveclientdata/activeplayerrunes":
                    return @".\res\samples\activeplayerrunes.json";
                case "https://127.0.0.1:2999/liveclientdata/playerlist":
                    return @".\res\samples\playerlist.json";
                case "https://127.0.0.1:2999/liveclientdata/eventdata":
                    return @".\res\samples\eventdata.json";
                case "https://127.0.0.1:2999/liveclientdata/gamestats":
                    return @".\res\samples\gamestats.json";
            }
            //This part accounts for different summoner names given for these URLs.
            if (url.StartsWith("https://127.0.0.1:2999/liveclientdata/playerscores?summonerName=")) {
                return @".\res\samples\playerscores_" + url.Substring(64) + ".json";
            }
            if (url.StartsWith("https://127.0.0.1:2999/liveclientdata/playersummonerspells?summonerName=")) {
                return @".\res\samples\playersummonerspells_" + url.Substring(72) + ".json";
            }
            if (url.StartsWith("https://127.0.0.1:2999/liveclientdata/playermainrunes?summonerName=")) {
                return @".\res\samples\playermainrunes_" + url.Substring(67) + ".json";
            }
            if (url.StartsWith("https://127.0.0.1:2999/liveclientdata/playeritems?summonerName=")) {
                return @".\res\samples\playeritems_" + url.Substring(63) + ".json";
            }
            return "";
        }
    }
}
