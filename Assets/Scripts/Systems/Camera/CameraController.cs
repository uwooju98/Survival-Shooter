
using UnityEngine;
using System.Collections;

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

    [Header("Camera Intro")]
    [SerializeField] private float startOffsetY = 20f;
    [SerializeField] private float gameOffsetY = 15.5f;
    [SerializeField] private float cameraMoveDuration = 2f;


    private Vector3 offsetPosition;


    private void Awake()
    {
        offsetPosition = transform.position - player.position;

        offsetPosition.y = startOffsetY;
    }

    private void LateUpdate()
    {
        Vector3 aimOffset = Vector3.ClampMagnitude(playerInput.AimVector * aimOffsetRatio, maxAimOffset);

        Vector3 cameraPosition = player.position + offsetPosition + aimOffset;

        cameraPosition.x = Mathf.Clamp(cameraPosition.x, minX, maxX);
        cameraPosition.z = Mathf.Clamp(cameraPosition.z, minZ, maxZ);

        transform.position = cameraPosition;
    }


    public IEnumerator PlayStartCamera()
    {
        yield return MoveCameraHeight(gameOffsetY);
    }

    public IEnumerator PlayGameOverCamera()
    {
        yield return MoveCameraHeight(startOffsetY);
    }

    private IEnumerator MoveCameraHeight(float targetY)
    {
        float startY = offsetPosition.y;
        float timer = 0f;

        while (timer < cameraMoveDuration)
        {
            timer += Time.deltaTime;

            float t = Mathf.SmoothStep(0f, 1f, timer / cameraMoveDuration);

            offsetPosition.y = Mathf.Lerp(startY, targetY, t);

            yield return null;
        }

        offsetPosition.y = targetY;
    }
}
