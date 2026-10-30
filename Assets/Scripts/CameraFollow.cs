using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private PlayerInput playerInput;

    [SerializeField] public float minX;
    [SerializeField] public float maxX;
    [SerializeField] public float minZ;
    [SerializeField] public float maxZ;


    private Vector3 offsetPosition;
    private Vector3 offsetAimPosition;

    // private float startTimer;
    // private bool isStart;
    // private bool isDie;

    private void Awake()
    {
        offsetPosition = transform.position - player.position;
    }


    private void Update()
    {    

    }


    private void LateUpdate()
    {
        offsetAimPosition =
            Vector3.ClampMagnitude(playerInput.aimVector * 0.3f, 3f);

        Vector3 CameraPosition =
            player.position + offsetPosition + offsetAimPosition;



        CameraPosition.x = Mathf.Clamp(CameraPosition.x, minX, maxX);
        CameraPosition.z = Mathf.Clamp(CameraPosition.z, minZ, maxZ);
        transform.position = CameraPosition;

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
