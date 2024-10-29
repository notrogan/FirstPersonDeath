using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using FirstPersonDeath.Patches;
using HarmonyLib;

namespace FirstPersonDeath
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class FirstPersonDeathBase : BaseUnityPlugin
    {
        private const string modGUID = "rogan.FirstPersonDeath";
        private const string modName = "First Person Death";
        private const string modVersion = "1.2.1";

        private readonly Harmony harmony = new Harmony(modGUID);

        private static FirstPersonDeathBase Instance;

        internal static ManualLogSource mls;

        public static ConfigEntry<string> SwapKey;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            this.LoadConfigs();

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);

            mls.LogInfo("FirstPersonDeath Started!");

            harmony.PatchAll(typeof(FirstPersonDeathBase));
            harmony.PatchAll(typeof(KeyDownPatch));
            harmony.PatchAll(typeof(KillPlayerPatch));
            harmony.PatchAll(typeof(HudManagerPatch));
            harmony.PatchAll(typeof(MaskedPlayerPatch));
            harmony.PatchAll(typeof(PlayerControllerPatch));
        }
        private void LoadConfigs()
        {
            SwapKey = Config.Bind("FirstPersonDeath", "SwapKey", "E", "Key used to toggle perspectives; Default binding may conflict with other mods!");
        }
    }
}
