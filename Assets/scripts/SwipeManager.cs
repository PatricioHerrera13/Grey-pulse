using UnityEngine;

public class SwipeManager : MonoBehaviour
{
    private bool firstCutMade = false;

    void Update()
    {
        // Soporta tanto Mouse (Editor) como Touch (Unity Remote y builds)
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            Vector2 inputPosition;

            if (Input.touchCount > 0)
                inputPosition = Input.GetTouch(0).position;   // Celular
            else
                inputPosition = Input.mousePosition;          // Editor / Mouse

            Vector2 worldPosition = Camera.main.ScreenToWorldPoint(inputPosition);

            RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);

            if (hit.collider != null)
            {
                Figure figure = hit.collider.GetComponent<Figure>();
                if (figure != null)
                {
                    TryCutFigure(figure);
                }
            }
        }
    }

    // ====================== LÓGICA ORIGINAL (sin cambios) ======================
    void TryCutFigure(Figure figure)
    {
        if (figure.currentState != FigureState.White)
            return;

        if (!firstCutMade)
        {
            firstCutMade = true;
            TimerManager.Instance.StartTimer();
        }

        GameManager gm =
            GameManager.Instance;

        if (figure.figureType ==
            FigureType.Big)
        {
            gm.SpawnSmallFigures(
                figure.transform.position
            );
        }

        gm.figures.Remove(figure);

        int points =
            (figure.figureType ==
            FigureType.Big)
            ? 10
            : 5;

        ScoreManager.Instance
            .AddScore(points);

        if (figure.figureType ==
            FigureType.Big)
        {
            AudioManager.Instance
                .PlayCutBig();
        }
        else
        {
            AudioManager.Instance
                .PlayCutSmall();
        }

        Destroy(
            figure.gameObject
        );

        if (figure.figureType ==
            FigureType.Big)
        {
            AdvanceChain();
        }

        CheckRoundFinished();
    }

    void AdvanceChain()
    {
        GameManager gm = GameManager.Instance;

        if (gm.currentGrayFigure == null) return;

        gm.currentWhiteFigure = gm.currentGrayFigure;
        gm.currentWhiteFigure.SetState(FigureState.White);

        Figure newGray = gm.GetRandomFigure(gm.currentWhiteFigure);

        if (newGray != null)
        {
            gm.currentGrayFigure = newGray;
            gm.currentGrayFigure.SetState(FigureState.Gray);
        }
        else
        {
            gm.currentGrayFigure = null;
        }
    }

    void CheckRoundFinished()
    {
        if (GameManager.Instance.figures.Count == 0)
        {
            firstCutMade = false;
            TimerManager.Instance.StopTimer();
            TimerManager.Instance.ResetTimer();
            GameManager.Instance.NextRound();
        }
    }

    public void ResetRun()
    {
        firstCutMade = false;
    }
}