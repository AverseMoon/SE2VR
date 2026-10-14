using System.Reflection;
using HarmonyLib;
using Keen.VRage.Library.Extensions;
using Keen.VRage.Render.FrameData;

namespace SE2VR.Client.Patches;

/// <summary>
/// Last point in render submission where commands can be submitted
/// </summary>
[HarmonyPatch]
public static class CommitFramePatch
{
    public static Action? PreCommit;

    static MethodBase? TargetMethod()
    {
        return AccessTools.Method(typeof(SharedData), "CommitFrame") ?? AccessTools.Method(typeof(SharedData), "AfterUpdate");
    }

    [HarmonyPrefix]
    static void Prefix() => PreCommit.InvokeIfNotNull();
}
