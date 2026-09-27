using UnityEngine;

public class PlayerCameraLook : MonoBehaviour
{

    [Header("Camera parameters")]
    [SerializeField] private float _cameraSensitivity = 20f;

    [Header("Headbob parameters")]
    [SerializeField] private float _headbobAmplitude = 0.08f;
    [SerializeField] private AnimationCurve _amplitudeCurve;
    [SerializeField] private float _headbobFrequence = 8f;
    [SerializeField] private AnimationCurve _frequenceCurve;
    [SerializeField] private float _headbobTimer = 0;
    [SerializeField] private float _headbobDuration = 2f;

    private CharacterController _characterController;

    private Camera _mainCamera;
    private Transform _mainCameraTransform;

    private float _CameraLocalYPosition;
    private float _yaw;
    private float _pitch;


    private InputSystem_Actions _actions;
    private void Awake()
    {
        _actions = new InputSystem_Actions();
        _characterController = GetComponent<CharacterController>();

        _mainCamera = GetComponentInChildren<Camera>();
        _mainCameraTransform = _mainCamera.transform;
        _CameraLocalYPosition = _mainCameraTransform.localPosition.y;
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
        HandleHeadbobOscillation();
    }

    private void HandleMouseMovement()
    {
        var lookValue = _actions.Player.Look.ReadValue<Vector2>();

        _yaw += lookValue.x * _cameraSensitivity * Time.deltaTime;
        _pitch -= lookValue.y * _cameraSensitivity * Time.deltaTime;
        _pitch = Mathf.Clamp(_pitch, -90f, 90f);

        transform.rotation = Quaternion.Euler(0, _yaw, 0);
        _mainCameraTransform.localRotation = Quaternion.Euler(_pitch, 0f, 0f);

    }

    private void HandleHeadbobOscillation()
    {

        var moveValue = _actions.Player.Move.ReadValue<Vector2>();

        if (_characterController.isGrounded && moveValue.sqrMagnitude > 0.01f)
        {
            _mainCameraTransform.localPosition = new Vector3(_mainCameraTransform.localPosition.x,
                _CameraLocalYPosition + Mathf.Sin(_headbobTimer * _headbobFrequence) * _headbobAmplitude, _mainCameraTransform.localPosition.z);

            _headbobTimer += Time.deltaTime;
        }


    }
}
