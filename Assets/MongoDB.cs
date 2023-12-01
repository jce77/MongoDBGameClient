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
public class MongoIDB : MonoBehaviour
{
    
    private const string apiUrl = "http://localhost:3000/highscores";

    void Start()
    {
        StartCoroutine(ShowHighscores());
    }


    IEnumerator ShowHighscores()
    {
        UnityWebRequest request = UnityWebRequest.Get(apiUrl);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log($"Raw JSON Response: {request.downloadHandler.text}");

            // Deserialize the received JSON into the wrapper class
            HighscoresWrapper wrapper = JsonUtility.FromJson<HighscoresWrapper>(request.downloadHandler.text);

            // Check if there are highscores
            if (wrapper.highscores != null)
            {
                // Access highscores
                foreach (HighscoreData highscore in wrapper.highscores)
                {
                    // You can access _id as a string, as it's a nested object in your JSON
                    Debug.Log($"ID: {highscore._id}, Name: {highscore.name}, Score: {highscore.highscore}");
                }

                Debug.Log("Hit here");
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
    }
}
