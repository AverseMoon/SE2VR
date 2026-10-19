using System.Reflection;
using HarmonyLib;

namespace SE2VR.Client.Wrappers;

/// <summary>
/// My SceneDrawSystem
/// Used to acccess internal type
/// </summary>
public class MySceneDrawSystem(object? obj)
{
#pragma warning disable KN023
    private static readonly MethodInfo _drawMethod;
    private static readonly FieldInfo _upsamplingJobField;
    private static readonly FieldInfo _fsrField;
#pragma warning restore KN023

    public static readonly Type Type;

    public MyFsr Fsr => new(_upsamplingJobField.GetValue(obj) is { } job ? _fsrField.GetValue(job) : null);

    static MySceneDrawSystem()
    {
        Type = AccessTools.TypeByName("Keen.VRage.Render12.Core.Systems.SceneDrawSystem");
        _drawMethod = AccessTools.Method(Type, "Draw");
        _upsamplingJobField = AccessTools.Field(Type, "_upsamplingJob");
        _fsrField = AccessTools.Field(AccessTools.TypeByName("Keen.VRage.Render12.PostProcessStage.Upsampling.UpsamplingJob"), "_fsr4_1");
    }

    public void Draw(object finalLDRBuffer) => _drawMethod.Invoke(obj, [finalLDRBuffer]);
}
