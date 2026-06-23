using UnityEngine;

public class Figure : MonoBehaviour
{
    [Header("State")]
    public FigureState currentState;

    [Header("Type")]
    public FigureType figureType =
        FigureType.Big;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    [Header("Collision")]
    [SerializeField]
    private float collisionCooldown = 0.15f;

    private float nextCollisionTime = 0f;

    [Header("Bounce")]
    [SerializeField]
    private float bounceVariation = 0.35f;

    [SerializeField]
    private float minAxisSpeed = 0.2f;

    [Header("Speed Limits")]
    [SerializeField]
    private float minSpeed = 3f;

    [SerializeField]
    private float maxSpeed = 5f;

    private void Awake()
    {
        spriteRenderer =
            GetComponent<SpriteRenderer>();

        rb =
            GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        UpdateVisual();
    }

    private void FixedUpdate()
    {
        if (
            figureType ==
            FigureType.Small
        )
        {
            FixStuckMovement();
            ClampSpeed();
        }
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
        //------------------------------------------------
        // CHOQUE CONTRA PAREDES
        //------------------------------------------------
        if (
            collision.gameObject.CompareTag("Wall")
        )
        {
            if (
                figureType !=
                FigureType.Small
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
            nextCollisionTime =
                Time.time +
                collisionCooldown;

            CycleState();

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayBounce();
            }

            AddBounceVariation();

            return;
        }

        //------------------------------------------------
        // CHOQUE CONTRA OTRA FIGURA
        //------------------------------------------------
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

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBounce();
        }
    }

    void AddBounceVariation()
    {
        if (rb == null)
            return;

        Vector2 velocity =
            rb.linearVelocity;

        Vector2 randomOffset =
            new Vector2(
                Random.Range(
                    -bounceVariation,
                    bounceVariation
                ),
                Random.Range(
                    -bounceVariation,
                    bounceVariation
                )
            );

        velocity += randomOffset;

        velocity =
            velocity.normalized *
            velocity.magnitude;

        rb.linearVelocity =
            velocity;

        ClampSpeed();
    }

    void FixStuckMovement()
    {
        if (rb == null)
            return;

        if (
            rb.linearVelocity.magnitude
            < 0.1f
        )
        {
            return;
        }

        Vector2 direction =
            rb.linearVelocity.normalized;

        if (
            Mathf.Abs(direction.x)
            < minAxisSpeed
        )
        {
            float dir =
                Random.value > 0.5f
                ? 1f
                : -1f;

            rb.linearVelocity =
                new Vector2(
                    dir *
                    rb.linearVelocity.magnitude,
                    rb.linearVelocity.y
                );
        }

        if (
            Mathf.Abs(direction.y)
            < minAxisSpeed
        )
        {
            float dir =
                Random.value > 0.5f
                ? 1f
                : -1f;

            rb.linearVelocity =
                new Vector2(
                    rb.linearVelocity.x,
                    dir *
                    rb.linearVelocity.magnitude
                );
        }
    }

    void ClampSpeed()
    {
        if (rb == null)
            return;

        float speed =
            rb.linearVelocity.magnitude;

        if (speed < minSpeed)
        {
            rb.linearVelocity =
                rb.linearVelocity.normalized *
                minSpeed;
        }

        if (speed > maxSpeed)
        {
            rb.linearVelocity =
                rb.linearVelocity.normalized *
                maxSpeed;
        }
    }
}