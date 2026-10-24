using Avalonia.Controls;
using Avalonia.LogicalTree;
using HarmonyLib;
using Keen.Game2.Client.UI.Overlays;

namespace SE2VR.Client.Patches;

/// <summary>
/// Patch to hide background images in the loading screen and track whether it is visible or not
/// </summary>
[HarmonyPatch(typeof(LoadingTransitionScreen), MethodType.Constructor)]
public static class LoadingTransitionScreenPatch
{
    public static bool Loading { get; private set; }

    [HarmonyPostfix]
    public static void Postfix(LoadingTransitionScreen __instance)
    {
        if (__instance.DataContext is not LoadingTransitionScreenViewModel { IsLoading: true })
            return;

        Loading = true;
        HideBackground(__instance);

        __instance.DataContextChanged += OnDataContextChanged;
    }

    private static void OnDataContextChanged(object? sender, EventArgs e)
    {
        var screen = (LoadingTransitionScreen)sender!;
        if (screen.DataContext is not null)
            return;

        Loading = false;
        screen.DataContextChanged -= OnDataContextChanged;
    }

    private static void HideBackground(LoadingTransitionScreen screen)
    {
        foreach (var control in screen.GetLogicalDescendants())
        {
            switch (control)
            {
                case Image image:
                    image.IsVisible = false;
                    break;

                case Panel panel:
                    panel.Background = null;
                    break;

                default:
                    break;
            }
        }
    }
}
