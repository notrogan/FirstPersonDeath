using BepInEx.Configuration;
using GameNetcodeStuff;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;

namespace FirstPersonDeath.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class KeyDownPatch
    {
        public static KeyCode SwapKeyCode = (KeyCode)System.Enum.Parse(typeof(KeyCode), FirstPersonDeathBase.SwapKey.Value);
        public static KeyboardShortcut SwapKey = new KeyboardShortcut(SwapKeyCode);

        public static bool KeyDown = false;
        public static bool UsePlayerCamera = false;

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        private static void SwapPatch()
        {
            if (GameNetworkManager.Instance.localPlayerController.isPlayerDead)
            {
                if (SwapKey.IsDown())
                {
                    if (!KeyDown)
                    {
                        UsePlayerCamera = !UsePlayerCamera;
                        KeyDown = true;
                    }
                }
                else if (SwapKey.IsUp())
                {
                    KeyDown = false;
                }
            }
            else
            {
                UsePlayerCamera = true;
            }
        }
    }
}
