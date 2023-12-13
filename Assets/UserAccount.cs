using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;

public class UserAccount : MonoBehaviour
{
    public static UserAccount l;

    private void Awake()
    {
        if (l != null)
        {
            Destroy(gameObject);
            return;
        }
        l = this;
    }

    #region datatypes

    public class ErrorMessage
    {
        public int code;
        public string message;

        public ErrorMessage(int code, string message)
        {
            this.code = code;
            this.message = message;
        }
    }

    [System.Serializable]
    public class AuthResponse
    {
        public string token;
        public string username;
    }

    #endregion

    public AuthResponse authResponse;

    public RegistrationUI registrationUI;
    public LoginUI loginUI;
    
    [Header("Test Variables")]
    public string username;
    public string password;
    public string email;

    public List<ErrorMessage> errorMessages;

    private void Start()
    {
        InitErrorCodes();
        //MongoDB.l.account.registrationUI.TestInit();
    }

    public void InitErrorCodes()
    {
        errorMessages = new List<ErrorMessage>
        {
            new ErrorMessage(
            200, "Successfully logged in."
            ),

            new ErrorMessage(
            201, "Successfully registered."
            ),

            new ErrorMessage(
            401, "Not currently logged in." // 'Invalid token' on server-side
            ),

            new ErrorMessage(
            403, "Not currently logged in." // 'Token not provided' on server-side
            ),

            new ErrorMessage(
            406, "Password was incorrect."
            ),

            new ErrorMessage(
            407, "Username is not valid and sent by a bot, this incident has been reported."
            ),

            new ErrorMessage(
            408, "Password is not valid and sent by a bot, this incident has been reported."
            ),

            new ErrorMessage(
            409, "Email is not valid and sent by a bot, this incident has been reported."
            ),

            new ErrorMessage(
            410, "Username or email is already registered."
            ),

            new ErrorMessage(
            500, "The server had an error."
            ),
            new ErrorMessage(
            501, "There is no connection to the server."
            )
        };
    }

    static bool TryExtractHttpStatusCode(string responseString, out int statusCode)
    {
        if (responseString == "" || responseString == null)
        {
            statusCode = -1;
            return false;
        }
        
        // Use a regular expression to match the HTTP status code
        var match = Regex.Match(responseString, @"HTTP/\d\.\d (\d{3})");

        if (match.Success && match.Groups.Count == 2)
        {
            return int.TryParse(match.Groups[1].Value, out statusCode);
        }

        // Return false if the pattern does not match
        statusCode = 0;
        return false;
    }

    public void ShowError(int statusCode, UIControl.Type windowType)
    {
        foreach (var errorMessage in errorMessages)
        {
            if (errorMessage.code != statusCode)
                continue;
            switch (windowType)
            {
                case UIControl.Type.Registration:
                    registrationUI.ShowServerFeedbackMessage(errorMessage);
                    break;
                case UIControl.Type.Login:
                    loginUI.ShowServerFeedbackMessage(errorMessage.message);
                    break;
                default:
                    Debug.LogError(windowType);
                    break;
            }
            return;
        }
        Debug.LogError("No error message is made for status code " + statusCode);
    }
    public void ShowError(string error, UIControl.Type windowType)
    {
        int statusCode = -1;
        if (!TryExtractHttpStatusCode(error, out statusCode))
        {
            // this is the status code that shows when the server is down and there is no connection
            statusCode = 501;
        }
        ShowError(statusCode, windowType);
    }

    IEnumerator AuthenticateUser(string username, string password)
    {
        #region Wait if busy with another server connection process

        if (MongoDB.l.busy)
        {
            Debug.LogWarning("Waiting for another process to finish");
            yield return MongoDB.l.waitTime;
        }
        MongoDB.l.busy = true;

        #endregion

        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        UnityWebRequest www = UnityWebRequest.Post(MongoDB.apiUrlStart + "/authenticate", form);

        Debug.Log("Request started AuthenticateUser");

        // Send the request
        yield return www.SendWebRequest();

        Debug.Log("Request successful");

        // Check for errors
        if (www.result != UnityWebRequest.Result.Success)
        {
            ShowError(www.error, UIControl.Type.Login);
        }
        else
        {
            // Parse the JSON response to get the token
            string jsonResponse = www.downloadHandler.text;
            AuthResponse authResponse = JsonUtility.FromJson<AuthResponse>(jsonResponse);
            this.authResponse = authResponse;
            UIControl.l.loggedInAs.LoggedInAs(authResponse.username);

            ShowError((int)www.responseCode, UIControl.Type.Login);
            Debug.Log("User logged in successfully: " + www.downloadHandler.text);
        }

        MongoDB.l.busy = false;
        www.Dispose();
    }

    public void TryAuthenticateUser(string usernameOrEmail, string password)
    {
        StartCoroutine(AuthenticateUser(usernameOrEmail, password));
    }

    public void TryRegisterUser(string username, string password, string email)
    {
        StartCoroutine(RegisterUser(username, password, email));
    }

    IEnumerator RegisterUser(string username, string password, string email)
    {
        #region Wait if busy with another server connection process

        if (MongoDB.l.busy)
        {
            Debug.LogWarning("Waiting for another process to finish");
            yield return MongoDB.l.waitTime;
        }
        MongoDB.l.busy = true;

        #endregion

        // Construct the request payload
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);
        form.AddField("email", email);

        Debug.Log($"username={username}, password={password}, email={email}, url={MongoDB.apiUrlStart + "/auth/register"}");

        // Create the UnityWebRequest
        //UnityWebRequest www = UnityWebRequest.Post(MongoDB.apiUrlStart + "/auth/register", form);
        UnityWebRequest www = UnityWebRequest.Post(MongoDB.apiUrlStart + "/register", form);

        // Send the request
        yield return www.SendWebRequest();

        Debug.Log("Request successful");

        // Check for errors
        if (www.result != UnityWebRequest.Result.Success)
        {
            ShowError(www.error, UIControl.Type.Registration);
        }
        else
        {
            ShowError((int)www.responseCode, UIControl.Type.Registration);
            Debug.Log("User registered successfully: " + www.downloadHandler.text);
        }

        MongoDB.l.busy = false;
        www.Dispose();
    }

    #region Validity checking

    public static bool EmailIsValid(string email)
    {
        return Regex.IsMatch(email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z_]{2,}$");
    }

    public static bool ContainsOddCharacters(string input)
    {
        // Check that there are no characters outside the specified ranges
        if (Regex.IsMatch(input, @"[^A-Za-z\d!@#$%^&*()_+\-=]"))
        {
            Debug.Log("String must not contain characters outside the specified ranges: " + input);
            return true;
        }
        return false;
    }

    // sync with same variables in the server code: validation.js
    public static readonly int passwordMinLength = 8;
    public static readonly int passwordMaxLength = 32;
    public static readonly int usernameMinLength = 5;
    public static readonly int usernameMaxLength = 16;

    public void ShowFeedbackMessage(string msg, UIControl.Type windowType)
    {
        switch (windowType)
        {
            case UIControl.Type.Registration:
                registrationUI.ShowFeedbackMessage(msg);
                break;
            case UIControl.Type.Login:
                loginUI.ShowFeedbackMessage(msg);
                break;
            default: Debug.LogError("No case for :" + windowType + ", MSG: " + msg); break;
        }    
    }

    public bool PasswordIsValid(string input, UIControl.Type windowType)
    {
        input = input.Trim();

        if (input.Length > passwordMaxLength)
        {
            string errMsg = "Password cannot exceed " + passwordMaxLength + " characters.";
            ShowFeedbackMessage(errMsg, windowType);
            Debug.Log(errMsg);
            return false;
        }

        if (input.Length < passwordMinLength)
        {
            string errMsg = $"Password must be at least {passwordMinLength} characters long.";
            ShowFeedbackMessage(errMsg, windowType);
            Debug.Log(errMsg);
            return false;
        }
        
        // Check for at least one uppercase letter
        if (!Regex.IsMatch(input, @"[A-Z]"))
        {
            string errMsg = "Password must contain at least one uppercase letter.";
            ShowFeedbackMessage(errMsg, windowType);
            Debug.Log(errMsg);
            return false;
        }

        // Check for at least one lowercase letter
        if (!Regex.IsMatch(input, @"[a-z]"))
        {
            string errMsg = "Password must contain at least one lowercase letter.";
            ShowFeedbackMessage(errMsg, windowType);
            Debug.Log(errMsg);
            return false;
        }

        // Check for at least one digit (number)
        if (!Regex.IsMatch(input, @"\d"))
        {
            string errMsg = "Password must contain at least one digit (number).";
            ShowFeedbackMessage(errMsg, windowType);
            Debug.Log(errMsg);
            return false;
        }

        // Check that there are no characters outside the specified ranges
        if (Regex.IsMatch(input, @"[^A-Za-z\d!@#$%^&*()_+\-=]"))
        {
            string errMsg = "Password must not contain characters outside the specified ranges.";
            ShowFeedbackMessage(errMsg, windowType);
            Debug.Log(errMsg);
            return false;
        }

        // All checks passed, the string is valid
        return true;
    }

    public bool UsernameIsValid(string username, UIControl.Type windowType)
    {
        if (string.IsNullOrEmpty(username))
        {
            string errMsg = "Username should not be null or empty";
            ShowFeedbackMessage(errMsg, windowType);
            Debug.Log(errMsg);
            return false;
        }
        
        if (username.Length > usernameMaxLength)
        {
            string errMsg = "Username cannot exceed " + usernameMaxLength + " characters.";
            ShowFeedbackMessage(errMsg, windowType);
            Debug.Log(errMsg);
            return false;
        }
        if (username.Length < usernameMinLength)
        {
            string errMsg = $"Username should be at least {usernameMinLength} characters long";
            ShowFeedbackMessage(errMsg, windowType);
            Debug.Log(errMsg);
            return false;
        }

        if (!Regex.IsMatch(username, @"^[a-zA-Z0-9]+$"))
        {
            string errMsg = "Username should only contain A-Z, a-z, and 0-9";
            ShowFeedbackMessage(errMsg, windowType);
            Debug.Log(errMsg);
            return false;
        }

        // If all checks pass, the username is valid
        return true;
    }

    #endregion

    #region 

    public string GetFeedbackMessageText(FeedbackMessage message)
    {
        switch (message)
        {
            // 0
            case FeedbackMessage.None: return "";
            // 1 
            case FeedbackMessage.PasswordsDontMatch: return "Passwords entered do not match.";
            // 2 
            case FeedbackMessage.SuccessRegistering: return "Successfully registered.";
            // 3 
            case FeedbackMessage.EmailNotValid: return "Email entered is not valid.";
            // 4
            case FeedbackMessage.UsernameNotValid: return $"Username must be at least {UserAccount.usernameMinLength} characters, A-Z, a-z, and 0-9";
            // 5
            case FeedbackMessage.PasswordNotValid: return $"Password must be at least {UserAccount.passwordMinLength} characters, and include A-Z, a-z, and 0-9 and special characters.";
            // 6
            case FeedbackMessage.ContainsOddCharacters: return $"Characters must be within A-Z, a-z, 0-9, and !@#$%^&*()-=_+";
            // error
            default: Debug.LogError(message); return message.ToString();
        }
    }

    #endregion

    #region Datatypes

    public enum FeedbackMessage
    {
        // 0
        None,
        // 1
        PasswordsDontMatch,
        // 2
        SuccessRegistering,
        // 3 
        EmailNotValid,
        // 4
        UsernameNotValid,
        // 5
        PasswordNotValid,
        // 6
        ContainsOddCharacters
    }

    #endregion
}
