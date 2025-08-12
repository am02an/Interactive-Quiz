using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;

public class BunnyUploader : MonoBehaviour
{
    #region BunnyCDN Config
    private string storageZoneName = "mycertificates";
    private string CDN = "QuizCertificate";
    private string apiAccessKey = "ad5e5f22-0617-41d4-8c5b6a1b6cad-80ad-49a0";
    #endregion

    #region Public Methods
    /// <summary>
    /// Uploads a file to BunnyCDN in: certificate/{playerName}/{fileName}
    /// </summary>
    public IEnumerator UploadFile(string localFilePath, string playerName, string fileName, System.Action<bool, string> callback)
    {
        if (!File.Exists(localFilePath))
        {
            Debug.LogError("❌ File not found: " + localFilePath);
            callback(false, null);
            yield break;
        }

        byte[] fileData = File.ReadAllBytes(localFilePath);

        // Build folder structure: certificate/{playerName}/{fileName}
        string remotePath = $"certificate/{playerName}/{fileName}";

        // URL for BunnyCDN storage
        string url = $"https://storage.bunnycdn.com/{storageZoneName}/{remotePath}";

        UnityWebRequest www = UnityWebRequest.Put(url, fileData);

        // Set required headers
        www.SetRequestHeader("AccessKey", apiAccessKey);
        www.SetRequestHeader("Content-Type", "application/pdf");

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("✅ File uploaded successfully: " + remotePath);

            // Public URL
            string publicUrl = $"https://{CDN}.b-cdn.net/{remotePath}";
            callback(true, publicUrl);
        }
        else
        {
            Debug.LogError("❌ Upload failed: " + www.error);
            callback(false, null);
        }
    }
    #endregion
}
