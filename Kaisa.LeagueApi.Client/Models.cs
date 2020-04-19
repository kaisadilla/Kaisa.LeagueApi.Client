using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaisa.LeagueApi.Client {
    /// <summary>
    /// Contains all of the player stats. Note that champion stats are stored inside a variable called "champion".
    /// </summary>
    public class PlayerStats {
        public readonly Ability[] abilities;
        public readonly Champion champion;
        public readonly double currentGold;

        public readonly FullRunes fullRunes;

        public readonly int level;
        public readonly string summonerName;

        public PlayerStats(
                Ability[] abilities, Champion champion, double currentGold,
                FullRunes fullRunes,
                int level, string summonerName) {
            this.abilities = abilities;
            this.champion = champion;
            this.currentGold = currentGold;
            this.fullRunes = fullRunes;
            this.level = level;
            this.summonerName = summonerName;
        }
    }

    /// <summary>
    /// Contains all the stats of a champion.
    /// </summary>
    public class Champion {
        [JsonProperty("attackDamage")]
        public readonly double AD;
        public readonly double attackSpeed;
        public readonly double critChance;
        public readonly double critDamage;
        [JsonProperty("attackRange")]
        public readonly double range;
        [JsonProperty("physicalLethality")]
        public readonly double lethality;
        [JsonProperty("armorPenetrationFlat")]
        public readonly double armorPen;
        [JsonProperty("armorPenetrationPercent")]
        public readonly double armorPenP;
        [JsonProperty("bonusArmorPenetrationPercent")]
        public readonly double armorBonusPenP;

        [JsonProperty("abilityPower")]
        public readonly double AP;
        /// <summary>
        /// This number is a negative double that goes from 0 to 1. For example, 30% cdr will be represented as -0.30d.
        /// </summary>
        public readonly double cooldownReduction;
        [JsonProperty("magicLethality")]
        public readonly double magicLethality;
        [JsonProperty("magicPenetrationFlat")]
        public readonly double magicPen;
        [JsonProperty("magicPenetrationPercent")]
        public readonly double magicPenP;
        [JsonProperty("bonusMagicPenetrationPercent")]
        public readonly double magicBonusPenP;

        [JsonProperty("lifeSteal")]
        public readonly double lifesteal;
        [JsonProperty("spellVamp")]
        public readonly double spellvamp;

        [JsonProperty("currentHealth")]
        public readonly double health_current;
        [JsonProperty("maxHealth")]
        public readonly double health_max;
        [JsonProperty("healthRegenRate")]
        public readonly double healthRegen;

        [JsonProperty("resourceMax")]
        public readonly double resource_max;
        [JsonProperty("resourceRegenRate")]
        public readonly double resourceRegen;
        [JsonProperty("resourceValue")]
        public readonly double resource_current;
        public readonly string resourceType;

        public readonly double armor;
        public readonly double magicResist;

        public readonly double moveSpeed;
        public readonly double tenacity;

        public Champion(
                double AD, double attackSpeed, double critChance, double critDamage, double range, double lethality, double armorPen, double armorPenP, double armorBonusPenP,
                double AP, double cooldownReduction, double magicLethality, double magicPen, double magicPenP, double magicBonusPenP,
                double lifesteal, double spellvamp,
                double health_current, double health_max, double healthRegen,
                double resource_max, double resourceRegen, double resource_current, string resourceType,
                double armor, double magicResist,
                double moveSpeed, double tenacity) {
            this.AD = AD;
            this.attackSpeed = attackSpeed;
            this.critChance = critChance;
            this.critDamage = critDamage;
            this.range = range;
            this.lethality = lethality;
            this.armorPen = armorPen;
            this.armorPenP = armorPenP;
            this.armorBonusPenP = armorBonusPenP;
            this.AP = AP;
            this.cooldownReduction = cooldownReduction;
            this.magicLethality = magicLethality;
            this.magicPen = magicPen;
            this.magicPenP = magicPenP;
            this.magicBonusPenP = magicBonusPenP;
            this.lifesteal = lifesteal;
            this.spellvamp = spellvamp;
            this.health_current = health_current;
            this.health_max = health_max;
            this.healthRegen = healthRegen;
            this.resource_max = resource_max;
            this.resourceRegen = resourceRegen;
            this.resource_current = resource_current;
            this.resourceType = resourceType;
            this.armor = armor;
            this.magicResist = magicResist;
            this.moveSpeed = moveSpeed;
            this.tenacity = tenacity;
        }
    }

    public struct Ability {
        [JsonProperty("abilityLevel")]
        public readonly int level;
        [JsonProperty("displayName")]
        public readonly string name;
        public readonly string id;
        public readonly string rawDescription;
        public readonly string rawDisplayName;

        [JsonConstructor] //Without this, Newtonsoft.Json can't properly deserialize structs.
        public Ability(int level, string name, string id, string rawDescription, string rawDisplayName) {
            this.level = level;
            this.name = name;
            this.id = id;
            this.rawDescription = rawDescription;
            this.rawDisplayName = rawDisplayName;
        }
    }

    public struct Rune {
        [JsonProperty("displayName")]
        public readonly string name;
        public readonly int id;
        public readonly string rawDescription;
        public readonly string rawDisplayName;

        [JsonConstructor]
        public Rune(string name, int id, string rawDescription, string rawDisplayName) {
            this.name = name;
            this.id = id;
            this.rawDescription = rawDescription;
            this.rawDisplayName = rawDisplayName;
        }
    }

    public struct RuneTree {
        [JsonProperty("displayName")]
        public readonly string name;
        public readonly int id;
        public readonly string rawDescription;
        public readonly string rawDisplayName;

        [JsonConstructor]
        public RuneTree(string name, int id, string rawDescription, string rawDisplayName) {
            this.name = name;
            this.id = id;
            this.rawDescription = rawDescription;
            this.rawDisplayName = rawDisplayName;
        }
    }

    public struct StatRune {
        public readonly int id;
        public readonly string rawDescription;

        [JsonConstructor]
        public StatRune(int id, string rawDescription) {
            this.id = id;
            this.rawDescription = rawDescription;
        }
    }

    public class FullRunes {
        public readonly Rune[] runes;
        public readonly Rune keystoneRune;
        public readonly RuneTree primaryTree;
        public readonly RuneTree secondaryTree;
        public readonly StatRune[] statRunes;

        public FullRunes(Rune[] runes, Rune keystoneRune, RuneTree primaryTree, RuneTree secondaryTree, StatRune[] statRunes) {
            this.runes = runes;
            this.keystoneRune = keystoneRune;
            this.primaryTree = primaryTree;
            this.secondaryTree = secondaryTree;
            this.statRunes = statRunes;
        }
    }

    public class BasicRunes {
        public readonly Rune keystoneRune;
        public readonly RuneTree primaryTree;
        public readonly RuneTree secondaryTree;

        public BasicRunes(Rune keystoneRune, RuneTree primaryTree, RuneTree secondaryTree) {
            this.keystoneRune = keystoneRune;
            this.primaryTree = primaryTree;
            this.secondaryTree = secondaryTree;
        }
    }

    public class Player {
        public readonly string summonerName;
        public readonly string team;
        [JsonProperty("skinID")]
        public readonly int skinId;
        public readonly string position;
        public readonly Score score;

        public readonly string championName;
        public readonly int level;
        public readonly Item[] items;
        public readonly SummonerSpell[] summonerSpells;
        public readonly BasicRunes basicRunes;

        public readonly bool isDead;
        public readonly double respawnTimer;
        public readonly bool isBot;

        public readonly string rawChampionName;

        public Player(
                string summonerName, string team, int skinId, string position, Score score,
                string championName, int level, Item[] items, SummonerSpell[] summonerSpells, BasicRunes basicRunes,
                bool isDead, double respawnTimer, bool isBot,
                string rawChampionName) {
            this.summonerName = summonerName;
            this.team = team;
            this.skinId = skinId;
            this.position = position;
            this.score = score;
            this.championName = championName;
            this.level = level;
            this.items = items;
            this.summonerSpells = summonerSpells;
            this.basicRunes = basicRunes;
            this.isDead = isDead;
            this.respawnTimer = respawnTimer;
            this.isBot = isBot;
            this.rawChampionName = rawChampionName;
        }
    }

    public struct Item {
        public readonly int slot;
        [JsonProperty("displayName")]
        public readonly string name;
        public readonly int count;
        public readonly bool canUse;
        public readonly bool consumable;
        [JsonProperty("itemID")]
        public readonly int id;
        public readonly int price;
        public readonly string rawDescription;
        public readonly string rawDisplayName;

        [JsonConstructor]
        public Item(int slot, string name, int count, bool canUse, bool consumable, int id, int price, string rawDescription, string rawDisplayName) {
            this.slot = slot;
            this.name = name;
            this.count = count;
            this.canUse = canUse;
            this.consumable = consumable;
            this.id = id;
            this.price = price;
            this.rawDescription = rawDescription;
            this.rawDisplayName = rawDisplayName;
        }
        /*
        /// <summary>
        /// Returns true if both items have the same id.
        /// </summary>
        public static bool operator == (Item x, Item y) {
            if (x.id != y.id) return false;
            else return true;
        }
        public static bool operator != (Item x, Item y) {
            if (x != y) return true;
            else return false;
        }
        */
    }

    public struct Score {
        public readonly int kills;
        public readonly int deaths;
        public readonly int assists;
        public readonly int creepScore;
        public readonly double wardScore;

        [JsonConstructor]
        public Score(int kills, int deaths, int assists, int creepScore, double wardScore) {
            this.kills = kills;
            this.deaths = deaths;
            this.assists = assists;
            this.creepScore = creepScore;
            this.wardScore = wardScore;
        }
    }

    public struct SummonerSpell {
        [JsonProperty("displayName")]
        public readonly string name;
        public readonly string rawDescription;
        public readonly string rawDisplayName;

        [JsonConstructor]
        public SummonerSpell(string name, string rawDescription, string rawDisplayName) {
            this.name = name;
            this.rawDescription = rawDescription;
            this.rawDisplayName = rawDisplayName;
        }

        public static bool operator ==(SummonerSpell x, SummonerSpell y) {
            if (x.name == y.name) return true;
            else return false;
        }

        public static bool operator !=(SummonerSpell x, SummonerSpell y) {
            if (x == y) return false;
            else return true;
        }
    }

    public struct Game {
        [JsonProperty("gameMode")]
        public readonly string mode;
        [JsonProperty("gameTime")]
        public readonly double time;
        public readonly string mapName;
        public readonly string mapNumber;
        public readonly string mapTerrain;

        [JsonConstructor]
        public Game(string mode, double time, string mapName, string mapNumber, string mapTerrain) {
            this.mode = mode;
            this.time = time;
            this.mapName = mapName;
            this.mapNumber = mapNumber;
            this.mapTerrain = mapTerrain;
        }
    }
}
