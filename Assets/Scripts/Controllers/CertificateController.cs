using UnityEngine;

public class CertificateController : MonoBehaviour
{
    #region Fields and Properties
    public CertificateView view;
    #endregion

    #region Unity Callbacks
    private void Start()
    {
        LoadAndShowCertificate();
    }
    #endregion

    #region Methods
    private void LoadAndShowCertificate()
    {
        var data = LocalDB.LoadSession();
        if (data != null)
        {
            view.ShowCertificate(data);
        }
    }
    #endregion
}
