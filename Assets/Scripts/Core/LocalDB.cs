using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;

public static class LocalDB
{
    #region Constants
    private const string SessionKey = "Quiz_Session";
    #endregion

    #region Save to Local + PlayFab
    public static void SaveSession(PlayerSessionData data)
    {
        // Save locally
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(SessionKey, json);
        PlayerPrefs.Save();

        // Always overwrite cloud values — no need to compare first
        var updateData = new Dictionary<string, string>
    {
        { "PlayerName", data.name },
        { "Class", data.className },
        { "Mobile", data.mobile },
        { "Email", data.email },
        { "PlayTime", data.playTime },
        { "Score", data.score.ToString() } // Always update score
    };

        var request = new UpdateUserDataRequest
        {
            Data = updateData
        };

        PlayFabClientAPI.UpdateUserData(request,
            updateResult =>
            {
                Debug.Log("✅ Player data saved to PlayFab cloud.");
            },
            error =>
            {
                Debug.LogError("❌ Failed to save data to PlayFab: " + error.GenerateErrorReport());
            });
    }

    #endregion

    #region Load from PlayFab or Local
    public static void LoadSession(Action<PlayerSessionData> onLoaded)
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(),
            result =>
            {
                if (result.Data != null && result.Data.Count > 0)
                {
                    PlayerSessionData data = new PlayerSessionData
                    {
                        name = GetValue(result.Data, "PlayerName"),
                        className = GetValue(result.Data, "Class"),
                        mobile = GetValue(result.Data, "Mobile"),
                        email = GetValue(result.Data, "Email"),
                        playTime = GetValue(result.Data, "PlayTime"),
                        score = int.TryParse(GetValue(result.Data, "Score"), out int s) ? s : 0
                    };

                // Cache locally (only overwrites if data actually changed)
                SaveSession(data);

                    onLoaded?.Invoke(data);
                }
                else
                {
                    Debug.LogWarning("⚠ No cloud data found, loading local.");
                    onLoaded?.Invoke(LoadLocalOnly());
                }
            },
            error =>
            {
                Debug.LogWarning("⚠ Failed to get cloud data, using local: " + error.GenerateErrorReport());
                onLoaded?.Invoke(LoadLocalOnly());
            });
    }

    #endregion

    #region Local Only Load
    private static PlayerSessionData LoadLocalOnly()
    {
        if (PlayerPrefs.HasKey(SessionKey))
        {
            return JsonUtility.FromJson<PlayerSessionData>(PlayerPrefs.GetString(SessionKey));
        }
        return null;
    }
    #endregion

    #region Helper
    private static string GetValue(Dictionary<string, UserDataRecord> data, string key)
    {
        return data.ContainsKey(key) ? data[key].Value : "";
    }
    #endregion
}
