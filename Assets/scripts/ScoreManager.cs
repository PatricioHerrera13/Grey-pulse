using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public int score;

    public TMP_Text scoreText;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateUI();
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateUI();
    }

    void UpdateUI()
    {
        scoreText.text =
            "SCORE: " + score;
    }

    public void ResetScore()
    {
        score = 0;
        UpdateUI();
    }
  }
