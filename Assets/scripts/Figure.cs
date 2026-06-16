using UnityEngine;

public class Figure : MonoBehaviour
{
    [Header("State")]
    public FigureState currentState;

    [Header("Type")]
    public FigureType figureType =
        FigureType.Big;

    private SpriteRenderer spriteRenderer;

    [Header("Collision")]
    [SerializeField]
    private float collisionCooldown = 0.1f;

    private float nextCollisionTime = 0f;

    private void Awake()
    {
        spriteRenderer =
            GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        UpdateVisual();
    }

    public void SetState(
        FigureState newState
    )
    {
        currentState = newState;
        UpdateVisual();
    }

    void UpdateVisual()
    {
        switch (currentState)
        {
            case FigureState.Black:
                spriteRenderer.color =
                    Color.black;
                break;

            case FigureState.Gray:
                spriteRenderer.color =
                    Color.gray;
                break;

            case FigureState.White:
                spriteRenderer.color =
                    Color.white;
                break;
        }
    }

    public void CycleState()
    {
        switch (currentState)
        {
            case FigureState.Black:
                SetState(
                    FigureState.Gray
                );
                break;

            case FigureState.Gray:
                SetState(
                    FigureState.White
                );
                break;

            case FigureState.White:
                SetState(
                    FigureState.Gray
                );
                break;
        }
    }

    private void OnCollisionEnter2D(
    Collision2D collision
)
    {
        Figure other =
            collision.gameObject
                .GetComponent<Figure>();

        if (other == null)
            return;

        if (
            figureType != FigureType.Small &&
            other.figureType != FigureType.Small
        )
        {
            return;
        }

        if (
            Time.time <
            nextCollisionTime
        )
        {
            return;
        }

        if (
            Time.time <
            other.nextCollisionTime
        )
        {
            return;
        }

        nextCollisionTime =
            Time.time +
            collisionCooldown;

        other.nextCollisionTime =
            Time.time +
            collisionCooldown;

        CycleState();
        other.CycleState();
    }
}