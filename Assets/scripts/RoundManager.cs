using TMPro;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance;

    public TMP_Text roundText;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        roundText.text =
            "ROUND: " +
            GameManager.Instance.currentRound;
    }
}