using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaisa.LeagueApi.Client {
    public class LeagueEventArgs : EventArgs {
        public readonly LeagueEvent leagueEvent;

        public LeagueEventArgs(LeagueEvent leagueEvent) {
            this.leagueEvent = leagueEvent;
        }
    }

    public class ItemsChangedEventArgs : EventArgs {
        public readonly string[] oldItemNames;
        public readonly string[] newItemNames;

        public ItemsChangedEventArgs(string[] oldItemNames, string[] newItemNames) {
            this.oldItemNames = oldItemNames;
            this.newItemNames = newItemNames;
        }
    }
}
