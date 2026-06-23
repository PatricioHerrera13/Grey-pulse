using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("References")]
    public GameObject figurePrefab;

    [Header("Round")]
    public int currentRound = 1;

    [Header("Grid")]
    public int columns = 4;
    public int rows = 4;
    public float cellSize = 1.6f;
    
    [Header("Small Figures")]
    public GameObject smallFigurePrefab;
    public int smallFiguresPerBig = 2;
    public float smallFigureForce = 5f;
   
    public List<Figure> figures = new List<Figure>();

    private List<Vector2> availablePositions = new List<Vector2>();

    [HideInInspector]
    public Figure currentWhiteFigure;

    [HideInInspector]
    public Figure currentGrayFigure;

    // ==================== TUTORIAL MEJORADO ====================
    [Header("UI Flow - Tutorial")]
    public GameObject tutorialPanel;
    public TMP_Text countdownText;
    
    [Header("Tutorial Images")]
    public GameObject[] tutorialImages;   // Arrastra aquí las 5 imágenes

    [HideInInspector]
    public bool gameStarted = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // No inicia automáticamente
    }

    private void OnEnable()
    {
        StartCoroutine(StartTutorialAfterLoad());
    }

    private IEnumerator StartTutorialAfterLoad()
    {
        yield return new WaitForSeconds(0.1f);
        
        if (!gameStarted)
        {
            StartGameWithTutorial();
        }
    }

    public void StartGameWithTutorial()
    {
        gameStarted = false;
        if (tutorialPanel != null) tutorialPanel.SetActive(true);
        StartCoroutine(TutorialCoroutine());
    }

    // ==================== TUTORIAL CON 5 IMÁGENES (2 SEGUNDOS POR PASO) ====================
    private IEnumerator TutorialCoroutine()
    {
        if (countdownText == null || tutorialImages == null) yield break;

        countdownText.gameObject.SetActive(true);

        for (int i = 5; i >= 1; i--)
        {
            // Activar solo la imagen correspondiente
            ShowTutorialImage(i);
            
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(2f);     // ← Cambiado a 2 segundos
        }

        // ¡YA!
        countdownText.text = "¡YA!";
        ShowTutorialImage(0); // Oculta todas las imágenes
        yield return new WaitForSeconds(1f);         // Tiempo del "¡YA!"

        // Finalizar tutorial
        countdownText.gameObject.SetActive(false);
        if (tutorialPanel != null) 
            tutorialPanel.SetActive(false);
        
        gameStarted = true;

        // Iniciar el juego
        SpawnRound();

        if (RoundManager.Instance != null)
            RoundManager.Instance.UpdateUI();
    }

    // Muestra la imagen correspondiente (1 a 5). Si paso 0 → oculta todas
    private void ShowTutorialImage(int number)
    {
        // Desactivar todas primero
        foreach (GameObject img in tutorialImages)
        {
            if (img != null) img.SetActive(false);
        }

        // Activar la correcta
        if (number >= 1 && number <= tutorialImages.Length)
        {
            if (tutorialImages[number - 1] != null)
                tutorialImages[number - 1].SetActive(true);
        }
    }

    // ==================== CÓDIGO ORIGINAL (sin cambios) ====================
    public void SpawnRound()
    {
        ArenaManager.Instance.SpawnRandomArena();
        ClearRound();
        GenerateGrid();

        int amount = currentRound + 5;
        amount = Mathf.Min(amount, availablePositions.Count);

        for (int i = 0; i < amount; i++)
        {
            int randomIndex = Random.Range(0, availablePositions.Count);
            Vector2 pos = availablePositions[randomIndex];
            availablePositions.RemoveAt(randomIndex);

            GameObject obj = Instantiate(figurePrefab, pos, Quaternion.identity);
            Figure figure = obj.GetComponent<Figure>();

            figure.SetState(FigureState.Black);
            figures.Add(figure);
        }

        ChooseInitialFigures();

        Debug.Log("Nueva ronda: " + currentRound);
    }

    void GenerateGrid()
    {
        availablePositions.Clear();

        float startX = -(columns - 1) * cellSize / 2f;
        float startY = -(rows - 1) * cellSize / 2f;

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                Vector2 pos = new Vector2(startX + x * cellSize, startY + y * cellSize);
                availablePositions.Add(pos);
            }
        }
    }

    void ChooseInitialFigures()
    {
        if (figures.Count < 2) return;

        int whiteIndex = Random.Range(0, figures.Count);
        currentWhiteFigure = figures[whiteIndex];
        currentWhiteFigure.SetState(FigureState.White);

        Figure nextFigure = GetRandomFigure(currentWhiteFigure);

        if (nextFigure != null)
        {
            currentGrayFigure = nextFigure;
            currentGrayFigure.SetState(FigureState.Gray);
        }
    }

    public Figure GetRandomFigure(Figure excluded)
    {
        List<Figure> candidates = new List<Figure>();
        foreach (Figure f in figures)
        {
            if (f != null && f != excluded)
                candidates.Add(f);
        }

        if (candidates.Count == 0) return null;

        int randomIndex = Random.Range(0, candidates.Count);
        return candidates[randomIndex];
    }

    public void SpawnSmallFigures(Vector2 position)
    {
        for (int i = 0; i < smallFiguresPerBig; i++)
        {
            GameObject obj = Instantiate(smallFigurePrefab, position, Quaternion.identity);
            Figure smallFigure = obj.GetComponent<Figure>();
            figures.Add(smallFigure);

            Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
            Vector2 direction = Random.insideUnitCircle.normalized;
            rb.AddForce(direction * smallFigureForce, ForceMode2D.Impulse);
        }
    }

    void ClearRound()
    {
        foreach (Figure f in figures)
        {
            if (f != null)
                Destroy(f.gameObject);
        }

        figures.Clear();
        currentWhiteFigure = null;
        currentGrayFigure = null;
    }

    public void RestartRun()
    {
        currentRound = 1;

        if (RoundManager.Instance != null)
            RoundManager.Instance.UpdateUI();

        TimerManager.Instance.StopTimer();
        TimerManager.Instance.ResetTimer();

        SwipeManager swipe = FindFirstObjectByType<SwipeManager>();
        if (swipe != null)
            swipe.ResetRun();

        ScoreManager.Instance.ResetScore();

        SpawnRound();
    }

    public void NextRound()
    {
        currentRound++;

        if (RoundManager.Instance != null)
            RoundManager.Instance.UpdateUI();

        SpawnRound();
    }
}