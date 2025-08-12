using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;

public class RegistrationView : MonoBehaviour
{
    #region UI References
    public TMP_InputField nameInput;
    public TMP_InputField classInput;
    public TMP_InputField mobileInput;
    public TMP_InputField emailInput;
    public Button submitButton;
    public TextMeshProUGUI errorText;
    #endregion

    #region Private Fields
    private Color originalColor;
    private readonly Color errorColor = new Color(1f, 0.5f, 0.5f); // light red
    #endregion

    #region Unity Callbacks
    private void Start()
    {
        CacheOriginalInputColors();

        AddInputListeners();

        ValidateInputs();
    }
    #endregion

    #region Validation Logic
    private void ValidateInputs()
    {
        errorText.text = "";
        submitButton.interactable = false;

        bool isNameValid = ValidateInputField(nameInput);
        bool isClassValid = ValidateInputField(classInput);
        bool isMobileValid = ValidateMobile(mobileInput.text);
        bool isEmailValid = IsValidEmail(emailInput.text.Trim());

        SetInputFieldColor(nameInput, isNameValid);
        SetInputFieldColor(classInput, isClassValid);
        SetInputFieldColor(mobileInput, isMobileValid);
        SetInputFieldColor(emailInput, isEmailValid);

        if (!isNameValid || !isClassValid || !isMobileValid || !isEmailValid)
        {
            if (!isNameValid || !isClassValid)
                errorText.text = "Please fill in all fields.";
            else if (!isMobileValid)
                errorText.text = "Mobile number must be exactly 10 digits.";
            else if (!isEmailValid)
                errorText.text = "Please enter a valid email address.";

            return;
        }

        submitButton.interactable = true;
    }

    private bool ValidateInputField(TMP_InputField input)
    {
        return !string.IsNullOrWhiteSpace(input.text);
    }

    private bool ValidateMobile(string mobile)
    {
        mobile = mobile.Trim();
        return mobile.Length == 10 && Regex.IsMatch(mobile, @"^\d{10}$");
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            return Regex.IsMatch(email,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                RegexOptions.IgnoreCase);
        }
        catch
        {
            return false;
        }
    }
    #endregion

    #region UI Helpers
    private void SetInputFieldColor(TMP_InputField input, bool isValid)
    {
        var bg = GetInputFieldBackground(input);
        if (bg != null)
        {
            bg.color = isValid ? originalColor : errorColor;
        }
    }

    private Image GetInputFieldBackground(TMP_InputField input)
    {
        if (input == null)
        {
            Debug.LogError("Input field reference is null!");
            return null;
        }

        // Check Image on the same GameObject
        var bg = input.GetComponent<Image>();
        if (bg != null) return bg;

        // Check child named "Bg"
        var childBgTransform = input.transform.Find("Bg");
        if (childBgTransform != null)
        {
            var childBg = childBgTransform.GetComponent<Image>();
            if (childBg != null) return childBg;
            else Debug.LogError($"'Bg' child of {input.name} does not have an Image component!");
        }
        else
        {
            Debug.LogError($"No 'Bg' child found for {input.name}");
        }

        return null;
    }

    private void CacheOriginalInputColors()
    {
        originalColor = GetInputFieldBackground(nameInput).color;
    }

    private void AddInputListeners()
    {
        nameInput.onValueChanged.AddListener(_ => ValidateInputs());
        classInput.onValueChanged.AddListener(_ => ValidateInputs());
        mobileInput.onValueChanged.AddListener(_ => ValidateInputs());
        emailInput.onValueChanged.AddListener(_ => ValidateInputs());
    }
    #endregion
}
