using System.Reflection;
using HarmonyLib;

namespace SE2VR.Client.Wrappers;

/// <summary>
/// My render SettingsManager
/// Used to acccess internal type
/// </summary>
public class MySettings(object? obj)
{
#pragma warning disable KN023
    private static readonly FieldInfo _renderViewField;
    private static readonly FieldInfo _previousRenderViewField;
    private static readonly MethodInfo _onBeginDrawMethod;
    private static readonly MethodInfo _onEndDrawMethod;
#pragma warning restore KN023

    public MyRenderView RenderView
    {
        get => new(_renderViewField.GetValue(obj)!);
        set => _renderViewField.SetValue(obj, value.Box);
    }

    public MyRenderView PreviousRenderView
    {
        get => new(_previousRenderViewField.GetValue(obj)!);
        set => _previousRenderViewField.SetValue(obj, value.Box);
    }

    static MySettings()
    {
        var type = AccessTools.TypeByName("Keen.VRage.Render12.Core.Systems.SettingsManager");
        _renderViewField = AccessTools.Field(type, "_renderView");
        _previousRenderViewField = AccessTools.Field(type, "_previousRenderView");
        _onBeginDrawMethod = AccessTools.Method(type, "OnBeginDraw");
        _onEndDrawMethod = AccessTools.Method(type, "OnEndDraw");
    }

    public void OnBeginDraw() => _onBeginDrawMethod.Invoke(obj, null);

    public void OnEndDraw() => _onEndDrawMethod.Invoke(obj, null);

}
