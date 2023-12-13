using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class Highscore
{
    public string name;
    public int highscore;
}


[System.Serializable]
public class HighscoreData
{
    public string _id; // Change the type to string
    public string username;
    public int highscore;
}

[System.Serializable]
public class HighscoresWrapper
{
    public List<HighscoreData> highscores;
}
public class MongoDB : MonoBehaviour
{
    public UserAccount account;

    public const string apiUrlStart = "http://localhost:3000";
    private const string showHighscoresApiUrl = "/highscores";
    private const string addHighscoreApiUrl = "/addhighscore";
    public bool busy;
    public readonly WaitForSeconds waitTime = new WaitForSeconds(0.5f);
    public static MongoDB l;
    [Header("For Testing")]
    public string addScoreTestName;
    public int addScoreTestScore;

    private void Awake()
    {
        if (l != null)
            Destroy(gameObject);
        l = this;
        DontDestroyOnLoad(gameObject);
    }

    public IEnumerator ShowHighscores()
    {
        if (busy)
        {
            Debug.LogWarning("Waiting for another process to finish");
            yield return waitTime;
        }
        busy = true;

        UnityWebRequest request = UnityWebRequest.Get(apiUrlStart + showHighscoresApiUrl);

        yield return request.SendWebRequest();

        // Wait until the request is done
        while (!request.isDone)
        {
            yield return null; // Wait for the next frame
        }

        if (request.result == UnityWebRequest.Result.Success)
        {
            //Debug.Log($"Raw JSON Response: {request.downloadHandler.text}");

            // Deserialize the received JSON into the wrapper class
            // {"highscores":[{"name":"name string","highscore":0}]} is this data's format
            HighscoresWrapper wrapper = JsonUtility.FromJson<HighscoresWrapper>(request.downloadHandler.text);

            if (wrapper.highscores != null)
            {
                UIControl.l.highscore.SetHighScores(wrapper);
            }
            else
            {
                Debug.Log("No highscores found.");
            }
        }
        else
        {
            Debug.LogError($"Error: {request.error}");
        }

        busy = false;

        request.Dispose();
    }

    public void BegingAddHighscore(string name, int score)
    {
        StartCoroutine(AddHighscore(name, score));
    }

    public IEnumerator AddHighscore(string name, int score)
    {
        if (busy)
        {
            Debug.LogWarning("Waiting for another process to finish");
            yield return waitTime;
        }
        busy = true;

        // Create a form to send highscore data
        WWWForm form = new WWWForm();
        form.AddField("highscore", score);

        // Set the authentication token in the request headers
        //if (!string.IsNullOrEmpty(UserAccount.l.authResponse.token))
        //{
            // Bearer indicates the authentication type
        //    form.headers["Authorization"] = "Bearer " + UserAccount.l.authResponse.token;
        //}

        UnityWebRequest request = UnityWebRequest.Post(apiUrlStart + addHighscoreApiUrl, form);
        request.SetRequestHeader("Authorization", UserAccount.l.authResponse.token);

        // Make a UnityWebRequest to add the highscore
        //UnityWebRequest request = UnityWebRequest.Post(apiUrlStart + addHighscoreApiUrl, form);


        // Send the request
        yield return request.SendWebRequest();

        // Wait until the request is done
        while (!request.isDone)
        {
            yield return null; // Wait for the next frame
        }
        
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Highscore submitted successfully");
        }
        else
        {
            if ((int)request.responseCode == 401 || (int)request.responseCode == 403)
            {
                UIControl.l.highscore.feedbackText.text = "Not logged in";
            }
            
            Debug.LogError($"Error: {request.error}");
        }
        busy = false;
        request.Dispose();


        StartCoroutine(ShowHighscores());
    }
}
