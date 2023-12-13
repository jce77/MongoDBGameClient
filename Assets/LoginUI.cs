using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public Button loginButton;

    public TMP_InputField[] inputFields;
    public TextMeshProUGUI feedbackText;
    public void Init()
    {
        feedbackText.text = "";
    }

    public void ShowServerFeedbackMessage(string message)
    {
        feedbackText.text = message;
    }

    public void ClickedLoginButton()
    {
        string usernameOrEmail = usernameInput.text.Trim();
        string password = passwordInput.text.Trim();

        Debug.Log("ClickedLoginButton");

        #region Input validity checking

        // contains odd characters
        if (UserAccount.ContainsOddCharacters(usernameOrEmail) ||
            UserAccount.ContainsOddCharacters(password))
        {
            ShowFeedbackMessage(UserAccount.FeedbackMessage.ContainsOddCharacters);
            return;
        }

        // username not valid
        if (!UserAccount.l.UsernameIsValid(usernameOrEmail, UIControl.Type.Login))
        {
            // feedback message is inside the function for this one
            return;
        }

        // password not valid
        if (!UserAccount.l.PasswordIsValid(password, UIControl.Type.Login))
        {
            // feedback message is inside the function for this one
            return;
        }

        #endregion

        MongoDB.l.account.TryAuthenticateUser(usernameOrEmail, password);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            UIControl.SwitchToNextInput(inputFields);
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            ClickedLoginButton();
        }
    }


    #region Functions

    public void ShowFeedbackMessage(string feedbackMsg)
    {
        feedbackText.text = feedbackMsg;
    }

    public void ShowFeedbackMessage(UserAccount.FeedbackMessage feedbackMsg)
    {
        ShowFeedbackMessage(UserAccount.l.GetFeedbackMessageText(feedbackMsg));
    }

    #endregion
}
