using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace FirstPersonDeath.Patches
{
    [HarmonyPatch(typeof(MaskedPlayerEnemy))]
    internal class MaskedPlayerPatch
    {
        public static Transform MaskedTransform;

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void GetMaskedPatch(ref PlayerControllerB ___mimickingPlayer, Transform ___headTiltTarget)
        {
            if (___mimickingPlayer)
            {
                if (___mimickingPlayer.playerUsername == PlayerControllerPatch.PlayerUsername)
                {
                    MaskedTransform = ___headTiltTarget;
                }
            }
        }
    }
}
