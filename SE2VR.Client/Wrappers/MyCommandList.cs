using System.Reflection;
using HarmonyLib;
using Vortice.Direct3D12;

namespace SE2VR.Client.Wrappers;

/// <summary>
/// My engine command list
/// Used to acccess internal type
/// </summary>
public class MyCommandList(object? obj) : IDisposable
{
#pragma warning disable KN023
    private static readonly MethodInfo _copyResourceMethod;
    private static readonly MethodInfo _explicitStateTransitionMethod;
#pragma warning restore KN023

    public object Instance => obj!;

    static MyCommandList()
    {
        var type = AccessTools.TypeByName("Keen.VRage.Render12.Core.CommandLists.CopyCommandList");
        _copyResourceMethod = AccessTools.Method(type, "CopyResource");
        _explicitStateTransitionMethod = AccessTools.Method(type, "ExplicitStateTransition");
    }

    public void Dispose() => ((IDisposable)obj!).Dispose();

    /// <summary>
    /// Copies content of image texture to the destination texture
    /// </summary>
    public void CopyResource(object destination, object source) => _copyResourceMethod.Invoke(obj, [destination, source]);

    public void TransitionTo(MyTexture texture, ResourceStates state) => _explicitStateTransitionMethod.Invoke(obj, [texture.AutoResourceState, state, false]);
}
