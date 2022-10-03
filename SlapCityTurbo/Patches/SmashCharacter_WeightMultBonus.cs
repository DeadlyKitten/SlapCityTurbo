using HarmonyLib;
using SlapCityTurbo.Configuration;
using SlapCityTurbo.Utils;
using Smash;
using System.Collections.Generic;

namespace SlapCityTurbo.Patches
{
    [HarmonyPatch(typeof(SmashCharacter), "WeightMultBonus", MethodType.Getter)]
    class SmashCharacter_WeightMultBonus
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return TranspilerUtils.EditCharacterBaseStat(instructions, PluginConfig.WeightMultBonus);
        }
    }
}