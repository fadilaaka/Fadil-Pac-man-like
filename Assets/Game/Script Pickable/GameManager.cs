using TMPro;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public TMP_Text scoreText;

    private int totalMoneyPoints;
    private int currentScore = 0;
    private List<MoneyPoint> moneyPoints;

    [Header("Sound Settings")]
    public AudioSource audioSource;
    public AudioClip collectSound;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        moneyPoints = FindObjectsByType<MoneyPoint>(FindObjectsSortMode.None).ToList();
        RemoveRandomMoneyPoints(28);
        totalMoneyPoints = moneyPoints.Count;
        UpdateScoreUI();
    }

    private void RemoveRandomMoneyPoints(int count)
    {
        if (moneyPoints.Count <= count) return;
        List<MoneyPoint> selectedToRemove = moneyPoints.OrderBy(x => Random.value).Take(count).ToList();
        foreach (MoneyPoint money in selectedToRemove)
        {
            moneyPoints.Remove(money);
            Destroy(money.gameObject);
        }
    }
    public void AddScore(int amount)
    {
        currentScore += amount;
        UpdateScoreUI();
        if (audioSource != null && collectSound != null)
        {
            audioSource.PlayOneShot(collectSound);
        }
        if (currentScore >= totalMoneyPoints)
        {
            SceneManager.LoadScene("GameWinScene");
        }
    }

    private void UpdateScoreUI()
    {
        scoreText.text = $"{currentScore} / {totalMoneyPoints}";
    }
}
