using ZXing;
using ZXing.Common;  // Required for PixelData
using ZXing.QrCode;
using UnityEngine;

public static class QRGenerator
{
    #region Public Methods
    public static Texture2D GenerateQRCode(string text)
    {
        var qrWriter = new BarcodeWriterPixelData
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
                Height = 256,
                Width = 256
            }
        };

        var pixelData = qrWriter.Write(text);

        var tex = new Texture2D(pixelData.Width, pixelData.Height, TextureFormat.RGBA32, false);
        tex.LoadRawTextureData(pixelData.Pixels);
        tex.Apply();

        return tex;
    }
    #endregion
}
