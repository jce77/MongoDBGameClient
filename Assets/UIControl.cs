using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIControl : MonoBehaviour
{
    public static UIControl l;
    private void Awake()
    {
        if (l != null)
        {
            Destroy(gameObject);
            return;
        }
        l = this;
    }

    public enum Type { Registration, Login, Highscore}
    public CanvasGroup registrationUIAlpha;
    public CanvasGroup loginUIAlpha;
    public CanvasGroup highscoreUIAlpha;
    public CanvasGroup loggedInAsUIAlpha;

    private static readonly float inactiveUIAlpha = 0.7f;
    public RegistrationUI registration;
    public HighscoreUI highscore;
    public LoginUI login;
    public LoggedInAsUI loggedInAs;
    public Score[] scores;

    private void Start()
    {
        registration.Init();
        login.Init();
        foreach(var score in scores)
            score.Init();
        ClickedTestRegistrationUI();
    }

    public void ClickedChangeTestMenuButton(int type)
    {
        SetUIOn((Type)type);
    }

    public void ClickedTestRegistrationUI()
    {
        SetUIOn(Type.Registration);
    }

    public void ClickedTestLoginUI()
    {
        SetUIOn(Type.Login);
    }



    public void SetUIOn(Type type)
    {
        registration.enabled = false;
        registrationUIAlpha.alpha = inactiveUIAlpha;
        registrationUIAlpha.blocksRaycasts = false;

        login.enabled = false;
        loginUIAlpha.alpha = inactiveUIAlpha;
        loginUIAlpha.blocksRaycasts = false;

        highscore.enabled = false;
        highscoreUIAlpha.alpha = inactiveUIAlpha;
        highscoreUIAlpha.blocksRaycasts = false;

        /// removed for now, that panel should always be on
        //loggedInAs.enabled = false;
        //loggedInAsUIAlpha.alpha = inactiveUIAlpha;
        //loggedInAsUIAlpha.blocksRaycasts = false;

        switch (type)
        {
            case Type.Registration:
                registrationUIAlpha.alpha = 1;
                registrationUIAlpha.blocksRaycasts = true;
                registration.enabled = true;
                break;
            case Type.Login:
                loginUIAlpha.alpha = 1;
                loginUIAlpha.blocksRaycasts = true;
                login.enabled = true;
                break;
            case Type.Highscore:
                highscoreUIAlpha.alpha = 1;
                highscoreUIAlpha.blocksRaycasts = true;
                highscore.enabled = true;
                break; 
            //case Type.LoggedInAs:
            //    loggedInAsUIAlpha.alpha = 1;
            //    loggedInAsUIAlpha.blocksRaycasts = true;
            //    loggedInAs.enabled = true;
            //    break;
        }
    }

    public static void SwitchToNextInput(TMP_InputField[] inputFields)
    {
        if (inputFields.Length == 0)
            return;

        for (int i = 0; i < inputFields.Length; i++)
        {
            if (inputFields[i].isFocused)
            {
                int nextIndex = (i + 1) % inputFields.Length;
                inputFields[nextIndex].Select();
                return;
            }
        }

        // If no input field is focused, select the first one
        inputFields[0].Select();
    }
}
