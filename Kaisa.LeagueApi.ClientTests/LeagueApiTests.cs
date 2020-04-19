using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kaisa.LeagueApi.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaisa.LeagueApi.Client.Tests {
    [TestClass()]
    public class LeagueApiTests {
        LeagueApi leagueApi = new LeagueApi(false);

        [TestMethod()]
        public async Task IsPlayerInGameTest() {
            bool output = await leagueApi.IsPlayerInGame();
            bool expectedResult = false;

            if (output != expectedResult) {
                Assert.Fail();
            }
        }

        [TestMethod()]
        public async Task GetActivePlayerStatsTest() {
            PlayerStats output = await leagueApi.GetActivePlayerStats();
            //TODO: Full comparison.
            double expectedCurrentGold = output.currentGold;

            if(output.currentGold != expectedCurrentGold) {
                Assert.Fail();
            }
        }

        //TODO: Complete tests.
    }
}