using System.Threading.Tasks;
using UnityEngine;

public static partial class SketchfabModelImporter {
    
    public static Task<DownloadedSketchfabModel> Download(SketchfabModelMetadata model) {
        var result = new TaskCompletionSource<DownloadedSketchfabModel>();
        Download(model, res => result.SetResult(res));
        return result.Task;
    }
    
    public static Task<GameObject> Import(DownloadedSketchfabModel downloaded) {
        var result = new TaskCompletionSource<GameObject>();
        Import(downloaded, res => result.SetResult(res));
        return result.Task;
    }
    
    public static Task<GameObject> DownloadAndImport(SketchfabModelMetadata model, bool enableCache = false) {
        var result = new TaskCompletionSource<GameObject>();
        DownloadAndImport(model, res => result.SetResult(res), enableCache);
        return result.Task;
    }
}