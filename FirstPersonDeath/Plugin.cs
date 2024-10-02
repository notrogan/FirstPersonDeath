using BepInEx;
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
<<<<<<< Updated upstream
        private const string modVersion = "1.0.3";
=======
        private const string modVersion = "1.1.3";
>>>>>>> Stashed changes

        private readonly Harmony harmony = new Harmony(modGUID);

        private static FirstPersonDeathBase Instance;

        internal static ManualLogSource mls;

<<<<<<< Updated upstream
=======
        public static ConfigEntry<string> SwapKey;

>>>>>>> Stashed changes
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

<<<<<<< Updated upstream
=======

            this.LoadConfigs();

>>>>>>> Stashed changes
            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);

            mls.LogInfo("FirstPersonDeath Started!");

            harmony.PatchAll(typeof(FirstPersonDeathBase));
            harmony.PatchAll(typeof(KeyDownPatch));
            harmony.PatchAll(typeof(KillPlayerPatch));
            harmony.PatchAll(typeof(PlayerControllerPatch));
        }
<<<<<<< Updated upstream
=======

        private void LoadConfigs()
        {
            SwapKey = Config.Bind("FirstPersonDeath", "SwapKey", "E", "Key used to toggle perspectives; Default binding may conflict with other mods!");
        }
>>>>>>> Stashed changes
    }
}
