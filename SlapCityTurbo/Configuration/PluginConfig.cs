using BepInEx.Configuration;
using SCMU.Online;
using System;

namespace SlapCityTurbo.Configuration
{
    class PluginConfig
    {
        static bool initialized = false;
        internal static bool settingsChanged = false;

        #region Properties
        public static bool IsEnabled
        {
            get;
            private set;
        }
        public static float DamageMultBonus
        {
            get;
            private set;
        }
        public static float KnockbackMultBonus
        {
            get;
            private set;
        }
        public static float WeightMultBonus
        {
            get;
            private set;
        }
        public static float HitlagMultBonus
        {
            get;
            private set;
        }
        public static float StateSpeedMultBonus
        {
            get;
            private set;
        }
        public static float RunSpeedMultBonus
        {
            get;
            private set;
        }
        #endregion

        static ConfigEntry<bool> isEnabled;
        static ConfigEntry<float> damageMultBonus;
        static ConfigEntry<float> knockbackMultBonus;
        static ConfigEntry<float> weightMultBonus;
        static ConfigEntry<float> hitlagMultBonus;
        static ConfigEntry<float> stateSpeedMultBonus;
        static ConfigEntry<float> runSpeedMultBonus;

        internal static void Init()
        {
            var config = Plugin.Instance.Config;
            config.SettingChanged += OnSettingChanged;

            isEnabled = config.Bind("Settings", "Enabled", false, new ConfigDescription(string.Empty, null, new ConfigurationManagerAttributes { Order = 6 }));
            damageMultBonus = config.Bind("Settings", "Damage Multiplier", 1f, new ConfigDescription("Multiplier on damage % per attack.", null, new ConfigurationManagerAttributes { Order = 5 }));
            knockbackMultBonus = config.Bind("Settings", "Knockback Multiplier", 1f, new ConfigDescription("Multiplier on knockback per attack.", null, new ConfigurationManagerAttributes { Order = 4 }));
            weightMultBonus = config.Bind("Settings", "Weight Multiplier", 2f, new ConfigDescription("Multiplier on weight.", null, new ConfigurationManagerAttributes { Order = 3 }));
            hitlagMultBonus = config.Bind("Settings", "Hitlag Multiplier", 0.5f, new ConfigDescription("Multiplier on hitlag per attack.", null, new ConfigurationManagerAttributes { Order = 2 }));
            stateSpeedMultBonus = config.Bind("Settings", "State Speed Multiplier", 2f, new ConfigDescription("Multiplier on how fast actions are taken.", null, new ConfigurationManagerAttributes { Order = 1 }));
            runSpeedMultBonus = config.Bind("Settings", "Run Speed Multiplier", 2f, new ConfigDescription("Multiplayer on how fast characters move.", null, new ConfigurationManagerAttributes { Order = 0 }));

            Save();
            Reload();
            initialized = true;
        }

        static void OnSettingChanged(object sender, EventArgs args)
        {
            settingsChanged = true;
        }

        internal static void LoadOnlineSettings(LobbyModSettings lobbysettings)
        {
            // Check if all players have the mod installed
            if (!ModRegistry.CheckLobbyModInstalled(Plugin.Instance.Info.Metadata.Name))
            {
                Plugin.LogDebug("Not all players have the mod. Disabling...");
                return;
            }

            IsEnabled = lobbysettings.TryGetSetting(nameof(IsEnabled), out var setting) && Convert.ToBoolean(setting.value);
            if (!IsEnabled) return; // If not enabled, no point in loading the rest of the settings

            DamageMultBonus = lobbysettings.TryGetSetting(nameof(DamageMultBonus), out setting) ? Convert.ToSingle(setting.value) : 1f;
            KnockbackMultBonus = lobbysettings.TryGetSetting(nameof(KnockbackMultBonus), out setting) ? Convert.ToSingle(setting.value) : 1f;
            WeightMultBonus = lobbysettings.TryGetSetting(nameof(WeightMultBonus), out setting) ? Convert.ToSingle(setting.value) : 1f;
            HitlagMultBonus = lobbysettings.TryGetSetting(nameof(HitlagMultBonus), out setting) ? Convert.ToSingle(setting.value) : 1f;
            StateSpeedMultBonus = lobbysettings.TryGetSetting(nameof(StateSpeedMultBonus), out setting) ? Convert.ToSingle(setting.value) : 1f;
            RunSpeedMultBonus = lobbysettings.TryGetSetting(nameof(RunSpeedMultBonus), out setting) ? Convert.ToSingle(setting.value) : 1f;
        }

        internal static LobbyModSettings GetLobbyModSettings()
        {
            settingsChanged = false;
            Reload();

            var lobbySettings = new LobbyModSettings(Plugin.Instance.Info.Metadata.Name,
                new Setting(nameof(IsEnabled), IsEnabled),
                new Setting(nameof(DamageMultBonus), DamageMultBonus),
                new Setting(nameof(KnockbackMultBonus), KnockbackMultBonus),
                new Setting(nameof(WeightMultBonus), WeightMultBonus),
                new Setting(nameof(HitlagMultBonus), HitlagMultBonus),
                new Setting(nameof(StateSpeedMultBonus), StateSpeedMultBonus),
                new Setting(nameof(RunSpeedMultBonus), RunSpeedMultBonus)
            );

            return lobbySettings;
        }

        static void Save()
        {
            if (initialized)
            {
                isEnabled.Value = IsEnabled;
                damageMultBonus.Value = DamageMultBonus;
                knockbackMultBonus.Value = KnockbackMultBonus;
                weightMultBonus.Value = WeightMultBonus;
                hitlagMultBonus.Value = HitlagMultBonus;
                stateSpeedMultBonus.Value = StateSpeedMultBonus;
                runSpeedMultBonus.Value = RunSpeedMultBonus;
            }
            Plugin.Instance.Config.Save();
        }

        internal static void Reload()
        {
            Plugin.Instance.Config.Reload();

            IsEnabled = isEnabled.Value;
            DamageMultBonus = damageMultBonus.Value;
            KnockbackMultBonus = knockbackMultBonus.Value;
            WeightMultBonus = weightMultBonus.Value;
            HitlagMultBonus = hitlagMultBonus.Value;
            StateSpeedMultBonus = stateSpeedMultBonus.Value;
            RunSpeedMultBonus = runSpeedMultBonus.Value;
        }
    }
}
