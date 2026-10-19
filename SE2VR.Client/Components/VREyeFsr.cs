using Keen.VRage.Library.Mathematics;
using SE2VR.Client.Wrappers;

namespace SE2VR.Client.Components;

/// <summary>
/// Gives the right eye its own FSR context/history
/// </summary>
public class VREyeFsr
{
    private object? _rightContext;
    private Vector2I _rightResolution;
    private bool _rightForce31;
    private object? _heldEngineContext;

    public void BeginRightEye()
    {
        if (MyCoreSystems.SceneDrawSystem.Fsr is not { } fsr || !fsr.IsActive)
        {
            if (_rightContext != null)
                MyFsr.DisposeContext(_rightContext);

            _rightContext = null;
            return;
        }

        var resolution = fsr.FinalResolution;
        bool force31 = fsr.ForceFsr31;
        if (_rightContext != null && (_rightResolution != resolution || _rightForce31 != force31))
        {
            MyFsr.DisposeContext(_rightContext);
            _rightContext = null;
        }

        if (_rightContext == null)
        {
            _rightContext = MyFsr.CreateContext(resolution, force31);
            _rightResolution = resolution;
            _rightForce31 = force31;
        }

        _heldEngineContext = fsr.Context;
        fsr.Context = _rightContext;
    }

    public void EndRightEye()
    {
        if (_heldEngineContext is not { } engineContext)
            return;

        _heldEngineContext = null;
        var fsr = MyCoreSystems.SceneDrawSystem.Fsr;
        if (fsr.Context == _rightContext)
        {
            fsr.Context = engineContext;
            return;
        }

        _rightContext = null;
        MyFsr.DisposeContext(engineContext);
    }
}
