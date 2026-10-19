using HarmonyLib;
using Keen.VRage.Library.Mathematics;
using Vortice.Direct3D12;

namespace SE2VR.Client.Wrappers;

/// <summary>
/// My engine texture (BackBuffer, ResizableRWRenderTargetTexture, etc).
/// Used to acccess internal type
/// </summary>
public class MyTexture(object obj)
{
    public object Instance => obj;

    public ID3D12Resource D3DResource => (ID3D12Resource)AccessTools.Property(obj.GetType(), "D3DResource").GetValue(obj)!;
    public Vector2I Resolution => (Vector2I)AccessTools.Property(obj.GetType(), "Resolution").GetValue(obj)!;
    public object AutoResourceState => AccessTools.Property(obj.GetType(), "AutoResourceState").GetValue(obj)!;

    public void Dispose() => ((IDisposable)obj).Dispose();

    public MyTexture CreateEmptyCopy(string nameSuffix)
    {
        object? Field(string name) => AccessTools.Field(obj.GetType(), name).GetValue(obj);

        return CreateRWResizableRenderTargetTexture((string)Field("_debugName")! + nameSuffix, Field("_srvFormat")!, (Vector2I)Field("_maxResolution")!, Field("_uavFormat"), (int)D3DResource.Description.MipLevels, Field("_d3dClearColor"), Field("_allocationGroup"));
    }

    public void Resize(MyCommandList commandList, Vector2I resolution) => AccessTools.Method(obj.GetType(), "Resize").Invoke(obj, [commandList.Instance, resolution]);

    public static MyTexture CreateRWResizableRenderTargetTexture(string name, object srvFormat, Vector2I resolution, object? uavFormat, int mipMaps, object? clearColor, object? allocationGroup)
    {
        object textures = MyCoreSystems.BindableTextures!;
        var method = AccessTools.Method(textures.GetType(), "CreateRWResizableRenderTargetTexture");
        return new MyTexture(method.Invoke(textures, [name, srvFormat, resolution, uavFormat, mipMaps, clearColor, allocationGroup])!);
    }
}
