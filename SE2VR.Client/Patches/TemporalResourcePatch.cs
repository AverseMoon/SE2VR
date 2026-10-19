using System.Reflection;
using HarmonyLib;
using SE2VR.Client.Wrappers;

namespace SE2VR.Client.Patches;

/// <summary>
/// Patch that announces creation/destruction of any temporal resources so they can be copied on the right eye
/// </summary>
[HarmonyPatch]
public static class TemporalResourcePatch
{
    public static Action<object>? Created;
    public static Action<object>? Disposed;

    static IEnumerable<MethodBase> TargetMethods()
    {
        yield return AccessTools.GetDeclaredConstructors(MyTemporalResource.Type).Single();
        yield return AccessTools.Method(MyTemporalResource.Type, "Dispose");
    }

    [HarmonyPostfix]
    static void Postfix(object __instance, MethodBase __originalMethod)
    {
        if (__instance.GetType() != MyTemporalResource.Type)
            return;

        if (__originalMethod.IsConstructor)
            Created?.Invoke(__instance);
        else
            Disposed?.Invoke(__instance);
    }
}
