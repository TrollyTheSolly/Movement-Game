using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [Header("Look Settings")]
    [SerializeField] private float lookSensitivity = 5f;
    private float cameraPitch = 0f;
    private PlayerInputActions playerControls;
    private InputAction look;
    private InputAction screenLock;
    private bool locked = false;

    private void Awake()
    {
        playerControls = new PlayerInputActions();
    }

    private void OnEnable()
    {
        screenLock = playerControls.Player.Lock;
        look = playerControls.Player.Look;
        look.Enable();
        screenLock.Enable();
    }

    private void OnDisable()
    {
        look.Disable();
        screenLock.Disable();
    }
    void Update()
    {
        if (Cursor.lockState == CursorLockMode.Locked) HandleCameraRotation();


        if (screenLock.triggered)
        {
            locked = !locked;
            if (locked) {
                Cursor.lockState = CursorLockMode.Locked;
            } else
            {
                Cursor.lockState = CursorLockMode.None;
            }
        }
    }

    #region Look Logic
    private void HandleCameraRotation()
    {
        Vector2 lookInput = look.ReadValue<Vector2>() * lookSensitivity;
        RotatePlayer(lookInput.x);
        UpdateCameraPitch(lookInput.y);
    }

    private void RotatePlayer(float xRotation)
    {
        transform.Rotate(Vector3.up * xRotation);
    }

    private void UpdateCameraPitch(float yRotation)
    {
        cameraPitch = Mathf.Clamp(cameraPitch - yRotation, -90f, 90f);
        cameraTransform.localEulerAngles = Vector3.right * cameraPitch;
    }
    #endregion
}
