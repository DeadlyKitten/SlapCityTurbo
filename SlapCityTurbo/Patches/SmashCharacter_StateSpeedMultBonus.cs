using HarmonyLib;
using SlapCityTurbo.Configuration;
using SlapCityTurbo.Utils;
using Smash;
using System.Collections.Generic;

namespace SlapCityTurbo.Patches
{
    [HarmonyPatch(typeof(SmashCharacter), "StateSpeedMultBonus", MethodType.Getter)]
    class SmashCharacter_StateSpeedMultBonus
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return TranspilerUtils.EditCharacterBaseStat(instructions, PluginConfig.StateSpeedMultBonus);
        }
    }
}