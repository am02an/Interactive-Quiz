using UnityEngine;
using UnityEngine.SceneManagement;

public class RegistrationController : MonoBehaviour
{
    #region Fields and Properties
    public RegistrationView view;
    #endregion

    #region Public Methods
    public void OnRegisterButton()
    {
        var data = new PlayerSessionData
        {
            name = view.nameInput.text.Trim(),
            className = view.classInput.text.Trim(),
            mobile = view.mobileInput.text.Trim(),
            email = view.emailInput.text.Trim(),
            playTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            score = 0
        };

        SessionManager.Instance.CurrentSession = data;
        LocalDB.SaveSession(data);

        SceneManager.LoadScene("QuizScene");
    }
    #endregion
}
