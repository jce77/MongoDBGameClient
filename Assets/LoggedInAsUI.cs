using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoggedInAsUI : MonoBehaviour
{
    public TextMeshProUGUI usernameText;
    public void LoggedInAs(string username)
    {
        usernameText.text = username;
    }
}
