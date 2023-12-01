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
    public string name;
    public int highscore;
}

[System.Serializable]
public class HighscoresWrapper
{
    public List<HighscoreData> highscores;
}
public class MongoDB : MonoBehaviour
{
    private const string apiUrlStart = "http://localhost:3000/";
    private const string showHighscoresApiUrl = "highscores";
    private const string addHighscoreApiUrl = "addhighscore";
    public bool busy;
    private WaitForSeconds waitTime = new WaitForSeconds(0.5f);
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
                foreach (HighscoreData highscore in wrapper.highscores)
                {
                    // Printing name and score
                    Debug.Log($"Name: {highscore.name}, Score: {highscore.highscore}");
                }
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

    public IEnumerator AddHighscore(string name, int highscore)
    {
        if (busy)
        {
            Debug.LogWarning("Waiting for another process to finish");
            yield return waitTime;
        }
        busy = true;

        
        string jsonPayload = $"{{\"name\": \"{name}\", \"highscore\": {highscore}}}";

        // Create a POST request
        UnityWebRequest request = new UnityWebRequest(apiUrlStart + addHighscoreApiUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");


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
            Debug.LogError($"Error: {request.error}");
        }
        busy = false;
        request.Dispose();

        
    }
}
