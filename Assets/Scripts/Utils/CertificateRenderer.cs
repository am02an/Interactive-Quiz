using UnityEngine;
using System.IO;

public class CertificateRenderer : MonoBehaviour
{
    #region Fields and Properties
    public Canvas certificateCanvas; // Assign your certificate UI Canvas here
    #endregion

    #region Public Methods
    public void SaveCertificatePNG(string filePath)
    {
        StartCoroutine(CaptureUI(filePath));
    }
    #endregion

    #region Private Methods
    private System.Collections.IEnumerator CaptureUI(string filePath)
    {
        yield return new WaitForEndOfFrame(); // Wait for UI to render

        // Create a texture the size of the screen
        Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);

        // Read screen pixels into the texture
        texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        texture.Apply();

        // Save as PNG
        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes(filePath, bytes);

        Debug.Log($"✅ Certificate saved with UI at: {filePath}");
    }
    #endregion
}
