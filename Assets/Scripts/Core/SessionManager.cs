using UnityEngine;

public class SessionManager : MonoBehaviour
{
    #region Singleton
    public static SessionManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    #region Fields and Properties
    public PlayerSessionData CurrentSession;
    public Question[] QuestionsPool;
    #endregion
}
