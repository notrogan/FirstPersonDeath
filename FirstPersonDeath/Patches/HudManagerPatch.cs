using HarmonyLib;

namespace FirstPersonDeath.Patches
{
    [HarmonyPatch(typeof(HUDManager), "Update")]
    internal class HudManagerPatch
    {
        private static void Postfix(HUDManager __instance)
        {
            __instance.holdButtonToEndGameEarlyText.text += $"\n\n\n\n\nSwitch Camera: [{FirstPersonDeathBase.SwapKey.Value}]";
        }
    }
}
