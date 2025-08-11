using UnityEngine;

public class CertificateController : MonoBehaviour
{
    public CertificateView view;

    void Start()
    {
        var data = LocalDB.LoadSession();
        if (data != null)
        {
            view.ShowCertificate(data);
        }
    }
}
