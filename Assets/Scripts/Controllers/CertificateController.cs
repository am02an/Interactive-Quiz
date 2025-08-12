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
        // Save latest session data first
        LocalDB.SaveSession(SessionManager.Instance.CurrentSession);

        // Then load and display
                view.ShowCertificate();
        //LocalDB.LoadSession((data) =>
        //{
        //    if (data != null)
        //    {
        //    }
        //    else
        //    {
        //        Debug.LogWarning("⚠ No certificate data found.");
        //    }
        //});
    }
    #endregion
}
