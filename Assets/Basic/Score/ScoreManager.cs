using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _scoreText;
    private int _score;
    private int _maxScore;
    public void Start()
    {
        _score = 0;
        _maxScore = 0;
        UpdateUI();
    }
    public void UpdateUI()
    {
        _scoreText.text = "Point: " + _score + "/" + _maxScore;
    }
    public void SetMaxScore(int value)
    {
        _maxScore = value;
        UpdateUI();
    }
    public void AddScore(int value)
    {
        _score += value;
        UpdateUI();
    }
}