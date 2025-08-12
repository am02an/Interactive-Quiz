using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class QuizView : MonoBehaviour
{
    #region UI References
    public TextMeshProUGUI questionText;
    public Button[] optionButtons;
    public TextMeshProUGUI timerText;
    public Image timerBar;
    public TextMeshProUGUI scoreText;
    public Image feedbackPanel;
    public TextMeshProUGUI feedbackText;
    #endregion

    #region Unity Callbacks
    private void OnEnable()
    {
        GameEvents.OnTimerUpdated += UpdateTimer;
        GameEvents.OnScoreChanged += UpdateScore;
    }

    private void OnDisable()
    {
        GameEvents.OnTimerUpdated -= UpdateTimer;
        GameEvents.OnScoreChanged -= UpdateScore;
    }
    #endregion

    #region Public Methods
    public void DisplayQuestion(Question q, Action<int> onOptionSelected)
    {
        // Set question text
        questionText.text = q.questionText;

        // Set options
        for (int i = 0; i < optionButtons.Length; i++)
        {
            optionButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = q.options[i];
            optionButtons[i].interactable = true;
            optionButtons[i].image.color = Color.white; // reset color

            int index = i; // local copy for lambda
            optionButtons[i].onClick.RemoveAllListeners();
            optionButtons[i].onClick.AddListener(() => onOptionSelected(index));
        }
    }

    public void ShowOptionFeedback(int selectedIndex, int correctIndex)
    {
        // Disable buttons to prevent extra clicks
        foreach (var btn in optionButtons)
            btn.interactable = false;

        // Color the selected button
        if (selectedIndex == correctIndex)
        {
            optionButtons[selectedIndex].image.color = Color.green;
        }
        else
        {
            optionButtons[selectedIndex].image.color = Color.red;
            optionButtons[correctIndex].image.color = Color.green; // show correct one
        }
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
    #endregion

    #region Private Methods
    private void UpdateTimer(float t)
    {
        timerText.text = Mathf.CeilToInt(t).ToString();
        timerBar.fillAmount = t / 60f;
    }

    private void UpdateScore(int s)
    {
        scoreText.text = "Score: " + s;
    }
    #endregion
}
