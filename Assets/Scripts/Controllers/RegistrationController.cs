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

        if (string.IsNullOrEmpty(playerName) || string.IsNullOrEmpty(playerClass) || string.IsNullOrEmpty(mobile) || string.IsNullOrEmpty(email))
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

        // Pass session data to PlayFabManager and only save locally after cloud success
        PlayFabManager.Instance.LoginOrRegister(sessionData, success =>
        {
            if (success)
            {
                Debug.Log("✅ Player registered/logged in and data synced with cloud.");
                UnityEngine.SceneManagement.SceneManager.LoadScene("QuizScene");
            }
            else
            {
                Debug.LogError("❌ Failed to register or log in player. Data not saved locally.");
            }
        });
    }
}
