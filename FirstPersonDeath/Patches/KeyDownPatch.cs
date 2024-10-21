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

        public static PlayerControllerB NetworkController;

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        private static void SwapPatch()
        {
            //NetworkController = GameNetworkManager.Instance.localPlayerController;

            //FirstPersonDeathBase.mls.LogInfo(LastSpectatedPlayer);

            if (GameNetworkManager.Instance.localPlayerController.isPlayerDead)
            {
                if (SwapKey.IsDown())
                {
                    if (!SwapKeyDown)
                    {
                        UsePlayerCamera = !UsePlayerCamera;
                        SwapKeyDown = true;
                    }
                }
                else if (SwapKey.IsUp())
                {
                    //if (LastSpectatedPlayer == "" || UsePlayerCamera == false)
                    //{
                    //    LastSpectatedPlayer = NetworkController.spectatedPlayerScript.playerUsername;
                    //}
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
