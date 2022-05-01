
public class DownloadedSketchfabModel {
    public SketchfabModelMetadata ModelMetadata { get;}
    public byte[] DownloadedData { get; }

    public DownloadedSketchfabModel(SketchfabModelMetadata modelMetadata, byte[] downloadedData) {
        ModelMetadata = modelMetadata;
        DownloadedData = downloadedData;
    }
}
