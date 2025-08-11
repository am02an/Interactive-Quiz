using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using System.Collections;

public class QuizController : MonoBehaviour
{
    public QuizView view;
    private int currentIndex = 0;
    private float timeLeft = 60f;
    private int score = 0;
    private bool isAnswering = false;

    void Start()
    {
        LoadQuestions();
        ShowQuestion();
    }

    void Update()
    {
        if (!isAnswering) // Pause timer during feedback
        {
            timeLeft -= Time.deltaTime;
            GameEvents.OnTimerUpdated?.Invoke(timeLeft);
        }

        if (timeLeft <= 0) EndQuiz();
    }

    void LoadQuestions()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("Questions");
        if (jsonFile == null)
        {
            Debug.LogError("Questions.json not found in Resources folder.");
            return;
        }

        SessionManager.Instance.QuestionsPool = JsonConvert.DeserializeObject<Question[]>(jsonFile.text);
    }

    void ShowQuestion()
    {
        if (currentIndex >= 5)
        {
            EndQuiz();
            return;
        }

        isAnswering = false;
        view.DisplayQuestion(SessionManager.Instance.QuestionsPool[currentIndex], OnAnswerSelected);
    }

    void OnAnswerSelected(bool isCorrect)
    {
        isAnswering = true;

        if (isCorrect)
        {
            score++;
            view.ShowAnswerFeedback(true); // ✅ Green highlight
        }
        else
        {
            view.ShowAnswerFeedback(false); // ❌ Red highlight
        }

        GameEvents.OnScoreChanged?.Invoke(score);

        // Wait 1.2 seconds so player sees feedback before next question
        StartCoroutine(NextQuestionAfterDelay(1.2f));
    }

    IEnumerator NextQuestionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        currentIndex++;
        ShowQuestion();
    }

    void EndQuiz()
    {
        var data = SessionManager.Instance.CurrentSession;
        data.score = score;
        LocalDB.SaveSession(data);
        SceneManager.LoadScene("CertificateScene");
    }
}
