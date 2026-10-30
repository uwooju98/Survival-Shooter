using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static readonly int HashMove = Animator.StringToHash("Move");

    public float moveSpeed = 10f;

    public Gun gun;


    private Animator playerAnimator;
    private PlayerInput playerInput;
    private Rigidbody playerRigidbody;


    public Vector3 moveDirection { get; private set; }
    public Vector3 lookDirection { get; private set; }
    public float moveAmount { get; private set; }


    private void Awake()
    {
        playerAnimator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
        playerRigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        moveDirection =
            playerInput.vertical * Vector3.forward + playerInput.horizontal * Vector3.right;

        lookDirection = playerInput.aimVector;

        moveAmount =
            Mathf.MoveTowards(moveAmount, Mathf.Clamp01(moveDirection.magnitude), 5f * Time.deltaTime);

        playerAnimator.SetFloat(HashMove, moveAmount);

        if (playerInput.Fire)
        {
            gun.Fire();
        }
        // else if (playerInput.Reload && gun.Reload())
        // {
        //     playerAnimator.SetTrigger(IdReload);
        // }
    }

    private void FixedUpdate()
    {
        Vector3 moveDelta = moveDirection.normalized * moveSpeed * Time.fixedDeltaTime;

        if (lookDirection != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(lookDirection);

            playerRigidbody.MoveRotation(lookRotation);
        }
        playerRigidbody.MovePosition(playerRigidbody.position + moveDelta);
    }

}
