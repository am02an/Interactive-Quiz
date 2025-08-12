using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;

public class BunnyUploader : MonoBehaviour
{
    #region BunnyCDN Config
    private string storageZoneName = "mycertificat";
    private string CDN = "mycertificate";
    private string apiAccessKey = "224866d4-3010-4796-871366648803-aa9f-4dfa";
    #endregion

    #region Public Methods
    /// <summary>
    /// Upload a file to BunnyCDN storage.
    /// remoteFileName should include folder path, e.g., "Certificate/cert1.png"
    /// </summary>
    public IEnumerator UploadFile(string localFilePath, string remoteFileName, System.Action<bool, string> callback)
    {
        if (!File.Exists(localFilePath))
        {
            Debug.LogError("File not found: " + localFilePath);
            callback(false, null);
            yield break;
        }

        byte[] fileData = File.ReadAllBytes(localFilePath);

        // URL with folder path included in remoteFileName
        string url = $"https://storage.bunnycdn.com/{storageZoneName}/{remoteFileName}";

        UnityWebRequest www = UnityWebRequest.Put(url, fileData);

        // Set required headers
        www.SetRequestHeader("AccessKey", apiAccessKey);
        www.SetRequestHeader("Content-Type", "image/png");

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("File uploaded successfully: " + remoteFileName);

            // Construct the public URL, matching BunnyCDN public hostname and path
            string publicUrl = $"https://{CDN}.b-cdn.net/{remoteFileName}";
            callback(true, publicUrl);
        }
        else
        {
            Debug.LogError("Upload failed: " + www.error);
            callback(false, null);
        }
    }
    #endregion
}
