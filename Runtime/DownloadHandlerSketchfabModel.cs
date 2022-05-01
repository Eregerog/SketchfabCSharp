using UnityEngine.Networking;

public static class DownloadHandlerSketchfabModel
{
    public static SketchfabResponse<SketchfabModelMetadata> GetModel(UnityWebRequest _unityWebRequest)
    {
        return SketchfabResponse<SketchfabModelMetadata>.FromModelResponse(_unityWebRequest);
    }
}
