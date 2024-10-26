using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace FirstPersonDeath.Patches
{
    internal class HudManagerPatch
    {
        [HarmonyPatch(typeof(HUDManager), "Update")]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> EarlyVotePatch(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> list = instructions.ToList();
            for (int i = 0; i < list.Count - 1; i++)
            {
                if (list[i].opcode == OpCodes.Ldstr && (list[i].operand.ToString() == "Tell autopilot ship to leave early : [RMB] (Hold)"))
                {
                    list[i].operand = list[i].operand += $"\n\n\n\n\nSwitch Camera: [{FirstPersonDeathBase.SwapKey.Value}]"; ;
                    FirstPersonDeathBase.mls.LogInfo("Transpiler for HudManager executed successfully!");
                }
                else
                {
                    if (list[i].opcode == OpCodes.Ldstr && (list[i].operand.ToString() == "Voted for ship to leave early" || list[i].operand.ToString() == "Ship leaving in one hour"))
                    {
                        list[i].operand = list[i].operand += $"\n\n\n\n\n\nSwitch Camera: [{FirstPersonDeathBase.SwapKey.Value}]"; ;
                    }
                }
            }
            return list;
        }
    }
}
