using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using System.Collections;

public class QuizController : MonoBehaviour
{
    #region Fields and Properties
    public QuizView view;

    private int currentIndex = 0;
    private float timeLeft = 60f;
    private int score = 0;
    private bool isAnswering = false;
    #endregion

    #region Unity Callbacks
    private void Start()
    {
        LoadQuestions();
        ShowQuestion();
    }

    private void Update()
    {
        if (!isAnswering) // Pause timer during feedback
        {
            timeLeft -= Time.deltaTime;
            GameEvents.OnTimerUpdated?.Invoke(timeLeft);
        }

        if (timeLeft <= 0)
        {
            EndQuiz();
        }
    }
    #endregion

    #region Quiz Logic
    private void LoadQuestions()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("Questions");
        if (jsonFile == null)
        {
            Debug.LogError("Questions.json not found in Resources folder.");
            return;
        }

        SessionManager.Instance.QuestionsPool = JsonConvert.DeserializeObject<Question[]>(jsonFile.text);
    }

    private void ShowQuestion()
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

    private void OnAnswerSelected(int selectedIndex) // Pass index instead of bool
    {
        isAnswering = true;

        Question currentQuestion = SessionManager.Instance.QuestionsPool[currentIndex];

        bool isCorrect = selectedIndex == currentQuestion.correctIndex;
        if (isCorrect)
        {
            var data = SessionManager.Instance.CurrentSession;
            score++;
            data.score = score;
        }

        // Tell the view to color the buttons
        view.ShowOptionFeedback(selectedIndex, currentQuestion.correctIndex);

        GameEvents.OnScoreChanged?.Invoke(score);

        StartCoroutine(NextQuestionAfterDelay(1.2f));
    }

    private IEnumerator NextQuestionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        currentIndex++;
        ShowQuestion();
    }

    private void EndQuiz()
    {
       
        SceneManager.LoadScene("CertificateScene");
    }
    #endregion
}
