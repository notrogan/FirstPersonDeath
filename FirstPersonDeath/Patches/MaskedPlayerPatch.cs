using GameNetcodeStuff;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;

namespace FirstPersonDeath.Patches
{
    [HarmonyPatch(typeof(MaskedPlayerEnemy))]
    internal class MaskedPlayerPatch
    {
        public static Transform MaskedTransform;

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void GetPlay2erPatch(ref PlayerControllerB ___mimickingPlayer, Transform ___headTiltTarget)
        {
            if (___mimickingPlayer)
            {
                if (___mimickingPlayer.playerUsername == PlayerControllerPatch.PlayerUsername)
                {
                    MaskedTransform = ___headTiltTarget;
                }
                //Component[] components = ___headTiltTarget.gameObject.GetComponentsInChildren<Component>(true);
                //foreach (var component in components)
                //{
                //    FirstPersonDeathBase.mls.LogInfo(component.ToString());
                //}
            }
        }
    }
}
