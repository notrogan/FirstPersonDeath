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
        private const string modVersion = "1.0.3";

        private readonly Harmony harmony = new Harmony(modGUID);

        private static FirstPersonDeathBase Instance;

        internal static ManualLogSource mls;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);

            mls.LogInfo("FirstPersonDeath Started!");

            harmony.PatchAll(typeof(FirstPersonDeathBase));
            harmony.PatchAll(typeof(KillPlayerPatch));
        }
    }
}
