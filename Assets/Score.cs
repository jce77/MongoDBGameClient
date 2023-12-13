using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI scoreText;

    public void Init()
    {
        ResetThis();
    }

    public void ResetThis()
    {
        nameText.text = "";
        scoreText.text = "";
    }
}
