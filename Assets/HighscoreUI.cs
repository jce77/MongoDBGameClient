using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using JetBrains.Annotations;

public class HighscoreUI : MonoBehaviour
{
    public Score[] scores;

    public TMP_InputField scoreInputField;
    public TextMeshProUGUI feedbackText;

    private void Start()
    {
        feedbackText.text = "";
        StartCoroutine(MongoDB.l.ShowHighscores());
    }

    public void ClickedAddScore()
    {
        if (int.TryParse(scoreInputField.text, out int score))
        {
            MongoDB.l.BegingAddHighscore(UserAccount.l.authResponse.username, score);
        }
    }

    public void SetHighScores(HighscoresWrapper wrapper)
    {
        List<HighscoreData> reordered = new List<HighscoreData>();
        int count = wrapper.highscores.Count;
        for (int i = 0; i < count; i++)
        {
            int bestValue = int.MaxValue;
            int bestIndex = 0;
            for (int j = 0; j < wrapper.highscores.Count; j++)
            {
                if (wrapper.highscores[j].highscore < bestValue)
                {
                    bestValue = wrapper.highscores[j].highscore;
                    bestIndex = j;
                }
            }
            reordered.Add(wrapper.highscores[bestIndex]);
            wrapper.highscores.RemoveAt(bestIndex);
        }

        // for now it only shows the 10 best scores
        for (int i = 0; i < Mathf.Clamp(reordered.Count, 0, 10); i++)
        {
            scores[i].scoreText.text = reordered[i].highscore.ToString();
            scores[i].nameText.text = reordered[i].username;
        }
        for (int i = reordered.Count; i < 10; i++)
        {
            scores[i].scoreText.text = "";
            scores[i].nameText.text = "";
        }
    }
}
