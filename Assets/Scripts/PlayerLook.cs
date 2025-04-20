using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [Header("Look Settings")]
    [SerializeField] private float lookSensitivity = 5f;
    private float _cameraPitch = 0f;
    private PlayerInputActions _playerControls;
    private InputAction _look;
    private InputAction _screenLock;
    private bool _locked = false;

    private void Awake()
    {
        _playerControls = new PlayerInputActions();
    }

    private void OnEnable()
    {
        _screenLock = _playerControls.Player.Lock;
        _look = _playerControls.Player.Look;
        _look.Enable();
        _screenLock.Enable();
    }

    private void OnDisable()
    {
        _look.Disable();
        _screenLock.Disable();
    }
    private void Update()
    {
        if (Cursor.lockState == CursorLockMode.Locked) HandleCameraRotation();


        if (_screenLock.triggered)
        {
            _locked = !_locked;
            if (_locked) {
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
        Vector2 lookInput = _look.ReadValue<Vector2>() * lookSensitivity;
        RotatePlayer(lookInput.x);
        UpdateCameraPitch(lookInput.y);
    }

    private void RotatePlayer(float xRotation)
    {
        transform.Rotate(Vector3.up * xRotation);
    }

    private void UpdateCameraPitch(float yRotation)
    {
        _cameraPitch = Mathf.Clamp(_cameraPitch - yRotation, -90f, 90f);
        cameraTransform.localEulerAngles = Vector3.right * _cameraPitch;
    }
    #endregion
}
