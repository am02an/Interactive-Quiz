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
        LocalDB.LoadSession((data) =>
        {
            if (data != null)
            {
                view.ShowCertificate(data);
            }
            else
            {
                Debug.LogWarning("⚠ No certificate data found.");
            }
        });
    }
    #endregion
}
