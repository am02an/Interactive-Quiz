using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;

public class PlayFabManager : MonoBehaviour
{
    public static PlayFabManager Instance;

    [Header("Scenes")]
    public string mainGameScene = "QuizScene";
    public string registrationScene = "RegistrationScene";

 

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Always send the user to registration first
        UnityEngine.SceneManagement.SceneManager.LoadScene(registrationScene);
    }

    /// <summary>
    /// Called after the user submits the registration form.
    /// This will log in (create account if needed) and save their new data.
    /// </summary>
    public void RegisterAndStartQuiz(PlayerSessionData sessionData)
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = System.Guid.NewGuid().ToString(), // New account every time
            CreateAccount = true
        };

        PlayFabClientAPI.LoginWithCustomID(request,
            result =>
            {
                Debug.Log("✅ New account created! PlayFabId: " + result.PlayFabId);

                var displayNameRequest = new UpdateUserTitleDisplayNameRequest
                {
                    DisplayName = sessionData.name
                };

                PlayFabClientAPI.UpdateUserTitleDisplayName(displayNameRequest,
                    displayResult =>
                    {
                        Debug.Log("✅ Display Name set to: " + sessionData.name);

                        SavePlayerData(sessionData, () =>
                        {
                            SessionManager.Instance.CurrentSession = sessionData;
                            LocalDB.SaveSession(sessionData);

                            Debug.Log("🎯 Registration complete. Moving to quiz...");
                            UnityEngine.SceneManagement.SceneManager.LoadScene(mainGameScene);
                        });
                    },
                    displayError =>
                    {
                        Debug.LogError("❌ Failed to set Display Name: " + displayError.GenerateErrorReport());
                    });
            },
            error =>
            {
                Debug.LogError("❌ Registration/Login failed: " + error.GenerateErrorReport());
            });
    }



    #region SAVE PLAYER DATA
    private void SavePlayerData(PlayerSessionData data, System.Action onSuccess)
    {
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                { "PlayerName", data.name },
                { "Mobile", data.mobile },
                { "Class", data.className },
                { "Email", data.email },
                { "PlayTime", data.playTime },
                { "Score", data.score.ToString() }
            }
        };

        PlayFabClientAPI.UpdateUserData(request,
            result =>
            {
                Debug.Log("✅ Player data saved to cloud.");
                onSuccess?.Invoke();
            },
            error =>
            {
                Debug.LogError("❌ Failed to save player data: " + error.GenerateErrorReport());
            });
    }
    #endregion
}
