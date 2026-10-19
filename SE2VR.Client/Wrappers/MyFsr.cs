using System.Reflection;
using HarmonyLib;
using Keen.VRage.Library.Mathematics;

namespace SE2VR.Client.Wrappers;

/// <summary>
/// My FSR upscaler
/// Used to acccess internal type
/// </summary>
public class MyFsr(object? obj)
{
#pragma warning disable KN023
    private static readonly FieldInfo _contextField;
    private static readonly FieldInfo _resourcesField;
    private static readonly FieldInfo _forceFsr31Field;
    private static readonly PropertyInfo _finalResolutionProperty;
    private static readonly MethodInfo _borrowContextMethod;
    private static readonly MethodInfo _initializeContextMethod;
    private static readonly MethodInfo _disposeContextMethod;
    private static readonly uint _defaultFlags;
#pragma warning restore KN023

    public object? Context
    {
        get => obj == null ? null : _contextField.GetValue(obj);
        set => _contextField.SetValue(obj, value);
    }

    public bool IsActive => obj != null && _resourcesField.GetValue(obj) != null && Context != null;

    public Vector2I FinalResolution => (Vector2I)_finalResolutionProperty.GetValue(_resourcesField.GetValue(obj))!;

    public bool ForceFsr31 => (bool)_forceFsr31Field.GetValue(obj)!;

    static MyFsr()
    {
        var type = AccessTools.TypeByName("Keen.VRage.Render12.PostProcessStage.Upsampling.FSR4.FSR4_1");
        var contextType = AccessTools.TypeByName("Keen.VRage.Render12.PostProcessStage.Upsampling.FSR4.FSR4_1Context");
        _contextField = AccessTools.Field(type, "_context");
        _resourcesField = AccessTools.Field(type, "_resources");
        _forceFsr31Field = AccessTools.Field(type, "_contextForceFsr31");
        _finalResolutionProperty = AccessTools.Property(_resourcesField.FieldType, "FinalResolution");
        _defaultFlags = (uint)AccessTools.Field(type, "DEFAULT_FLAGS").GetRawConstantValue()!;
        _initializeContextMethod = AccessTools.Method(contextType, "Initialize");
        _disposeContextMethod = AccessTools.Method(contextType, "Dispose");
        _borrowContextMethod = AccessTools.Method(AccessTools.TypeByName("Keen.VRage.Render12.Resources.Utilities.GPUResourcePool"), "Borrow").MakeGenericMethod(contextType);
    }

    public static void DisposeContext(object context) => _disposeContextMethod.Invoke(context, null);

    public static object CreateContext(Vector2I resolution, bool forceFsr31)
    {
        var args = new object?[1] { null };
        _borrowContextMethod.Invoke(MyCoreSystems.GPUResourcePool, args);
        _initializeContextMethod.Invoke(args[0], [_defaultFlags, resolution, resolution, forceFsr31]);
        return args[0]!;
    }
}
