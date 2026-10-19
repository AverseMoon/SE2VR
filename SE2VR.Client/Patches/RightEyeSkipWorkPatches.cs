using System.Reflection;
using HarmonyLib;
using SE2VR.Client.Components;

namespace SE2VR.Client.Patches;

/// <summary>
/// Patch to disable various tasks during right eye render
/// </summary>
[HarmonyPatch]
public static class RightEyeSkipWorkPatch
{
    static IEnumerable<MethodBase> TargetMethods()
    {
        yield return AccessTools.Method("Keen.VRage.Render12.LightingStage.CascadeShadowsContext:FlushUpdates");
        yield return AccessTools.Method("Keen.VRage.Render12.LightingStage.CharacterShadowsContext:FlushUpdates");
        yield return AccessTools.Method("Keen.VRage.Render12.LightingStage.EnvironmentProbeManager:PrepareProbes");
        yield return AccessTools.Method("Keen.VRage.Render12.Core.Systems.SceneDrawSystem:ComputeCloudShadows");
    }

    [HarmonyPrefix]
    static bool Prefix() => VRRenderEngineComponent.Instance?.IsRightEye != true;
}