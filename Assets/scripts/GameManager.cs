using System.Collections.Generic;
using UnityEngine;

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
   
    public List<Figure> figures =
        new List<Figure>();

    private List<Vector2> availablePositions =
        new List<Vector2>();

    [HideInInspector]
    public Figure currentWhiteFigure;

    [HideInInspector]
    public Figure currentGrayFigure;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SpawnRound();

        if (RoundManager.Instance != null)
        {
            RoundManager.Instance.UpdateUI();
        }
    }
    public void SpawnRound()
    {
        ClearRound();
        GenerateGrid();

        int amount = currentRound + 5;

        amount = Mathf.Min(
            amount,
            availablePositions.Count
        );

        for (int i = 0; i < amount; i++)
        {
            int randomIndex =
                Random.Range(
                    0,
                    availablePositions.Count
                );

            Vector2 pos =
                availablePositions[randomIndex];

            availablePositions.RemoveAt(
                randomIndex
            );

            GameObject obj =
                Instantiate(
                    figurePrefab,
                    pos,
                    Quaternion.identity
                );

            Figure figure =
                obj.GetComponent<Figure>();

            figure.SetState(
                FigureState.Black
            );

            figures.Add(figure);
        }

        ChooseInitialFigures();

        Debug.Log(
            "Nueva ronda: " +
            currentRound
        );
    }

    void GenerateGrid()
    {
        availablePositions.Clear();

        float startX =
            -(columns - 1) *
            cellSize / 2f;

        float startY =
            -(rows - 1) *
            cellSize / 2f;

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                Vector2 pos =
                    new Vector2(
                        startX + x * cellSize,
                        startY + y * cellSize
                    );

                availablePositions.Add(pos);
            }
        }
    }

    void ChooseInitialFigures()
    {
        if (figures.Count < 2)
            return;

        int whiteIndex =
            Random.Range(
                0,
                figures.Count
            );

        currentWhiteFigure =
            figures[whiteIndex];

        currentWhiteFigure
            .SetState(
                FigureState.White
            );

        Figure nextFigure =
            GetRandomFigure(
                currentWhiteFigure
            );

        if (nextFigure != null)
        {
            currentGrayFigure =
                nextFigure;

            currentGrayFigure
                .SetState(
                    FigureState.Gray
                );
        }
    }

    public Figure GetRandomFigure(
        Figure excluded
    )
    {
        List<Figure> candidates =
            new List<Figure>();

        foreach (Figure f in figures)
        {
            if (
                f != null &&
                f != excluded
            )
            {
                candidates.Add(f);
            }
        }

        if (candidates.Count == 0)
            return null;

        int randomIndex =
            Random.Range(
                0,
                candidates.Count
            );

        return candidates[randomIndex];
    }

    public void SpawnSmallFigures(
    Vector2 position
)
    {
        for (
            int i = 0;
            i < smallFiguresPerBig;
            i++
        )
        {
            GameObject obj =
                Instantiate(
                    smallFigurePrefab,
                    position,
                    Quaternion.identity
                );

            Figure smallFigure =
                obj.GetComponent<Figure>();

            figures.Add(smallFigure);

            Rigidbody2D rb =
                obj.GetComponent<Rigidbody2D>();

            Vector2 direction =
                Random.insideUnitCircle.normalized;

            rb.AddForce(
                direction *
                smallFigureForce,
                ForceMode2D.Impulse
            );
        }
    }

    void ClearRound()
    {
        foreach (Figure f in figures)
        {
            if (f != null)
            {
                Destroy(
                    f.gameObject
                );
            }
        }

        figures.Clear();

        currentWhiteFigure = null;
        currentGrayFigure = null;
    }
    public void RestartRun()
    {
        currentRound = 1;

        if (RoundManager.Instance != null)
        {
            RoundManager.Instance.UpdateUI();
        }

        TimerManager.Instance.StopTimer();
        TimerManager.Instance.ResetTimer();

        SwipeManager swipe =
            FindFirstObjectByType<SwipeManager>();

        if (swipe != null)
        {
            swipe.ResetRun();
        }

        ScoreManager.Instance.ResetScore();

        SpawnRound();
    }
    public void NextRound()
    {
        currentRound++;

        if (RoundManager.Instance != null)
        {
            RoundManager.Instance.UpdateUI();
        }

        SpawnRound();
    }
}