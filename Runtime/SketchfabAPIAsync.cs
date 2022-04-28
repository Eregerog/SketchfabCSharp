using System.Threading.Tasks;

public static partial class SketchfabAPI {

    public static Task<SketchfabResponse<SketchfabModel>> GetModel(string modelUid, bool enableCache = false) {
        var result = new TaskCompletionSource<SketchfabResponse<SketchfabModel>>();
        GetModel(modelUid, res => result.SetResult(res), enableCache);
        return result.Task;
    }
}