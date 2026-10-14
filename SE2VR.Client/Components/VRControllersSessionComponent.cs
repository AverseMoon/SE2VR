using Keen.Game2.Client.GameSystems.Render;
using Keen.VRage.Animation.Client;
using Keen.VRage.Core.Game.Components;
using Keen.VRage.Core.Game.Data;
using Keen.VRage.Core.Game.Systems;
using Keen.VRage.DCS.Annotations;
using Keen.VRage.DCS.Builders;
using Keen.VRage.DCS.Components;
using Keen.VRage.Game.Client.Render;
using Keen.VRage.Game.Components.Models;
using Keen.VRage.Library.Definitions;
using Keen.VRage.Library.Reflection.DependencyInjections;
using SE2VR.Simulation;

namespace SE2VR.Client.Components;

/// <summary>
/// Session component that renders the VR controller models at the tracked hands.
/// <see cref="OpenVROptions.ShowControllers"/>
/// </summary>
public partial class VRControllersSessionComponent : SessionComponent, IInSceneListener
{
    private static readonly Guid _rightControllerModelId = new("3e14b273-42f2-4813-921a-1d375030d034");
    private static readonly Guid _leftControllerModelId = new("7cf5684a-9a0a-4432-90d5-4c1919f407ce");

    [Service]
    private readonly IEntitySpawner _spawner;

    [Service]
    private readonly OpenVRSessionComponent _vrSession;

    private OpenVREngineComponent _vr;

    private ModelComponentDefinition? _rightControllerModel;
    private ModelComponentDefinition? _leftControllerModel;

    private Entity? _rightController;
    private Entity? _leftController;

    [Init]
    protected new void Init()
    {
        base.Init();
        _vr = OpenVREngineComponent.Instance;

        //TODO differentiate the model based on the controller type
        _rightControllerModel = GetModelDefinition(_rightControllerModelId);
        _leftControllerModel = GetModelDefinition(_leftControllerModelId);

        static ModelComponentDefinition? GetModelDefinition(Guid modelId)
        {
            if (DefinitionManager.Instance.TryGetDefinition<ModelComponentDefinition>(modelId, out var model))
                return model;

            Logging.Error($"Controller model {modelId} not found");
            return null;
        }
    }

    void IInSceneListener.OnAddedToScene()
    {
        _rightController = CreateModelEntity(_rightControllerModel);
        _leftController = CreateModelEntity(_leftControllerModel);
    }

    private Entity? CreateModelEntity(ModelComponentDefinition? model)
    {
        if (model is null)
            return null;

        var eob = EntityBuilder.ForScene(Entity.Scene)
            .WithComponent((typeof(WorldTransformComponent), new WorldTransformComponentObjectBuilder()))
            .WithComponent((typeof(ModelComponent), model))
            .WithComponent(typeof(ModelRenderComponent))
            .WithComponent(typeof(RootEntityRenderComponent))
            .CompileObjectBuilder();

        return _spawner.SpawnEntity(eob);
    }

    [DoNotPause]
    [RigUpdateSystem.InverseKinematicsPostRagdollWindow.Begin]
    private void Tick()
    {
        if (_vrSession.ControlledEntity.Entity is not { } controlled)
            return;

        var wt = controlled.Data.GetWorldTransform();

        if (_vr.LeftHand is { } left)
            _leftController?.Data.SetWorldTransform(wt * left);

        if (_vr.RightHand is { } right)
            _rightController?.Data.SetWorldTransform(wt * right);
    }
}
