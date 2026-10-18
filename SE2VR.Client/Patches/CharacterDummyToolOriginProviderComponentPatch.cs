using HarmonyLib;
using Keen.Game2.Simulation.GameSystems.AI;
using Keen.Game2.Simulation.WorldObjects.Characters;
using Keen.VRage.Core;

namespace SE2VR.Client.Patches;

/// <summary>
/// Patch for overwriting the interaction origin server-side
/// TODO MP use an actual IToolOriginProvider via content, read from synced hand data
/// </summary>
[HarmonyPatch(typeof(CharacterDummyToolOriginProviderComponent), nameof(CharacterDummyToolOriginProviderComponent.WorldTransform), MethodType.Getter)]
public static class CharacterDummyToolOriginProviderComponent_WorldTransform_Patch
{
    public static WorldTransform? Origin;

    public static void Postfix(CharacterDummyToolOriginProviderComponent __instance, ref WorldTransform __result)
    {
        if (Origin.HasValue && !__instance.Entity.Data.Has<AIControllerData>())
            __result = Origin.Value;
    }
}
