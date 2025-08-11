using UnityEngine;
using System.IO;

public static class CertificateRenderer
{
    public static string SaveCertificatePNG(Camera cam, string filename)
    {
        RenderTexture rt = new RenderTexture(1920, 1080, 24);
        cam.targetTexture = rt;
        Texture2D screenshot = new Texture2D(1920, 1080, TextureFormat.RGB24, false);
        cam.Render();
        RenderTexture.active = rt;
        screenshot.ReadPixels(new Rect(0, 0, 1920, 1080), 0, 0);
        screenshot.Apply();

        byte[] bytes = screenshot.EncodeToPNG();
        string path = Path.Combine(Application.persistentDataPath, filename);
        File.WriteAllBytes(path, bytes);

        cam.targetTexture = null;
        RenderTexture.active = null;
        Object.Destroy(rt);

        return path;
    }
}
