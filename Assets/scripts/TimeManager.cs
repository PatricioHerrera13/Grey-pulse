using UnityEngine;
using UnityEngine.UI;

public class TimerManager : MonoBehaviour
{
    public static TimerManager Instance;

    [Header("Timer")]
    public float maxTime = 10f;

    private float currentTime;

    private bool timerRunning = false;

    [Header("UI")]
    public Image timeBar;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ResetTimer();
    }

    private void Update()
    {
        if (!timerRunning)
            return;

        currentTime -= Time.deltaTime;

        UpdateBar();

        if (currentTime <= 0f)
        {
            TimeUp();
        }
    }

    public void StartTimer()
    {
        timerRunning = true;
    }

    public void StopTimer()
    {
        timerRunning = false;
    }

    public void ResetTimer()
    {
        currentTime = maxTime;
        UpdateBar();
    }

    void UpdateBar()
    {
        if (timeBar == null)
            return;

        timeBar.fillAmount =
            currentTime / maxTime;
    }

    void TimeUp()
    {
        timerRunning = false;

        Debug.Log("Tiempo agotado");

        GameManager.Instance.RestartRun();
    }
}