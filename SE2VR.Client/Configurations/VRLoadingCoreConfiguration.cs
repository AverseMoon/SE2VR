using Keen.VRage.Library.Definitions.CoreConfigurations;
using Keen.VRage.Library.UI.PropertyInspector;

namespace SE2VR.Client.Configurations;

/// <summary>
/// Configuration for the post processing effects applied while the world is loading
/// </summary>
public partial class VRLoadingCoreConfiguration : CoreConfiguration
{
    /// <summary>
    /// Post process contrast while the world is loading
    /// </summary>
    public float Contrast { get; private set; } = 0.7f;

    /// <summary>
    /// Post process saturation while the world is loading
    /// </summary>
    public float Saturation { get; private set; } = 0f;

    /// <summary>
    /// Time to fade back to normal after loading has completed
    /// </summary>
    [TimeSpanScope(Scope = TimeSpanScope.Seconds)]
    public TimeSpan FadeDuration { get; private set; } = TimeSpan.FromSeconds(1.5);
}
