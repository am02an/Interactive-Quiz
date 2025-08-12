using UnityEngine;

public class RegistrationController : MonoBehaviour
{
    public RegistrationView view; // UI input fields

    public void OnRegisterButton()
    {
        string playerName = view.nameInput.text.Trim();
        string playerClass = view.classInput.text.Trim();
        string mobile = view.mobileInput.text.Trim();
        string email = view.emailInput.text.Trim();

        if (string.IsNullOrEmpty(playerName) || string.IsNullOrEmpty(playerClass) ||
            string.IsNullOrEmpty(mobile) || string.IsNullOrEmpty(email))
        {
            Debug.LogWarning("⚠ Please fill in all fields.");
            return;
        }

        // Create session data object
        var sessionData = new PlayerSessionData
        {
            name = playerName,
            className = playerClass,
            mobile = mobile,
            email = email,
            playTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            score = 0
        };

        // Call PlayFabManager to register and move to quiz
        PlayFabManager.Instance.RegisterAndStartQuiz(sessionData);
    }
}
