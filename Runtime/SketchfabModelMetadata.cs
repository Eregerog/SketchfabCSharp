using Newtonsoft.Json; 

public class SketchfabModelMetadata
{
    public string Uid { get; set; }
    public string Name { get; set; }
    public bool IsDownloadable { get; set; }
    public string Description { get; set; }
    public int FaceCount { get; set; }
    public int VertexCount { get; set; }
    public SketchfabThumbnailList Thumbnails { get; set; }

    public override string ToString()
    {
        string modelString = $"Uid: {Uid}\n" +
            $"Name: {Name}\n" +
            $"IsDownloadable: {IsDownloadable}\n" +
            $"Description: {Description}\n" +
            $"Face count: {FaceCount}\n" +
            $"Vertex count: {VertexCount}\n" +
            $"Thumbnails: {Thumbnails}";

        return modelString;
    }

    public string GetJsonString ()
    {
        return JsonConvert.SerializeObject(this);
    }

    public static SketchfabModelMetadata FromJson(string _data)
    {
        return JsonConvert.DeserializeObject<SketchfabModelMetadata>(_data);
    }
}
