using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static readonly int HashMove = Animator.StringToHash("Move");
    public static readonly int HashDash = Animator.StringToHash("Dash");

    [Header("Components")]
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Transform dashDirectionPivot;
    [SerializeField] private GameObject playerLights;
    [SerializeField] private Gun gun;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 10f;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 30f;
    [SerializeField] private float dashDuration = 0.225f;

    [Header("Collision")]
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float skinWidth = 0.03f;


    private PlayerInput playerInput;
    private Rigidbody playerRigidbody;
    private CapsuleCollider playerCollider;

    public Vector3 moveDirection { get; private set; }
    public Vector3 lookDirection { get; private set; }
    public Vector3 dashDirection { get; private set; }

    public bool isDashing { get; private set; }

    private float moveAmount;
    private float dashTimer;


    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerRigidbody = GetComponent<Rigidbody>();
        playerCollider = GetComponent<CapsuleCollider>();

        playerLights.SetActive(true);
    }
    private void Update()
    {
        UpdateMoveDirection();
        UpdateAnimation();
        UpdateLookDirection();

        HandleFire();
        HandleDashInput();
    }

    private void FixedUpdate()
    {
        RotatePlayer();

        if (isDashing)
        {
            MoveDash();
        }
        else
        {
            MovePlayer();
        }
    }

    private void UpdateMoveDirection()
    {
        moveDirection =
            playerInput.Vertical * Vector3.forward +
            playerInput.Horizontal * Vector3.right;
    }

    private void UpdateLookDirection()
    {
        if (!isDashing)
        {
            lookDirection = playerInput.AimVector;
        }
    }

    private void HandleFire()
    {
        if (playerInput.Fire && !isDashing)
        {
            gun.Fire();
        }
    }

    private void HandleDashInput()
    {
        if (!playerInput.Dash ||
            moveDirection == Vector3.zero ||
            isDashing)
        {
            return;
        }

        StartDash();
    }

    private void UpdateAnimation()
    {
        moveAmount =
            Mathf.MoveTowards(
                moveAmount,
                Mathf.Clamp01(moveDirection.magnitude),
                5f * Time.deltaTime);

        playerAnimator.SetFloat(HashMove, moveAmount);
    }

    private void RotatePlayer()
    {
        if (lookDirection == Vector3.zero || isDashing)
            return;

        Quaternion lookRotation =
            Quaternion.LookRotation(lookDirection);

        playerRigidbody.MoveRotation(lookRotation);
    }

    private void MovePlayer()
    {
        Vector3 moveDelta =
            moveDirection.normalized *
            moveSpeed *
            Time.fixedDeltaTime;

        Vector3 safeMove =
            CalculateSafeMove(moveDelta);

        playerRigidbody.MovePosition(
            playerRigidbody.position + safeMove);
    }

    private void StartDash()
    {
        dashDirection = moveDirection.normalized;

        dashDirectionPivot.rotation =
            Quaternion.LookRotation(dashDirection);

        dashTimer = dashDuration;
        isDashing = true;

        playerLights.SetActive(false);
        playerAnimator.SetTrigger(HashDash);
    }

    private void MoveDash()
    {
        Vector3 dashDelta =
            dashDirection *
            dashSpeed *
            Time.fixedDeltaTime;

        Vector3 safeDash =
            CalculateSafeMove(dashDelta);

        playerRigidbody.MovePosition(
            playerRigidbody.position + safeDash);

        dashTimer -= Time.fixedDeltaTime;

        if (dashTimer <= 0f)
        {
            EndDash();
        }
    }

    private void EndDash()
    {
        isDashing = false;

        dashDirectionPivot.localRotation =
            Quaternion.identity;

        playerLights.SetActive(true);
    }

    private void GetCapsulePoints(
    Vector3 position,
    out Vector3 point1,
    out Vector3 point2,
    out float radius)
    {
        Vector3 scale = transform.lossyScale;

        radius =
            playerCollider.radius *
            Mathf.Max(
                Mathf.Abs(scale.x),
                Mathf.Abs(scale.z));

        float height =
            playerCollider.height *
            Mathf.Abs(scale.y);

        height =
            Mathf.Max(height, radius * 2f);

        Vector3 center =
            position +
            transform.rotation *
            Vector3.Scale(
                playerCollider.center,
                scale);

        float halfHeight =
            height * 0.5f - radius;

        point1 =
            center + Vector3.up * halfHeight;

        point2 =
            center - Vector3.up * halfHeight;
    }

    private Vector3 CalculateSafeMove(Vector3 movement)
    {
        Vector3 position =
            playerRigidbody.position;

        Vector3 remainingMove =
            movement;

        Vector3 resultMove =
            Vector3.zero;

        for (int i = 0; i < 2; i++)
        {
            if (remainingMove.sqrMagnitude <= 0.000001f)
                break;

            GetCapsulePoints(
                position,
                out Vector3 point1,
                out Vector3 point2,
                out float radius);

            Vector3 direction =
                remainingMove.normalized;

            float distance =
                remainingMove.magnitude;

            if (Physics.CapsuleCast(
                point1,
                point2,
                radius,
                direction,
                out RaycastHit hit,
                distance + skinWidth,
                wallLayer,
                QueryTriggerInteraction.Ignore))
            {
                float moveDistance =
                    Mathf.Clamp(
                        hit.distance - skinWidth,
                        0f,
                        distance);

                Vector3 moveToWall =
                    direction * moveDistance;

                position += moveToWall;
                resultMove += moveToWall;

                Vector3 leftover =
                    remainingMove - moveToWall;

                remainingMove =
                    Vector3.ProjectOnPlane(
                        leftover,
                        hit.normal);
            }
            else
            {
                resultMove += remainingMove;
                break;
            }
        }

        return resultMove;
    }
}