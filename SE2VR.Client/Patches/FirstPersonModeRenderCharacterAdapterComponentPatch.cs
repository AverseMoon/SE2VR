using HarmonyLib;
using Keen.Game2.Client.GameSystems.CameraSystems.Modes;
using Keen.Game2.Client.WorldObjects.Character;

namespace SE2VR.Client.Patches;

/// <summary>
/// Patch to disable first person
/// </summary>
[HarmonyPatch(typeof(FirstPersonModeRenderCharacterAdapterComponent), nameof(FirstPersonModeRenderCharacterAdapterComponent.CameraViewModeChanged))]
public class FirstPersonModeRenderCharacterAdapterComponentPatch
{
    public static void Prefix(ref CameraViewMode newMode)
    {
        if (newMode == CameraViewMode.FirstPerson)
            newMode = CameraViewMode.ThirdPerson;
    }
}
