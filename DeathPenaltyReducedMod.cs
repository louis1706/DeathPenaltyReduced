using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;

using OpCodes = System.Reflection.Emit.OpCodes;

namespace DeathAnnouncer
{
    [BepInPlugin("com.yamato.DeathPenaltyReducedMod", "Death Penalty Reduced", "1.0.0.0")]
    public class DeathPenaltyReducedMod : BaseUnityPlugin
    {
        public static ConfigEntry<bool> IsEnable;

        private void Awake()
        {
            // Init config values
            IsEnable = Config.Bind("General", "IsEnable_DeathPenaltyReduced", true, "");

            var harmony = new Harmony("com.yamato.DeathPenaltyReducedMod");
            harmony.PatchAll();
            Harmony.CreateAndPatchAll(typeof(DeathPenaltyReducedMod));
        }

        [HarmonyPatch(typeof(Skills), nameof(Skills.LowerAllSkills))]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> ReducePenatly(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = new(instructions);

            int offset = 1;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Stloc_1) + offset;

            /*Remove 
            
            float num = keyValuePair.Value.m_level * factor;
			keyValuePair.Value.m_level -= num;
            */
            newInstructions.RemoveRange(index, 13);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];
        }
    }
}
