using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Collections;

public class CertificateView : MonoBehaviour
{
    [Header("UI References")]
    public RectTransform certificateArea; // Assign the full certificate panel
    public TextMeshProUGUI nameText, classText, scoreText, dateText, emailText, headingText, contextText;
    public RawImage qrImage;

    public void ShowCertificate(PlayerSessionData data)
    {
        // Heading
        headingText.text = "<color=#FFD700><b>Certificate of Participation</b></color>";

        // Context
        contextText.text = "This certifies that the following participant has successfully completed the quiz.";

        // Dynamic Data
        nameText.text = "<color=#00BFFF>Name:</color> <color=#FFFFFF>  " + data.name + "</color>";
        emailText.text = "<color=#00BFFF>Email:</color> <color=#FFFFFF>  " + data.email + "</color>";
        scoreText.text = "<color=#00BFFF>Score:</color> <color=#FFFFFF>  " + data.score + "</color>";
        dateText.text = "<color=#00BFFF>Date:</color> <color=#FFFFFF>  " + data.playTime + "</color>";
        classText.text = "<color=#00BFFF>Class:</color> <color=#FFFFFF>  " + data.className + "</color>";

        // Generate QR Code (make sure QRImage is inside certificateArea in hierarchy)
        qrImage.texture = QRGenerator.GenerateQRCode(
            $"Name: {data.name}\nClass: {data.className}\nEmail: {data.email}\nScore: {data.score}\nDate: {data.playTime}"
        );

        // Capture after rendering
        StartCoroutine(CaptureWithQR(data));
    }

    private IEnumerator CaptureWithQR(PlayerSessionData data)
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame(); // Extra frame to ensure QR renders
        CaptureCertificatePNG(data);
    }

    private void CaptureCertificatePNG(PlayerSessionData data)
    {
        // Convert UI panel rect to pixel space
        Vector3[] corners = new Vector3[4];
        certificateArea.GetWorldCorners(corners);

        float x = corners[0].x;
        float y = corners[0].y;
        float width = corners[2].x - corners[0].x;
        float height = corners[2].y - corners[0].y;

        // Flip Y (screen coords start at bottom left)
        y = Screen.height - y - height;

        // Create texture and read pixels
        Texture2D tex = new Texture2D((int)width, (int)height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(x, y, width, height), 0, 0);
        tex.Apply();

        // Save PNG
        string savePath = Path.Combine(Application.persistentDataPath, $"certificate_{data.name}_{System.DateTime.Now:yyyyMMdd_HHmmss}.png");
        File.WriteAllBytes(savePath, tex.EncodeToPNG());

        Debug.Log("✅ Certificate saved at: " + savePath);

#if UNITY_STANDALONE_WIN
        System.Diagnostics.Process.Start("explorer.exe", "/select," + savePath.Replace("/", "\\"));
#endif
    }
}
