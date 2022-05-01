using System;
using System.IO;
using System.IO.Compression;
using UnityEngine;
using UnityEngine.Networking;
using GLTFast;
using System.Threading.Tasks;

public static partial class SketchfabModelImporter
{
    private const string m_SketchfabModelCacheFolderName = "SketchfabModelCache";
    private const string m_SketchfabModelTemporaryDownloadFolderName = "SketchfabModelTemp";

    private static SketchfabModelDiskCache m_Cache = new SketchfabModelDiskCache(Path.Combine(Application.persistentDataPath, m_SketchfabModelCacheFolderName), 1024 * 1024 * 1024);
    private static SketchfabModelDiskTemp m_Temp = new SketchfabModelDiskTemp(Path.Combine(Application.persistentDataPath, m_SketchfabModelTemporaryDownloadFolderName), 10.0f);

    public static void EnsureInitialized()
    {
        if(m_Cache == null)
        {
            m_Cache = new SketchfabModelDiskCache(Path.Combine(Application.persistentDataPath, m_SketchfabModelCacheFolderName), 1024 * 1024 * 1024);
            m_Temp = m_Temp = new SketchfabModelDiskTemp(Path.Combine(Application.persistentDataPath, m_SketchfabModelTemporaryDownloadFolderName), 5.0f);
        }
    }

    public static async void DownloadAndImport(SketchfabModelMetadata _model, Action<GameObject> _onModelImported, bool _enableCache=false)
    {
        if(_enableCache)
        {
            if (await m_Cache.IsInCache(_model))
            {
                GltfImport($"file://{Path.Combine(m_Cache.AbsolutePath, _model.Uid, "scene.gltf")}", _onModelImported);
                return;
            }
        }

        Download(_model, _downloaded => Import(_downloaded, _onModelImported));
    }

    public static void Download(SketchfabModelMetadata _model, Action<DownloadedSketchfabModel> _onModelDownloaded) {
        SketchfabAPI.GetGLTFModelDownloadUrl(_model.Uid, (SketchfabResponse<string> _modelDownloadUrl) =>
        {
            if (!_modelDownloadUrl.Success)
            {
                Debug.LogError(_modelDownloadUrl.ErrorMessage);

                _onModelDownloaded?.Invoke(null);

                return;
            }

            Download(_model, _modelDownloadUrl.Object, _onModelDownloaded);
        });
    }

    public static void Import(DownloadedSketchfabModel _downloaded, Action<GameObject> _onModelImported)
    {
        if (_downloaded == null)
        {
            _onModelImported?.Invoke(null);

            return;
        }
        
        // Lock the temporary folder for all following operations to
        // avoid it from flushing itself in the middle of it
        m_Temp.Lock();

        try
        {
            string archivePath = Path.Combine(m_Temp.AbsolutePath, _downloaded.ModelMetadata.Uid);
            // Make sure to save again the model if downloaded twice
            if(Directory.Exists(archivePath))
            {
                Directory.Delete(archivePath, true);
            }
        
            using (ZipArchive zipArchive = new ZipArchive(new MemoryStream(_downloaded.GltfModel.DownloadedData), ZipArchiveMode.Read))
            {
                zipArchive.ExtractToDirectory(archivePath);
            }
            
            GltfImport($"file://{Path.Combine(archivePath, "scene.gltf")}", (GameObject _importedModel) =>
            {
                DirectoryInfo gltfDirectoryInfo = new DirectoryInfo(archivePath);
                m_Cache.AddToCache(gltfDirectoryInfo);

                _onModelImported?.Invoke(_importedModel);
            });
        }
        finally
        {
            // No matter what happens, release the lock so that it doesn't get stuck
            m_Temp.Unlock();
        }
    }

    private static void Download(SketchfabModelMetadata _model, string _downloadUrl, Action<DownloadedSketchfabModel> _onModelDownloaded) {
        UnityWebRequest downloadRequest = UnityWebRequest.Get(_downloadUrl);

        SketchfabWebRequestManager.Instance.SendRequest(downloadRequest, (UnityWebRequest _request) => {
            if (downloadRequest.isHttpError ||
                downloadRequest.isNetworkError) {
                Debug.Log(downloadRequest.error);

                _onModelDownloaded?.Invoke(null);

                return;
            }

            _onModelDownloaded?.Invoke(new DownloadedSketchfabModel(_model, new SketchfabGltfModel(downloadRequest.downloadHandler.data)));
        });
    }

    public static void SaveModelMetadataToCache(SketchfabModelMetadata _model)
    {
        m_Cache.CacheModelMetadata(_model);
    }

    private static async void GltfImport(string _gltfFilePath, Action<GameObject> _onModelImported)
    {
        GltfImport gltf = new GltfImport();

        bool success = true;
        try
        {
            success = await gltf.Load(_gltfFilePath);
        }
        catch(Exception ex)
        {
            success = false;
        }


        if (!success)
        {
            _onModelImported?.Invoke(null);

            return;
        }

        GameObject go = new GameObject("SketchfabModel");
        success = gltf.InstantiateMainScene(go.transform);

        if (!success)
        {
            UnityEngine.Object.Destroy(go);
            go = null;
        }

        _onModelImported?.Invoke(go);
    }

    public static Task<bool> IsUidInCache(string _uid)
    {
        return m_Cache.IsInCache(_uid);
    }

    public static Task<SketchfabModelMetadata> GetCachedModelMetadata(string _uid)
    {
        return m_Cache.GetCachedModelMetadata(_uid);
    }
}
