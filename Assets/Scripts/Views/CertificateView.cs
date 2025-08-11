using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class CertificateView : MonoBehaviour
{
    public TextMeshProUGUI nameText, classText, scoreText, dateText;
    public RawImage qrImage;

    public void ShowCertificate(PlayerSessionData data)
    {
        nameText.text = data.name;
        classText.text = data.className;
        scoreText.text = data.score.ToString();
        dateText.text = data.playTime;

        string certPath = CertificateRenderer.SaveCertificatePNG(Camera.main, "certificate.png");
        qrImage.texture = QRGenerator.GenerateQRCode(certPath);
    }
}
