using System.IO;
using Newtonsoft.Json;

public sealed class SketchfabGltfModel {
    
    public byte[] DownloadedData { get; }
    
    public SketchfabGltfModel(byte[] downloadedData) => DownloadedData = downloadedData;
}

public sealed class DownloadedSketchfabModel {
    public SketchfabModelMetadata ModelMetadata { get;}
    public SketchfabGltfModel GltfModel { get; }

    public DownloadedSketchfabModel(SketchfabModelMetadata modelMetadata, SketchfabGltfModel gltfModel) {
        ModelMetadata = modelMetadata;
        GltfModel = gltfModel;
    }

    // TODO DG WIP: 
    // - Async Serialize / Deserialize
    // - Centralized serialization
    // - Serialize as folder
    // - Serialize as zip
    // - Serialize GltfModel only (without metadata)
    
    public void SerializeAsJson(string filePath) {
        var serializer = new JsonSerializer();

        using var streamWriter = new StreamWriter(filePath);
        using var jsonWriter = new JsonTextWriter(streamWriter);
        serializer.Serialize(jsonWriter, this);
    }

    public static DownloadedSketchfabModel DeserializeFromJson(string filePath) {
        var serializer = new JsonSerializer();
        
        using var streamReader = new StreamReader(filePath);
        using var jsonReader = new JsonTextReader(streamReader);
        return serializer.Deserialize<DownloadedSketchfabModel>(jsonReader);
    }
}
