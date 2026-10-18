using HarmonyLib;
using Keen.Game2.Client.WorldObjects.Tools;
using Keen.VRage.Core;
using Keen.VRage.DCS.Accessors;

namespace SE2VR.Client.Patches;

/// <summary>
/// Patch for overwriting the interaction origin client-side
/// TODO use an actual IToolOriginProvider via content
/// </summary>
[HarmonyPatch(typeof(PlayerOriginProviderComponent), "GetWorldTransform")]
public static class PlayerOriginProviderComponent_GetWorldTransform_Patch
{
    /// <summary>
    /// Interaction origin
    /// </summary>
    public static WorldTransform? Origin;

    /// <summary>
    /// Entity whose origin is overwritten
    /// </summary>
    public static DEntity? Owner;

    public static void Postfix(PlayerOriginProviderComponent __instance, ref WorldTransform __result)
    {
        if (Origin.HasValue && Owner != null && __instance.DEntity == Owner)
            __result = Origin.Value;
    }
}
