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

        // Pick a random question index
        int randomIndex = Random.Range(0, SessionManager.Instance.QuestionsPool.Length);

        isAnswering = false;

        // Show the randomly picked question
        view.DisplayQuestion(SessionManager.Instance.QuestionsPool[randomIndex], OnAnswerSelected);
    }


    void OnAnswerSelected(int selectedIndex) // <-- pass index instead of bool
    {
        isAnswering = true;

        Question currentQuestion = SessionManager.Instance.QuestionsPool[currentIndex];

        bool isCorrect = selectedIndex == currentQuestion.correctIndex;
        if (isCorrect)
        {
            score++;
        }

        // Tell the view to color the buttons
        view.ShowOptionFeedback(selectedIndex, currentQuestion.correctIndex);

        GameEvents.OnScoreChanged?.Invoke(score);

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
