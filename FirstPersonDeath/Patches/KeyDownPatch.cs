using BepInEx.Configuration;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace FirstPersonDeath.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class KeyDownPatch
    {
        public static KeyCode SwapKeyCode = (KeyCode)System.Enum.Parse(typeof(KeyCode), FirstPersonDeathBase.SwapKey.Value);
        public static KeyboardShortcut SwapKey = new KeyboardShortcut(SwapKeyCode);

        public static bool SwapKeyDown = false;
        public static bool UsePlayerCamera = false;
        public static string LastSpectatedPlayer = "";

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        private static void SwapPatch()
        {
            if (GameNetworkManager.Instance.localPlayerController.isPlayerDead)
            {
                if (SwapKey.IsDown())
                {
                    if (!SwapKeyDown)
                    {
                        UsePlayerCamera = !UsePlayerCamera;
                        PlayerControllerPatch.SentDebug = false;
                        SwapKeyDown = true;
                    }
                }
                else if (SwapKey.IsUp())
                {
                    SwapKeyDown = false;
                }
            }
            else
            {
                UsePlayerCamera = true;
            }
        }
    }
}
