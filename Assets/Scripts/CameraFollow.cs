using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private PlayerInput playerInput;

    [Header("Aim Offset")]
    [SerializeField] private float aimOffsetRatio = 0.3f;
    [SerializeField] private float maxAimOffset = 3f;

    [Header("Camera Bounds")]
    [SerializeField] private float minX;
    [SerializeField] private float maxX;
    [SerializeField] private float minZ;
    [SerializeField] private float maxZ;


    private Vector3 offsetPosition;


    // private float startTimer;
    // private bool isStart;
    // private bool isDie;

    private void Awake()
    {
        offsetPosition =
            transform.position - player.position;
    }


    private void LateUpdate()
    {
        Vector3 aimOffset =
            Vector3.ClampMagnitude(
                playerInput.AimVector * aimOffsetRatio,
                maxAimOffset);

        Vector3 cameraPosition =
            player.position +
            offsetPosition +
            aimOffset;

        cameraPosition.x =
            Mathf.Clamp(cameraPosition.x, minX, maxX);

        cameraPosition.z =
            Mathf.Clamp(cameraPosition.z, minZ, maxZ);

        transform.position =
            cameraPosition;
    }

    // private void StartOffset()
    // {
    //     offsetPosition.y = Mathf.MoveTowards(offsetPosition.y, 12f, 3f * Time.deltaTime);
    // }

    // private void DieOffset()
    // {
    //     offsetPosition.y = Mathf.MoveTowards(offsetPosition.y, 20f, 3f * Time.deltaTime);
    // }
}
