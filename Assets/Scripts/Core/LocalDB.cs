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

        // First get existing cloud data
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(),
            result =>
            {
                var updateData = new Dictionary<string, string>();

                void CheckAndAdd(string key, string newValue)
                {
                    if (!result.Data.ContainsKey(key) || result.Data[key].Value != newValue)
                    {
                        updateData[key] = newValue; // Only update if value changed or key doesn't exist
                }
                }

                CheckAndAdd("PlayerName", data.name);
                CheckAndAdd("Class", data.className);
                CheckAndAdd("Mobile", data.mobile);
                CheckAndAdd("Email", data.email);
                CheckAndAdd("PlayTime", data.playTime);
                CheckAndAdd("Score", data.score.ToString());


                if (updateData.Count > 0)
                {
                    var request = new UpdateUserDataRequest
                    {
                        Data = updateData
                    };

                    PlayFabClientAPI.UpdateUserData(request,
                        updateResult => Debug.Log("✅ Player data updated to PlayFab cloud."),
                        error => Debug.LogError("❌ Failed to update data to PlayFab: " + error.GenerateErrorReport()));
                }
                else
                {
                    Debug.Log("ℹ No changes detected — skipping cloud update.");
                }
            },
            error =>
            {
                Debug.LogError("❌ Failed to retrieve existing cloud data: " + error.GenerateErrorReport());
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
