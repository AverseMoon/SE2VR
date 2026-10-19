using System.Reflection;
using HarmonyLib;

namespace SE2VR.Client.Wrappers;

/// <summary>
/// My TemporalResource
/// Used to acccess internal type
/// </summary>
public class MyTemporalResource(object obj)
{
#pragma warning disable KN023
    private static readonly FieldInfo _resource0Field;
    private static readonly FieldInfo _resource1Field;
    private static readonly FieldInfo _swappedField;
#pragma warning restore KN023

    public static readonly Type Type;

    public object Instance => obj;

    public object Resource0
    {
        get => _resource0Field.GetValue(obj)!;
        set => _resource0Field.SetValue(obj, value);
    }

    public object Resource1
    {
        get => _resource1Field.GetValue(obj)!;
        set => _resource1Field.SetValue(obj, value);
    }

    public bool Swapped
    {
        get => (bool)_swappedField.GetValue(obj)!;
        set => _swappedField.SetValue(obj, value);
    }

    static MyTemporalResource()
    {
        Type = AccessTools.TypeByName("Keen.VRage.Render12.Resources.Utilities.TemporalResource`1")
            .MakeGenericType(AccessTools.TypeByName("Keen.VRage.Render12.Resources.BindableTextures.ResizableRWRenderTargetTexture"));

        _resource0Field = AccessTools.Field(Type, "_resource0");
        _resource1Field = AccessTools.Field(Type, "_resource1");
        _swappedField = AccessTools.Field(Type, "_swapped");
    }
}
