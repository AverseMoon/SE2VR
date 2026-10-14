using Keen.Game2.Client.GameSystems.Render;
using Keen.Game2.Client.WorldObjects.Character;
using Keen.Game2.Simulation.WorldObjects.Characters;
using Keen.VRage.Animation.Client;
using Keen.VRage.Core.EngineComponents;
using Keen.VRage.Core.Game.Components;
using Keen.VRage.Core.Game.Systems;
using Keen.VRage.Core.Game.Utils;
using Keen.VRage.DCS.Annotations;
using Keen.VRage.DCS.Components;
using Keen.VRage.Library.Definitions;
using Keen.VRage.Library.Reflection.DependencyInjections;
using SE2VR.Simulation;

namespace SE2VR.Client.Components;

/// <summary>
/// Session component that hides the body of the controlled character (except the hands)
/// </summary>
public partial class VRBodyHidingSessionComponent : SessionComponent, IInSceneListener
{
    /// <summary>
    /// Preset that hides every body part except the gloves
    /// TODO move to configuration in content pipeline
    /// </summary>
    private static readonly Guid _hideBodyPresetId = new("92d3eaaf-4704-4915-bd02-13257b24ced4");

    /// <summary>
    /// Preset that hides every body part except the gloves and boots
    /// TODO move to configuration in content pipeline
    /// </summary>
    private static readonly Guid _hideBodyShowBootsPresetId = new("aa73f9ee-1ae0-4c4a-992c-20be087f95cd");

    [Service]
    private readonly IOptions _options;

    [Service]
    private readonly OpenVRSessionComponent _vrSession;

    private PresetIdTypeDefinition? _hideBodyPreset;
    private readonly WeakEntityReference _hiddenBody = new();

    [Init]
    protected new void Init()
    {
        base.Init();

        var hideBodyPresetId = _options.GetOrCreatePart<OpenVROptions>().ShowBoots ? _hideBodyShowBootsPresetId : _hideBodyPresetId;
        if (DefinitionManager.Instance.TryGetDefinition<PresetIdTypeDefinition>(hideBodyPresetId, out var hideBodyPreset))
            _hideBodyPreset = hideBodyPreset;
        else
            Logging.Error($"Body hiding preset {hideBodyPresetId} not found");
    }

    [Destructor]
    protected void Destroy()
    {
        _hiddenBody.Dispose();
    }

    void IInSceneListener.OnBeforeRemovedFromScene()
    {
        UpdateHiddenBody(null);
    }

    [DoNotPause]
    [RigUpdateSystem.InverseKinematicsPostRagdollWindow.Begin]
    private void Tick()
    {
        var controlled = _vrSession.ControlledEntity.Entity;
        UpdateHiddenBody(controlled?.TryGet<CharacterComponent>() is not null ? controlled : null);
    }

    private void UpdateHiddenBody(Entity? body)
    {
        if (_hideBodyPreset is null)
            return;

        if (_hiddenBody.Entity is { } previous && previous != body)
        {
            foreach (var strategy in previous.All<CharacterRenderStrategyComponent>())
                strategy.UnhidePreset(_hideBodyPreset, this);
        }

        _hiddenBody.Entity = body;

        if (body is not null)
        {
            foreach (var strategy in body.All<CharacterRenderStrategyComponent>())
                strategy.HidePreset(_hideBodyPreset, this);
        }
    }
}
