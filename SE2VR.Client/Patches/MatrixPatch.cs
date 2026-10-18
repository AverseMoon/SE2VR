using HarmonyLib;
using Keen.Game2.Client.GameSystems.CameraSystems;
using Keen.VRage.Library.Definitions;
using Keen.VRage.Library.Mathematics;
using Keen.VRage.Render.Contracts;
using OpenVRAPI;
using Valve.VR;

namespace SE2VR.Client.Patches;

/// <summary>
/// Patch for using the correct projection matrix in the camera
/// </summary>
[HarmonyPatch(typeof(Matrix), nameof(Matrix.CreatePerspectiveFovRhInfiniteComplementary))]
public static class MatrixPatch
{
    public static EVREye? CurrentEye;

    public static void Postfix(ref Matrix __result)
    {
        if (CurrentEye.HasValue && OpenVR.System != null)
            __result = (Matrix)VRUtils.GetPerspectiveFovRhInfiniteComplementary(CurrentEye.Value, DefinitionManager.Instance.GetDefinitionsOfType<CameraDefinition>().FirstOrDefault()?.NearPlane ?? 0.1f);
    }
}

/// <summary>
/// Another patch that prevents part of the matrix being overwritten by render
/// </summary>
[HarmonyPatch(typeof(RenderSettings), nameof(RenderSettings.SetCameraParameters))]
public static class RenderSettingsProjectionOffsetPatch
{
    public static void Prefix(ref Vector2 projectionOffset)
    {
        if (MatrixPatch.CurrentEye.HasValue && OpenVR.System != null)
        {
            var projection = VRUtils.GetPerspectiveFovRhInfiniteComplementary(MatrixPatch.CurrentEye.Value, 0.1f);
            projectionOffset = new Vector2((float)projection.M31, (float)projection.M32);
        }
    }
}
