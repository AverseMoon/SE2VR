using HarmonyLib;
using Keen.Game2.Client.UI.Tutorial;

namespace SE2VR.Client.Patches;

/// <summary>
/// Patch to automatically skip all FTUE tutorials
/// </summary>
[HarmonyPatch(typeof(TutorialOverlaySessionComponent), nameof(TutorialOverlaySessionComponent.LoadState))]
public class TutorialOverlaySessionComponentPatch
{
    private static readonly Action<TutorialOverlaySessionComponent> _onSkip = AccessTools.MethodDelegate<Action<TutorialOverlaySessionComponent>>(AccessTools.Method(typeof(TutorialOverlaySessionComponent), "OnSkipActivated"));

    public static bool Prefix(TutorialOverlaySessionComponent __instance)
    {
        _onSkip(__instance);
        __instance.Clear();

        return false;
    }
}
