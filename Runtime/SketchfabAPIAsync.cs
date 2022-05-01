using System.Threading.Tasks;

public static partial class SketchfabAPI {

    public static Task<SketchfabResponse<SketchfabModelMetadata>> GetModel(string modelUid, bool enableCache = false) {
        var result = new TaskCompletionSource<SketchfabResponse<SketchfabModelMetadata>>();
        GetModel(modelUid, res => result.SetResult(res), enableCache);
        return result.Task;
    }
}