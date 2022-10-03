using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace SlapCityTurbo.Utils
{
    class TranspilerUtils
    {
        internal static IEnumerable<CodeInstruction> EditCharacterBaseStat(IEnumerable<CodeInstruction> codes, float newValue)
        {
            codes.Last(x => x.opcode == OpCodes.Ldc_R4).operand = newValue;
            return codes;
        }
    }
}
