using System.Reflection;
using HarmonyLib;

namespace SE2VR.Client.Wrappers;

/// <summary>
/// MY device context
/// Used to acccess internal type
/// </summary>
public class MyDeviceContext(object? obj)
{
#pragma warning disable KN023
    private static readonly FieldInfo _directQueueField;
#pragma warning restore KN023

    static MyDeviceContext()
    {
        var type = AccessTools.TypeByName("Keen.VRage.Render12.Core.Device.DeviceContext");
        _directQueueField = AccessTools.Field(type, "<DirectQueue>k__BackingField");
    }

    public MyCommandQueue GetDirectQueue() => new(_directQueueField.GetValue(obj));
}
