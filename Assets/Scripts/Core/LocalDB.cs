using UnityEngine;

public static class LocalDB
{
    #region Constants
    private const string SessionKey = "Quiz_Session";
    #endregion

    #region Public Methods
    public static void SaveSession(PlayerSessionData data)
    {
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(SessionKey, json);
        PlayerPrefs.Save();
    }

    public static PlayerSessionData LoadSession()
    {
        if (PlayerPrefs.HasKey(SessionKey))
        {
            return JsonUtility.FromJson<PlayerSessionData>(PlayerPrefs.GetString(SessionKey));
        }
        return null;
    }
    #endregion
}
