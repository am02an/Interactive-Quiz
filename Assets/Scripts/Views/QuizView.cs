using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
public class QuizView : MonoBehaviour
{
    public TextMeshProUGUI questionText;
    public Button[] optionButtons;
    public TextMeshProUGUI timerText;
    public Image timerBar;
    public TextMeshProUGUI scoreText;
    public Image feedbackPanel; 
    public TextMeshProUGUI feedbackText;
    void OnEnable()
    {
        GameEvents.OnTimerUpdated += UpdateTimer;
        GameEvents.OnScoreChanged += UpdateScore;
    }

    void OnDisable()
    {
        GameEvents.OnTimerUpdated -= UpdateTimer;
        GameEvents.OnScoreChanged -= UpdateScore;
    }

    public void DisplayQuestion(Question q, Action<bool> callback)
    {
        questionText.text = q.questionText;
        for (int i = 0; i < optionButtons.Length; i++)
        {
            optionButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = q.options[i];
            int idx = i;
            optionButtons[i].onClick.RemoveAllListeners();
            optionButtons[i].onClick.AddListener(() => {
                callback(idx == q.correctIndex);
            });
        }
    }

    void UpdateTimer(float t)
    {
        timerText.text = Mathf.CeilToInt(t).ToString();
        timerBar.fillAmount = t / 60f;
    }

    void UpdateScore(int s)
    {
        scoreText.text = "Score: " + s;
    }
    public void ShowAnswerFeedback(bool isCorrect)
    {
        if (isCorrect)
        {
            // Change background to green
            feedbackPanel.color = Color.green;
            feedbackText.text = "✅ Correct!";
        }
        else
        {
            // Change background to red
            feedbackPanel.color = Color.red;
            feedbackText.text = "❌ Wrong!";
        }
    }

}
