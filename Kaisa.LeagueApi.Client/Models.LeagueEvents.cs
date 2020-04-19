using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaisa.LeagueApi.Client {
    /// <summary>
    /// The base 'event' class from which all event classes derive. If an event can't be mapped to its class, it will use this generic class instead.
    /// </summary>
    public class LeagueEvent {
        [JsonProperty("EventName")]
        public readonly string name;
        [JsonProperty("EventId")]
        public readonly int id;
        /// <summary>
        /// The timestamp of the event, in seconds since the start of the game.
        /// </summary>
        [JsonProperty("EventTime")]
        public readonly double time;

        public LeagueEvent(string name, int id, double time) {
            this.name = name;
            this.id = id;
            this.time = time;
        }

        public override string ToString() {
            return "An event of type " + name + " ocurred at time " + time;
        }
    }

    public class Event_GameStart : LeagueEvent {
        public Event_GameStart(string name, int id, double time) : base(name, id, time) { }

        public override string ToString() {
            return "The game started at time " + time;
        }
    }

    public class Event_MinionsFirstSpawn : LeagueEvent {
        public Event_MinionsFirstSpawn(string name, int id, double time) : base(name, id, time) { }

        public override string ToString() {
            return "First wave of minions spawned at time " + time;
        }
    }

    public class Event_TurretKill : LeagueEvent {
        [JsonProperty("KillerName")]
        public readonly string killerName;
        [JsonProperty("Assisters")]
        public readonly string[] assisters;
        [JsonProperty("TurretKilled")]
        public readonly string turretKilled;

        public Event_TurretKill(string name, int id, double time, string killerName, string[] assisters, string turretKilled) : base(name, id, time) {
            this.killerName = killerName;
            this.assisters = assisters;
            this.turretKilled = turretKilled;
        }

        public override string ToString() {
            return "The turret " + turretKilled + " was destroyed by " + killerName + " at time " + time;
        }
    }

    public class Event_InhibKill : LeagueEvent {
        [JsonProperty("KillerName")]
        public readonly string killerName;
        [JsonProperty("Assisters")]
        public readonly string[] assisters;
        [JsonProperty("InhibKilled")]
        public readonly string inhibKilled;

        public Event_InhibKill(string name, int id, double time, string killerName, string[] assisters, string inhibKilled) : base(name, id, time) {
            this.killerName = killerName;
            this.assisters = assisters;
            this.inhibKilled = inhibKilled;
        }

        public override string ToString() {
            return "The inhibitor " + inhibKilled + " was destroyed by " + killerName + " at time " + time;
        }
    }

    public class Event_DragonKill : LeagueEvent {
        [JsonProperty("KillerName")]
        public readonly string killerName;
        [JsonProperty("Assisters")]
        public readonly string[] assisters;
        [JsonProperty("DragonType")]
        public readonly string dragonType;
        [JsonProperty("Stolen")]
        public readonly bool stolen;

        public Event_DragonKill(string name, int id, double time, string killerName, string[] assisters, string dragonType, bool stolen) : base(name, id, time) {
            this.killerName = killerName;
            this.assisters = assisters;
            this.dragonType = dragonType;
            this.stolen = stolen;
        }

        public override string ToString() {
            return "A dragon of type " + dragonType + " was killed by " + killerName + " at time " + time + ". Stolen? " + stolen;
        }
    }

    public class Event_HeraldKill : LeagueEvent {
        [JsonProperty("KillerName")]
        public readonly string killerName;
        [JsonProperty("Assisters")]
        public readonly string[] assisters;
        [JsonProperty("Stolen")]
        public readonly bool stolen;

        public Event_HeraldKill(string name, int id, double time, string killerName, string[] assisters, bool stolen) : base(name, id, time) {
            this.killerName = killerName;
            this.assisters = assisters;
            this.stolen = stolen;
        }

        public override string ToString() {
            return "The herald was killed by " + killerName + " at time " + time + ". Stolen? " + stolen;
        }
    }

    public class Event_BaronKill : LeagueEvent {
        [JsonProperty("KillerName")]
        public readonly string killerName;
        [JsonProperty("Assisters")]
        public readonly string[] assisters;
        [JsonProperty("Stolen")]
        public readonly bool stolen;

        public Event_BaronKill(string name, int id, double time, string killerName, string[] assisters, bool stolen) : base(name, id, time) {
            this.killerName = killerName;
            this.assisters = assisters;
            this.stolen = stolen;
        }

        public override string ToString() {
            return "The Baron Nashor was killed by " + killerName + " at time " + time + ". Stolen? " + stolen;
        }
    }

    public class Event_ChampionKill : LeagueEvent {
        [JsonProperty("KillerName")]
        public readonly string killerName;
        [JsonProperty("Assisters")]
        public readonly string[] assisters;
        [JsonProperty("VictimName")]
        public readonly string victimName;

        public Event_ChampionKill(string name, int id, double time, string killerName, string[] assisters, string victimName) : base(name, id, time) {
            this.killerName = killerName;
            this.assisters = assisters;
            this.victimName = victimName;
        }

        public override string ToString() {
            return victimName + "was killed by " + killerName + " at time " + time + ". Stolen? Probably";
        }
    }

    public class Event_MultiKill : LeagueEvent {
        [JsonProperty("KillerName")]
        public readonly string killerName;
        [JsonProperty("KillStreak")]
        public readonly int killStreak;

        public Event_MultiKill(string name, int id, double time, string killerName, int killStreak) : base(name, id, time) {
            this.killerName = killerName;
            this.killStreak = killStreak;
        }

        public override string ToString() {
            return "A multikill (" + killStreak + ") by " + killerName + " ocurred at time " + time;
        }
    }

    public class Event_Ace : LeagueEvent {
        [JsonProperty("Acer")]
        public readonly string acer;
        [JsonProperty("AcingTeam")]
        public readonly string acingTeam;

        public Event_Ace(string name, int id, double time, string acer, string acingTeam) : base(name, id, time) {
            this.acer = acer;
            this.acingTeam = acingTeam;
        }

        public override string ToString() {
            return "Team" + acingTeam + "aced the opposing team at time " + time;
        }
    }

    public class Event_FirstTurretKill : LeagueEvent {
        [JsonProperty("KillerName")]
        public readonly string killerName;

        public Event_FirstTurretKill(string name, int id, double time, string killerName) : base(name, id, time) {
            this.killerName = killerName;
        }

        public override string ToString() {
            return "The first turret was destroyed by player " + killerName + " at time " + time;
        }
    }

    public class Event_FirstBlood : LeagueEvent {
        [JsonProperty("Recipient")]
        public readonly string killerName;

        public Event_FirstBlood(string name, int id, double time, string killerName) : base(name, id, time) {
            this.killerName = killerName;
        }
    }

    public class Event_GameEnd : LeagueEvent {
        public readonly bool wonByActivePlayer;

        public Event_GameEnd(string name, int id, double time, string result) : base(name, id, time) {
            wonByActivePlayer = (result == "Win");
        }
    }
}
