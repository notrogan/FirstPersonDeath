using GameNetcodeStuff;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace FirstPersonDeath.Patches
{
    internal class KillPlayerPatch
    {
        [HarmonyPatch(typeof(PlayerControllerB), "KillPlayer")]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> DeathLengthPatch(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> list = instructions.ToList();
            for (int i = 0; i < list.Count - 1; i++)
            {
                if (list[i].opcode == OpCodes.Ldc_R4 && list[i].operand.ToString() == "1.5")
                {
                    list[i].operand = 0.0f;
                    FirstPersonDeathBase.mls.LogInfo("Patched death camera time!");
                }
            }
            return list;
        }
    }
}
