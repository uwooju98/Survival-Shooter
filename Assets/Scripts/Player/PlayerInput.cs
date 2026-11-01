
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private const string VerticalAxis = "Vertical";
    private const string HorizontalAxis = "Horizontal";
    private const string FireAxis = "Fire1";


    [Header("References")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Camera mainCamera;

    [Header("Aim")]
    [SerializeField] private LayerMask groundLayer;


    private bool waitForFireRelease;
    private bool canControl = true;
    private bool canAim;


    public float Vertical { get; private set; }
    public float Horizontal { get; private set; }
    public Vector3 AimVector { get; private set; }

    public bool Fire { get; private set; }
    public bool Reload { get; private set; }
    public bool Dash { get; private set; }


    private void OnEnable()
    {
        gameManager.OnGameStateChanged += HandleGameStateChanged;
    }

    private void Update()
    {
        if (canAim)
        {
            UpdateAimInput();
        }

        if (!canControl)
        {
            ClearInput();
            return;
        }

        UpdateMoveInput();
        UpdateActionInput();
    }

    private void OnDisable()
    {
        gameManager.OnGameStateChanged -= HandleGameStateChanged;
    }


    private void UpdateMoveInput()
    {
        Vertical = Input.GetAxisRaw(VerticalAxis);
        Horizontal = Input.GetAxisRaw(HorizontalAxis);
    }

    private void UpdateAimInput()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            AimVector = hit.point - transform.position;
            AimVector = new Vector3(AimVector.x, 0f, AimVector.z);
        }
    }

    private void UpdateActionInput()
    {
        if (waitForFireRelease)
        {
            Fire = false;

            if (!Input.GetButton(FireAxis))
            {
                waitForFireRelease = false;
            }
        }
        else
        {
            Fire = Input.GetButton(FireAxis);
        }

        Dash = Input.GetKeyDown(KeyCode.LeftShift);
        Reload = Input.GetKeyDown(KeyCode.R);
    }


    public void ClearInput()
    {
        Vertical = 0f;
        Horizontal = 0f;

        Fire = false;
        Reload = false;
        Dash = false;
    }

    public void BlockFireUntilRelease()
    {
        waitForFireRelease = true;
        Fire = false;
    }

    public void SetControlEnabled(bool enabled)
    {
        canControl = enabled;

        if (!enabled)
        {
            ClearInput();
        }
    }


    private void HandleGameStateChanged(GameManager.GameState state)
    {
        canAim = state == GameManager.GameState.Playing ||
                 state == GameManager.GameState.WaveClear ||
                 state == GameManager.GameState.FinalWave;

        bool canPlay = state == GameManager.GameState.Playing ||
                       state == GameManager.GameState.FinalWave;

        SetControlEnabled(canPlay);
    }
}
