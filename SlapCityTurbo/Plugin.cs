using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using SCMU.Events;
using SCMU.Online;
using SlapCityTurbo.Configuration;
using System.Reflection;
using System.Collections;
using UnityEngine;
using Smash;

namespace SlapCityTurbo
{
    [BepInPlugin("com.steven.slapcity.turbo", "Turbo", "2.0.0.0")]
    [BepInDependency("com.steven.slapcity.scmu")]
    public class Plugin : BaseUnityPlugin
    {
        internal static Plugin Instance;

        const string harmonyID = "com.steven.slapcity.turbo";
        Harmony harmony;

        void Awake()
        {
            Instance = this;

            LogDebug("Initializing...");
            PluginConfig.Init();

            // set up events
            // TODO: add functionality to clean up events
            Events.OnGameStarted += OnGameStart;
            Events.OnGameEnded += OnGameEnd;
            Events.OnJoinedOrCreatedLobby += OnLobbyJoinOrCreate;

            ModRegistry.Register(Info.Metadata.Name);

            LogDebug("Initialized!");
        }

        void OnGameStart()
        {
            if (OnlineSettingsManager.InLobby)
            {
                var lobbySettings = OnlineSettingsManager.GetLobbySettingsForMod(Info.Metadata.Name);
                if (lobbySettings == null) return;
                PluginConfig.LoadOnlineSettings(lobbySettings);
                LogDebug("Loaded online lobby settings!");
            }
            else PluginConfig.Reload();

            if(routine != null) StopCoroutine(routine);

            if (!PluginConfig.IsEnabled) return;
            Enable();
        }

        void OnGameEnd()
        {
            Disable();

            if (SmashSteam.Lobbies.Joined)
                routine = StartCoroutine(KeepLobbySettingsUpdated());
        }
        void Enable()
        {
            if (Harmony.HasAnyPatches(harmonyID)) return;
            if (harmony == null) harmony = new Harmony(harmonyID);
            LogDebug("Patching...");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        void Disable()
        {
            if (harmony == null || !Harmony.HasAnyPatches(harmonyID)) return;
            LogDebug("Unpatching...");
            harmony.UnpatchSelf();
        }

        // ===========================================================
        //                          Online
        // ===========================================================
        #region Online
        void OnLobbyJoinOrCreate()
        {
            UpdateLobbySettings();

            routine = StartCoroutine(KeepLobbySettingsUpdated());
        }

        Coroutine routine;
        IEnumerator KeepLobbySettingsUpdated()
        {
            while (SmashSteam.Lobbies.Joined)
            {
                yield return new WaitForSeconds(0.5f);

                if (PluginConfig.settingsChanged)
                    UpdateLobbySettings();
            }
        }

        void UpdateLobbySettings()
        {
            LogDebug("Attempting to upload mod settings to lobby");

            var myLobbySettings = PluginConfig.GetLobbyModSettings();
            if (myLobbySettings == null) return;
            OnlineSettingsManager.ApplySettingsToLobby(myLobbySettings);
        }
        #endregion
        // ===========================================================
        //                         Logging
        // ===========================================================

        internal static void LogDebug(string message) => Instance.Log(message, LogLevel.Debug);
        internal static void LogInfo(string message) => Instance.Log(message, LogLevel.Info);
        private void Log(string message, LogLevel logLevel) => Logger.Log(logLevel, message);
    }
}
