using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextFinalScore : MonoBehaviour
{
    //writes final score on UI where needed

    public TMP_Text finalScore;
    void Start()
    {
        finalScore = GetComponent<TMP_Text>();
    }

    void Update()
    {
        finalScore.text = "FINAL SCORE: " + PlayerPrefs.GetInt("overallScore", 0);
    }
}
