using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Camera mainCamera;

    public static readonly string Vertical = "Vertical";
    public static readonly string Horizontal = "Horizontal";
    public static readonly string FireAxis = "Fire1";
    

    public float vertical { get; private set; }
    public float horizontal { get; private set; }
    public Vector3 aimVector { get; private set; }
    public bool Fire { get; private set; }

    
    private void Update()
    {
        MousePoint();
        vertical = Input.GetAxisRaw(Vertical);
        horizontal = Input.GetAxisRaw(Horizontal);

        Fire = Input.GetButton(FireAxis);

        Debug.Log($"{vertical} / {horizontal} / {Fire}");
    }

    private void MousePoint()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            Vector3 aimVector = hit.point - transform.position;
            aimVector.y = 0;
            this.aimVector = aimVector;
        }
    }
}
