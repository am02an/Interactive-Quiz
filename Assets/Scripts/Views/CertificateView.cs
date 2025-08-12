using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Collections;
using System;
using System.Text;

public class CertificateView : MonoBehaviour
{
    #region UI References
    [Header("UI References")]
    public RectTransform certificateArea;
    public EmailServiceExample emailServiceExample; // Assign the full certificate panel
    public BunnyUploader bunnyUploader;             // Assign the full certificate panel
    public TextMeshProUGUI nameText, classText, scoreText, dateText, emailText, headingText, contextText;
    public RawImage qrImage;
    #endregion

    #region Public Methods
    public void ShowCertificate()
    {

        // Heading
        headingText.text = "<color=#FFD700><b>Certificate of Participation</b></color>";

        // Context
        contextText.text = "This certifies that the following participant has successfully completed the quiz.";
        var data = SessionManager.Instance.CurrentSession;
        Debug.Log($"[Certificate] Incoming score: {data.score}");
        // Dynamic Data
        nameText.text = $"<color=#00BFFF>Name:</color> <color=#FFFFFF>  {data.name}</color>";
        emailText.text = $"<color=#00BFFF>Email:</color> <color=#FFFFFF>  {data.email}</color>";
        scoreText.text = $"<color=#00BFFF>Score:</color> <color=#FFFFFF>  {data.score}</color>";
        dateText.text = $"<color=#00BFFF>Date:</color> <color=#FFFFFF>  {data.playTime}</color>";
        classText.text = $"<color=#00BFFF>Class:</color> <color=#FFFFFF>  {data.className}</color>";

        // Generate QR Code (make sure QRImage is inside certificateArea in hierarchy)
        qrImage.texture = QRGenerator.GenerateQRCode(
            $"Name: {data.name}\nClass: {data.className}\nEmail: {data.email}\nScore: {data.score}\nDate: {data.playTime}"
        );

        // Capture after rendering
        StartCoroutine(CaptureWithQR(data));
    }
    #endregion

    #region Private Coroutines
    private IEnumerator CaptureWithQR(PlayerSessionData data)
    {
        // Wait to ensure UI & QR code fully rendered
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        string savedCertificatePath = null;

        // Capture PDF and store file path
        yield return StartCoroutine(CaptureCertificatePDFCoroutine(data, path =>
        {
            savedCertificatePath = path;
        }));

        if (string.IsNullOrEmpty(savedCertificatePath))
        {
            Debug.LogError("❌ Failed to capture certificate PDF.");
            yield break;
        }

        // Send local PDF via email (before upload if desired)
        emailServiceExample.SendMail(data.email, savedCertificatePath);

        // Extract file name from saved path
        string fileName = Path.GetFileName(savedCertificatePath);

        // Upload PDF to BunnyCDN inside: certificate/{playerName}/{fileName}
        yield return StartCoroutine(bunnyUploader.UploadFile(
            savedCertificatePath,
            data.name,      // player name folder
            fileName,       // PDF file name
            (success, url) =>
            {
                if (success)
                {
                    Debug.Log("✅ Certificate uploaded. Public URL: " + url);

                // Generate QR code from public URL
                qrImage.texture = QRGenerator.GenerateQRCode(url);

                // Optional: email again with public URL
                // emailServiceExample.SendMail(data.email, url);
            }
                else
                {
                    Debug.LogError("❌ Failed to upload certificate.");
                }
            }));
    }


    #endregion

    #region Private Methods
    public IEnumerator CaptureCertificatePDFCoroutine(PlayerSessionData data, Action<string> onComplete)
    {
        // wait for UI to render
        yield return new WaitForEndOfFrame();

        // get world corners and compute rect in screen coords
        Vector3[] corners = new Vector3[4];
        certificateArea.GetWorldCorners(corners);

        float x = corners[0].x;
        float y = corners[0].y;
        float width = corners[2].x - corners[0].x;
        float height = corners[2].y - corners[0].y;

        // convert to readPixels coordinate system
        y = Screen.height - y - height;

        // create texture and read pixels
        Texture2D tex = new Texture2D(Mathf.Max(1, (int)width), Mathf.Max(1, (int)height), TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(x, y, width, height), 0, 0);
        tex.Apply();

        // encode to JPG (must use JPEG for /DCTDecode)
        byte[] jpgBytes = tex.EncodeToJPG(90);
        UnityEngine.Object.Destroy(tex);

        // create pdf path
        string pdfPath = Path.Combine(Application.persistentDataPath,
            $"certificate_{SanitizeFileName(data.name)}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");

        // write PDF
        CreateSimplePDFWithJpeg(pdfPath, jpgBytes, (int)width, (int)height);

        Debug.Log("✅ PDF saved at: " + pdfPath);
#if UNITY_STANDALONE_WIN
        System.Diagnostics.Process.Start("explorer.exe", "/select," + pdfPath.Replace("/", "\\"));
#endif

        onComplete?.Invoke(pdfPath);
    }

    // minimal sanitizer for filename
    private string SanitizeFileName(string s)
    {
        foreach (char c in Path.GetInvalidFileNameChars()) s = s.Replace(c, '_');
        return s;
    }

    // Writes a very small PDF embedding a JPEG image (no external libs).
    private void CreateSimplePDFWithJpeg(string filePath, byte[] jpegBytes, int imgWidth, int imgHeight)
    {
        using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        using (var bw = new BinaryWriter(fs))
        {
            // PDF header (include a binary comment)
            bw.Write(Encoding.ASCII.GetBytes("%PDF-1.4\n%\u00E2\u00E3\u00CF\u00D3\n"));

            long[] offsets = new long[6]; // offsets[1..5] used

            // 1. Catalog
            offsets[1] = fs.Position;
            bw.Write(Encoding.ASCII.GetBytes("1 0 obj\n<< /Type /Catalog /Pages 2 0 R >>\nendobj\n"));

            // 2. Pages
            offsets[2] = fs.Position;
            bw.Write(Encoding.ASCII.GetBytes("2 0 obj\n<< /Type /Pages /Kids [3 0 R] /Count 1 >>\nendobj\n"));

            // 3. Page (MediaBox uses image width/height as points)
            offsets[3] = fs.Position;
            string pageObj = $"3 0 obj\n<< /Type /Page /Parent 2 0 R /MediaBox [0 0 {imgWidth} {imgHeight}] " +
                             $"/Resources << /XObject << /Im0 4 0 R >> /ProcSet [/PDF /ImageC] >> /Contents 5 0 R >>\nendobj\n";
            bw.Write(Encoding.ASCII.GetBytes(pageObj));

            // 4. Image XObject (JPEG bytes, DCTDecode)
            offsets[4] = fs.Position;
            bw.Write(Encoding.ASCII.GetBytes($"4 0 obj\n<< /Type /XObject /Subtype /Image /Width {imgWidth} /Height {imgHeight} " +
                                            $"/ColorSpace /DeviceRGB /BitsPerComponent 8 /Filter /DCTDecode /Length {jpegBytes.Length} >>\n"));
            bw.Write(Encoding.ASCII.GetBytes("stream\n"));
            bw.Write(jpegBytes); // write raw jpeg bytes
            bw.Write(Encoding.ASCII.GetBytes("\nendstream\nendobj\n"));

            // 5. Content stream that paints the image to the page
            offsets[5] = fs.Position;
            string content = $"q\n{imgWidth} 0 0 {imgHeight} 0 0 cm\n/Im0 Do\nQ\n";
            byte[] contentBytes = Encoding.ASCII.GetBytes(content);
            bw.Write(Encoding.ASCII.GetBytes($"5 0 obj\n<< /Length {contentBytes.Length} >>\nstream\n"));
            bw.Write(contentBytes);
            bw.Write(Encoding.ASCII.GetBytes("\nendstream\nendobj\n"));

            // xref
            long xrefPos = fs.Position;
            bw.Write(Encoding.ASCII.GetBytes("xref\n0 6\n0000000000 65535 f \n"));
            for (int i = 1; i <= 5; i++)
            {
                bw.Write(Encoding.ASCII.GetBytes($"{offsets[i]:D10} 00000 n \n"));
            }

            // trailer
            bw.Write(Encoding.ASCII.GetBytes($"trailer\n<< /Size 6 /Root 1 0 R >>\nstartxref\n{xrefPos}\n%%EOF\n"));
        }
    }
    #endregion
}
