using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaisa.LeagueApi.Client {
    public class EventApi {
        private AdvancedLeagueApi api;
        private string summonerName;
        private bool isApiLoaded = false;

        public EventApi(bool useTestSamples = false) {
            api = new AdvancedLeagueApi(useTestSamples);
        }
        /// <summary>
        /// Sets the name of the active player, which is needed for some events.
        /// </summary>
        /// <param name="summonerName"></param>
        public void SetActivePlayerName(string summonerName) => this.summonerName = summonerName;

        private void SetupEventWorker(ref BackgroundWorker worker, DoWorkEventHandler checkMethod) {
            if (worker != null) return;

            worker = new BackgroundWorker();
            worker.WorkerSupportsCancellation = true;
            worker.DoWork -= checkMethod;
            worker.DoWork += checkMethod;
            worker.RunWorkerAsync();
        }

        #region ApiLoaded listener.
        /// <summary>
        /// Starts the ApiLoaded listener, necessary to raise events when the api is loaded and unloaded.
        /// </summary>
        /// <param name="delay">The delay (in ms) between each comprobation of the event.</param>
        public void StartApiLoadedListener(int delay = 1000) {
            delayApiLoaded = delay;
            SetupEventWorker(ref workerApiLoaded, ListenApiLoaded);
        }
        public void StopApiLoadedListener() => workerApiLoaded.CancelAsync();

        private BackgroundWorker workerApiLoaded;
        private int delayApiLoaded;

        private async void ListenApiLoaded(object sender, DoWorkEventArgs e) {
            while (workerApiLoaded.CancellationPending) {
                Task endLoop = Task.Delay(delayApiLoaded);

                bool apiLoaded = await api.IsPlayerInGame();
                if (apiLoaded) {
                    if (!isApiLoaded) {
                        isApiLoaded = true;
                        OnApiLoaded();
                    }
                }
                else {
                    if (isApiLoaded) {
                        isApiLoaded = false;
                        OnApiEnded();
                    }
                }

                await endLoop;
            }
        }

        /// <summary>
        /// Raised when the player enters a game and the api becomes available. This is not related to <see cref="Event_GameStart"/>.
        /// </summary>
        public event ApiLoadedEventHandler ApiLoaded;
        public delegate void ApiLoadedEventHandler(object source, EventArgs args);
        protected virtual void OnApiLoaded() => ApiLoaded?.Invoke(this, EventArgs.Empty);
        /// <summary>
        /// Raised when a game finishes and the api is closed. This is not related to <see cref="Event_GameEnd"/>.
        /// </summary>
        public event ApiEndedEventHandler ApiEnded;
        public delegate void ApiEndedEventHandler(object source, EventArgs args);
        protected virtual void OnApiEnded() => ApiEnded?.Invoke(this, EventArgs.Empty);
        #endregion

        #region PlayerStatus listener
        /// <summary>
        /// Starts the PlayerStatus listener, necessary to raise events when the player dies or respawns.
        /// </summary>
        /// <param name="delay">The delay (in ms) between each comprobation of the event.</param>
        public void StartPlayerStatusListener(int delay = 100) {
            delayPlayerStatus = delay;
            SetupEventWorker(ref workerPlayerStatus, ListenPlayerStatus);
        }
        public void StopPlayerStatusListener() => workerPlayerStatus.CancelAsync();

        private BackgroundWorker workerPlayerStatus;
        private int delayPlayerStatus;

        private bool playerWasDead = false;
        private async void ListenPlayerStatus(object sender, DoWorkEventArgs e) {
            while (!workerPlayerStatus.CancellationPending) {
                Task endLoop = Task.Delay(delayPlayerStatus);

                bool isPlayerDead = await api.IsPlayerDead(summonerName);

                if (playerWasDead ^ isPlayerDead) {
                    if (isPlayerDead) OnPlayerDied();
                    else OnPlayerRespawned();
                    playerWasDead = isPlayerDead;
                }

                await endLoop;
            }
        }

        /// <summary>
        /// Raised when the active player dies.
        /// </summary>
        public event PlayerDiedEventHandler PlayerDied;
        public delegate void PlayerDiedEventHandler(object source, EventArgs args);
        protected virtual void OnPlayerDied() => PlayerDied?.Invoke(this, EventArgs.Empty);
        /// <summary>
        /// Raised when the active player respawns.
        /// </summary>
        public event PlayerRespawnedEventHandler PlayerRespawned;
        public delegate void PlayerRespawnedEventHandler(object source, EventArgs args);
        protected virtual void OnPlayerRespawned() => PlayerRespawned?.Invoke(this, EventArgs.Empty);
        #endregion

        #region LevelUp listener.
        /// <summary>
        /// Starts the LevelUp listener, necessary to raise events when the player levels up.
        /// </summary>
        /// <param name="delay">The delay (in ms) between each comprobation of the event.</param>
        public void StartLevelUpListener(int delay = 100) {
            delayLevelUp = delay;
            SetupEventWorker(ref workerLevelUp, ListenLevelUp);
        }

        public void StopLevelUpListener() => workerLevelUp.CancelAsync();

        private BackgroundWorker workerLevelUp;
        private int delayLevelUp;

        private int lastLevel = 1;
        private async void ListenLevelUp(object sender, DoWorkEventArgs e) {
            while (!workerLevelUp.CancellationPending) {
                Task endLoop = Task.Delay(delayLevelUp);

                int level = await api.GetActivePlayerLevel();
                if (level != lastLevel) {
                    OnLevelUp();
                    lastLevel = level;
                }

                await endLoop;
            }
        }
        /// <summary>
        /// Raised when the active player levels up.
        /// </summary>
        public event LevelUpEventHandler LevelUp;
        public delegate void LevelUpEventHandler(object source, EventArgs args);
        protected virtual void OnLevelUp() => LevelUp?.Invoke(this, EventArgs.Empty);
        #endregion

        #region ItemChanged listener.
        public void StartItemChangedListener(int delay = 100) {
            delayItemChanged = delay;
            SetupEventWorker(ref workerItemChanged, ListenItemChanged);
        }

        public void StopItemChangedListener() => workerItemChanged.CancelAsync();

        private BackgroundWorker workerItemChanged;
        private int delayItemChanged;

        private string[] oldItemNames = new string[1] { "no_item" };
        private async void ListenItemChanged(object sender, DoWorkEventArgs e) {
            while (!workerItemChanged.CancellationPending) {
                Task endLoop = Task.Delay(delayLevelUp);

                Item[] currentItems = (await api.GetPlayerItems(summonerName)).OrderBy(i => i.name).ToArray();
                string[] currentItemNames = new string[currentItems.Length];

                if (currentItems.Length == 0) {
                    currentItemNames = new string[1] { "no_item" };
                }
                else {
                    for (int i = 0; i < currentItemNames.Length; i++) {
                        currentItemNames[i] = currentItems[i].name;
                    }
                }

                if (!currentItemNames.SequenceEqual(oldItemNames)) {
                    ItemsChangedEventArgs args = new ItemsChangedEventArgs(oldItemNames, currentItemNames);
                    oldItemNames = currentItemNames;
                    OnItemChanged(args);
                }

                await endLoop;
            }
        }
        /// <summary>
        /// Raised when the items of the player change. This does not include when the order of the items change.
        /// The arguments of the event contain an array with the names of the current items and another array with the names of the old items.
        /// If the player has no items, the array will contain a single string with value "no_item".
        /// </summary>
        public event ItemChangedEventHandler ItemChanged;
        public delegate void ItemChangedEventHandler(object source, ItemsChangedEventArgs args);
        protected virtual void OnItemChanged(ItemsChangedEventArgs args) => ItemChanged?.Invoke(this, args);
        #endregion

        #region LeagueEvent listener.
        /// <summary>
        /// Starts the LeagueEvent listener, necessary to raise events related to LeagueEvent.
        /// LeagueEvents are always passed inside the event's arguments.
        /// </summary>
        /// <param name="delay">The delay (in ms) between each comprobation of the event.</param>
        public void StartLeagueEventListener(int delay = 100) {
            delayLeagueEvent = delay;
            SetupEventWorker(ref workerLeagueEvent, ListenLeagueEvent);
        }
        public void StopLeagueEventListener() => workerLeagueEvent.CancelAsync();

        private BackgroundWorker workerLeagueEvent;
        private int delayLeagueEvent;

        private int lastLeagueEventIndex = -1;
        private async void ListenLeagueEvent(object sender, DoWorkEventArgs e) {
            while (!workerLeagueEvent.CancellationPending) {
                Task endLoop = Task.Delay(delayLeagueEvent);

                LeagueEvent leagueEvent = await api.GetLastEvent();
                if (leagueEvent != null && leagueEvent.id != lastLeagueEventIndex) {
                    int thisEventIndex = leagueEvent.id;
                    //If only one event has happened since the last check.
                    if (thisEventIndex == lastLeagueEventIndex + 1) {
                        RaiseEvent(leagueEvent);
                    }
                    //If more than one event happened since the last check, get all events and trigger all events from [last + 1] until the last one.
                    else {
                        LeagueEvent[] allGameEvents = await api.GetEvents();
                        for (int i = lastLeagueEventIndex + 1; i <= thisEventIndex; i++) {
                            RaiseEvent(allGameEvents[i]);
                        }
                    }

                    lastLeagueEventIndex = leagueEvent.id;

                }

                await endLoop;
            }

            void RaiseEvent(LeagueEvent thisEvent) {
                LeagueEventArgs args = new LeagueEventArgs(thisEvent);
                OnLeagueEventHappened(args);
                if (thisEvent is Event_GameStart) OnGameStart(args);
                else if (thisEvent is Event_MinionsFirstSpawn) OnMinionsFirstSpawn(args);
                else if (thisEvent is Event_FirstTurretKill) OnFirstTurretKill(args);
                else if (thisEvent is Event_TurretKill) OnTurretKill(args);
                else if (thisEvent is Event_InhibKill) OnInhibKill(args);
                else if (thisEvent is Event_DragonKill) OnDragonKill(args);
                else if (thisEvent is Event_HeraldKill) OnHeraldKill(args);
                else if (thisEvent is Event_BaronKill) OnBaronKill(args);
                else if (thisEvent is Event_ChampionKill) OnChampionKill(args);
                else if (thisEvent is Event_MultiKill) OnMultiKill(args);
                else if (thisEvent is Event_Ace) OnAce(args);
                else if (thisEvent is Event_FirstBlood) OnFirstBlood(args);
                else if (thisEvent is Event_GameEnd) OnGameEnd(args);
            }
        }
        /// <summary>
        /// Raised whenever a new LeagueEvent of any kind happens in the game.
        /// </summary>
        public event LeagueEventHandler LeagueEventHappened;
        public delegate void LeagueEventHandler(object source, LeagueEventArgs args);
        protected virtual void OnLeagueEventHappened(LeagueEventArgs args) => LeagueEventHappened?.Invoke(this, args);
        /// <summary>
        /// Raised when the LeagueEvent 'GameStart' happens. This is unrelated to when the game or the api was loaded.
        /// </summary>
        public event GameStartHandler GameStart;
        public delegate void GameStartHandler(object source, LeagueEventArgs args);
        protected virtual void OnGameStart(LeagueEventArgs args) => GameStart?.Invoke(this, args);
        /// <summary>
        /// Raised when the first wave of minions spawn.
        /// </summary>
        public event MinionsFirstSpawnEventHandler MinionsFirstSpawn;
        public delegate void MinionsFirstSpawnEventHandler(object source, LeagueEventArgs args);
        protected virtual void OnMinionsFirstSpawn(LeagueEventArgs args) => MinionsFirstSpawn?.Invoke(this, args);
        /// <summary>
        /// Raised when the first brick is broken.
        /// </summary>
        public event FirstTurretKillEventHandler FirstTurretKill;
        public delegate void FirstTurretKillEventHandler(object source, LeagueEventArgs args);
        protected virtual void OnFirstTurretKill(LeagueEventArgs args) => FirstTurretKill?.Invoke(this, args);
        /// <summary>
        /// Raised when a turret is destroyed.
        /// </summary>
        public event TurretKillEventHandler TurretKill;
        public delegate void TurretKillEventHandler(object source, LeagueEventArgs args);
        protected virtual void OnTurretKill(LeagueEventArgs args) => TurretKill?.Invoke(this, args);
        /// <summary>
        /// Raised when an inhibitor is destroyed.
        /// </summary>
        public event InhibKillEventHandler InhibKill;
        public delegate void InhibKillEventHandler(object source, LeagueEventArgs args);
        protected virtual void OnInhibKill(LeagueEventArgs args) => InhibKill?.Invoke(this, args);
        /// <summary>
        /// Raised when a dragon is killed.
        /// </summary>
        public event DragonKillEventHandler DragonKill;
        public delegate void DragonKillEventHandler(object source, LeagueEventArgs args);
        protected virtual void OnDragonKill(LeagueEventArgs args) => DragonKill?.Invoke(this, args);
        /// <summary>
        /// Raised when the herald is killed.
        /// </summary>
        public event HeraldKillEventHandler HeraldKill;
        public delegate void HeraldKillEventHandler(object source, LeagueEventArgs args);
        protected virtual void OnHeraldKill(LeagueEventArgs args) => HeraldKill?.Invoke(this, args);
        /// <summary>
        /// Raised when Baron Nashor is killed.
        /// </summary>
        public event BaronKillEventHandler BaronKill;
        public delegate void BaronKillEventHandler(object source, LeagueEventArgs args);
        protected virtual void OnBaronKill(LeagueEventArgs args) => BaronKill?.Invoke(this, args);
        /// <summary>
        /// Raised when a champion is killed.
        /// </summary>
        public event ChampionKillEventHandler ChampionKill;
        public delegate void ChampionKillEventHandler(object source, LeagueEventArgs args);
        protected virtual void OnChampionKill(LeagueEventArgs args) => ChampionKill?.Invoke(this, args);
        /// <summary>
        /// Raised when a multi-kill happens.
        /// </summary>
        public event MultiKillEventHandler MultiKill;
        public delegate void MultiKillEventHandler(object source, LeagueEventArgs args);
        protected virtual void OnMultiKill(LeagueEventArgs args) => MultiKill?.Invoke(this, args);
        /// <summary>
        /// Raised when a team is aced.
        /// </summary>
        public event AceEventHandler Ace;
        public delegate void AceEventHandler(object source, LeagueEventArgs args);
        protected virtual void OnAce(LeagueEventArgs args) => Ace?.Invoke(this, args);
        /// <summary>
        /// Raised when a player gets the first blood.
        /// </summary>
        public event FirstBloodEventHandler FirstBlood;
        public delegate void FirstBloodEventHandler(object source, LeagueEventArgs args);
        protected virtual void OnFirstBlood(LeagueEventArgs args) => FirstBlood?.Invoke(this, args);
        /// <summary>
        /// Raised when the game ends.
        /// </summary>
        public event GameEndEventHandler GameEnd;
        public delegate void GameEndEventHandler(object source, LeagueEventArgs args);
        protected virtual void OnGameEnd(LeagueEventArgs args) => GameEnd?.Invoke(this, args);
        #endregion
    }
}
