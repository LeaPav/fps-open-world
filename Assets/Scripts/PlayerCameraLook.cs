using UnityEngine;

public class PlayerCameraLook : MonoBehaviour
{

    [SerializeField] private float _cameraSensitivity = 100f;

    private Camera _mainCamera;
    private Transform _mainCameraTransform;

    private float _yaw;
    private float _pitch;


    private InputSystem_Actions _actions;
    private void Awake()
    {
        _actions = new InputSystem_Actions();

        _mainCamera = GetComponentInChildren<Camera>();
        _mainCameraTransform = _mainCamera.transform;
  
    }

    private void OnEnable()
    {
        _actions.Player.Enable();
    }

    private void OnDisable()
    {
        _actions.Player.Disable();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMouseMovement();
    }

    void HandleMouseMovement()
    {
        var lookValue = _actions.Player.Look.ReadValue<Vector2>();

        _yaw += lookValue.x * _cameraSensitivity * Time.deltaTime;
        _pitch -= lookValue.y * _cameraSensitivity * Time.deltaTime;
        _pitch = Mathf.Clamp(_pitch, -90f, 90f);

        transform.rotation = Quaternion.Euler(0, _yaw, 0);
        _mainCameraTransform.localRotation = Quaternion.Euler(_pitch, 0f, 0f);

    }
}
