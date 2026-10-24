using Keen.VRage.Core.EngineComponents;
using Keen.VRage.Core.Game.GameSystems.GameTimes;
using Keen.VRage.DCS.Annotations;
using Keen.VRage.Library.Mathematics;
using Keen.VRage.Library.Reflection.DependencyInjections;
using Keen.VRage.Render.EngineComponents;
using SE2VR.Client.Configurations;
using SE2VR.Client.Patches;

namespace SE2VR.Client.Components;

/// <summary>
/// Session component that desaturates the world while it is loading
/// </summary>
public partial class VRLoadingEngineComponent : EngineComponent
{
    [Configuration]
    private readonly VRLoadingCoreConfiguration _config;

    [Service]
    private readonly IWallTime _wallTime;

    private bool _wasLoading;
    private TimeSpan? _fadeStart;

    [Init]
    protected new void Init()
    {
        base.Init();

        CommitFramePatch.PreCommit += Update;
    }

    [Destructor]
    protected void Destroy()
    {
        CommitFramePatch.PreCommit -= Update;
    }

    private void Update()
    {
        if (LoadingTransitionScreenPatch.Loading)
        {
            _wasLoading = true;
            _fadeStart = null;
            Fade(0);
            return;
        }

        if (_wasLoading)
        {
            _wasLoading = false;
            _fadeStart = _wallTime.WallTime.FrameTime;
        }

        if (_fadeStart is not { } fadeStart)
            return;

        float ratio = (float)Math.Min((_wallTime.WallTime.FrameTime - fadeStart) / _config.FadeDuration, 1);
        Fade(ratio);

        if (ratio >= 1)
            _fadeStart = null;
    }

    private void Fade(float ratio)
    {
        var settings = RenderEngineComponent.Instance.RenderContracts.GetSettings();
        settings.PostProcessSettings = settings.PostProcessSettings with
        {
            Contrast = MathHelper.Lerp(_config.Contrast, 1, ratio),
            Saturation = MathHelper.Lerp(_config.Saturation, 1, ratio),
        };
    }
}
