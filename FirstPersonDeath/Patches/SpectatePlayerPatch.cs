using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Unity.Netcode;

namespace FirstPersonDeath.Patches
{
    internal class SpectatePlayerPatch
    {
        [HarmonyPatch(typeof(PlayerControllerB), "Interact_performed")]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> InteractPerformedPatch(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> list = instructions.ToList();
            for (int i = 0; i < list.Count - 1; i++)
            {
                if (list[i].opcode == OpCodes.Call && list[i].operand == AccessTools.Method(typeof(PlayerControllerB), "SpectateNextPlayer"))
                {
                    //list.RemoveRange(0, i+1);
                    FirstPersonDeathBase.mls.LogInfo("Transpiler for PlayerControllerB executed successfully!");
                }
                //FirstPersonDeathBase.mls.LogInfo($"{list[i].opcode} - {list[i].operand}");
            }
            return list;
        }
    }
}
