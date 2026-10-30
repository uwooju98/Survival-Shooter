using UnityEditor.AnimatedValues;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private const string VerticalAxis = "Vertical";
    private const string HorizontalAxis = "Horizontal";
    private const string FireAxis = "Fire1";

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Camera mainCamera;
    public float Vertical { get; private set; }
    public float Horizontal { get; private set; }

    public Vector3 AimVector { get; private set; }

    public bool Fire { get; private set; }
    public bool Dash { get; private set; }


    private void Update()
    {
        UpdateMoveInput();
        UpdateAimInput();
        UpdateActionInput();
    }

    private void UpdateMoveInput()
    {
        Vertical = Input.GetAxisRaw(VerticalAxis);
        Horizontal = Input.GetAxisRaw(HorizontalAxis);
    }

    private void UpdateAimInput()
    {
        Ray ray =
            mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(
            ray,
            out RaycastHit hit,
            Mathf.Infinity,
            groundLayer))
        {
            AimVector =
                hit.point - transform.position;

            AimVector =
                new Vector3(
                    AimVector.x,
                    0f,
                    AimVector.z);
        }
    }

    private void UpdateActionInput()
    {
        Fire = Input.GetButton(FireAxis);
        Dash = Input.GetKeyDown(KeyCode.LeftShift);
    }
}
