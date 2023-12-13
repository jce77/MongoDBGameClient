using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;

public class RegistrationUI : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public TMP_InputField confirmPasswordInput;
    public TMP_InputField emailInput;
    public Button registerButton;
    public TextMeshProUGUI feedbackText;

    public TMP_InputField[] inputFields;

    public void Init()
    {
        ShowFeedbackMessage(UserAccount.FeedbackMessage.None);
    }

    private void Start()
    {
        /// testing validity checking on server side
        //MongoDB.l.account.TryRegisterUser("fglasdfnasdlfdfsfsdfsdf$#ihasdlfiasldfiuasdbnfalsdif", "Testing1!", "testing@testing.com");
        //MongoDB.l.account.TryRegisterUser("validusername1", "Testing1!adfaiusdhfaisdufhasdifuasdifuasdfan", "testing@testing.com");
        //MongoDB.l.account.TryRegisterUser("validusername1", "Testing1!", "not an email address at all");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            UIControl.SwitchToNextInput(inputFields);
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            ClickedRegister();
        }
    }

    public void TestInit()
    {
        
        usernameInput.text = "username1";
        passwordInput.text = "Password11";
        confirmPasswordInput.text = "Password11";
        emailInput.text = "email@email1.com";
        ClickedRegister();
    }

    public void ShowServerFeedbackMessage(UserAccount.ErrorMessage code)
    {
        feedbackText.text = code.message;
    }

    

    public void ShowFeedbackMessage(string feedbackMsg)
    {
        feedbackText.text = feedbackMsg;
        Debug.Log(feedbackMsg);
    }

    public void ShowFeedbackMessage(UserAccount.FeedbackMessage feedbackMsg)
    {
        ShowFeedbackMessage(UserAccount.l.GetFeedbackMessageText(feedbackMsg));
    }

    public void ClickedRegister()
    {
        string username = usernameInput.text.Trim();
        string password = passwordInput.text.Trim();
        string confirmPassword = confirmPasswordInput.text.Trim();
        string email = emailInput.text.Trim();

        #region Input validity checking

        // contains odd characters
        if (UserAccount.ContainsOddCharacters(username) ||
            UserAccount.ContainsOddCharacters(password))
        {
            ShowFeedbackMessage(UserAccount.FeedbackMessage.ContainsOddCharacters);
            return;
        }

        // passwords not equal
        if (password != confirmPassword)
        {
            ShowFeedbackMessage(UserAccount.FeedbackMessage.PasswordsDontMatch);
            return;
        }

        // username not valid
        if (!UserAccount.l.UsernameIsValid(username, UIControl.Type.Registration))
        {
            // feedback message is inside the function for this one
            return;
        }

        // password not valid
        if(!UserAccount.l.PasswordIsValid(password, UIControl.Type.Registration)) 
        {
            // feedback message is inside the function for this one
            return;
        }
        if (!UserAccount.EmailIsValid(email))
        {
            ShowFeedbackMessage(UserAccount.FeedbackMessage.EmailNotValid); 
            return;
        }

        #endregion

        MongoDB.l.account.TryRegisterUser(username, password, email);
    }
}
