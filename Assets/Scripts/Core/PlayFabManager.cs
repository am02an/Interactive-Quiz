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

    private bool sessionAlreadySaved = false; // Prevents duplicate saves

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
        AutoLogin();
    }

    #region AUTO LOGIN
    public void AutoLogin()
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = false
        };

        PlayFabClientAPI.LoginWithCustomID(request,
            result =>
            {
                Debug.Log("✅ Auto-login successful! PlayFabId: " + result.PlayFabId);

                LoadPlayerDataFromCloud(data =>
                {
                    if (data != null)
                    {
                        SessionManager.Instance.CurrentSession = data;
                        LocalDB.SaveSession(data);
                        Debug.Log("👤 Player session restored. Skipping registration.");
                        UnityEngine.SceneManagement.SceneManager.LoadScene(mainGameScene);
                    }
                    else
                    {
                        Debug.Log("⚠ No cloud data found. Sending to registration.");
                        UnityEngine.SceneManagement.SceneManager.LoadScene(registrationScene);
                    }
                });
            },
            error =>
            {
                Debug.LogWarning("⚠ Auto-login failed: " + error.GenerateErrorReport());
                UnityEngine.SceneManagement.SceneManager.LoadScene(registrationScene);
            });
    }
    #endregion

    #region LOGIN OR REGISTER
    public void LoginOrRegister(PlayerSessionData sessionData = null, System.Action<bool> onComplete = null)
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true
        };

        PlayFabClientAPI.LoginWithCustomID(request,
            result =>
            {
                Debug.Log("✅ Logged in! PlayFabId: " + result.PlayFabId);

                if (result.NewlyCreated)
                {
                    Debug.Log("🆕 New account detected. Saving player data...");
                    SavePlayerData(sessionData, () =>
                    {
                        SaveSessionLocallyOnce(sessionData);
                        onComplete?.Invoke(true);
                    });
                }
                else
                {
                    Debug.Log("👤 Existing player. Loading cloud data...");
                    LoadPlayerDataFromCloud(data =>
                    {
                        if (data != null)
                        {
                            SaveSessionLocallyOnce(data);
                            onComplete?.Invoke(true);
                        }
                        else
                        {
                            Debug.LogWarning("⚠ No cloud data found for this player.");
                            onComplete?.Invoke(false);
                        }
                    });
                }
            },
            error =>
            {
                Debug.LogError("❌ Login/Register failed: " + error.GenerateErrorReport());
                onComplete?.Invoke(false);
            });
    }
    #endregion

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

    private void SaveSessionLocallyOnce(PlayerSessionData data)
    {
        if (sessionAlreadySaved) return;
        SessionManager.Instance.CurrentSession = data;
        LocalDB.SaveSession(data);
        sessionAlreadySaved = true;
    }
    #endregion

    #region LOAD PLAYER DATA
    private void LoadPlayerDataFromCloud(System.Action<PlayerSessionData> onComplete)
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(),
            result =>
            {
                if (result.Data != null && result.Data.Count > 0)
                {
                    var data = new PlayerSessionData
                    {
                        name = result.Data["PlayerName"].Value,
                        mobile = result.Data["Mobile"].Value,
                        className = result.Data["Class"].Value,
                        email = result.Data["Email"].Value,
                        playTime = result.Data["PlayTime"].Value,
                        score = int.Parse(result.Data["Score"].Value)
                    };
                    Debug.Log("✅ Player data loaded from cloud.");
                    onComplete?.Invoke(data);
                }
                else
                {
                    onComplete?.Invoke(null);
                }
            },
            error =>
            {
                Debug.LogError("❌ Failed to load player data: " + error.GenerateErrorReport());
                onComplete?.Invoke(null);
            });
    }
    #endregion
}
