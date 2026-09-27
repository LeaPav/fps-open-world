using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerCameraLook : MonoBehaviour
{

    [Header("Camera parameters")]
    [SerializeField] private float _cameraSensitivity = 20f;

    [Header("Headbob parameters")]
    [SerializeField] private float _headbobAmplitude = 0.1f;
    [SerializeField] private AnimationCurve _amplitudeCurve;
    [SerializeField] private float _headbobFrequence = 8f;
    [SerializeField] private AnimationCurve _frequenceCurve;
    [SerializeField] private float _headbobDuration = 2f;

    [Header("Sway parameters")]
    [SerializeField] private AnimationCurve _swayCurve;
    [SerializeField] private float _swayAmplitude = 0.02f;
    [SerializeField] private float _transitionSpeed = 10f;
    [SerializeField] private float _swayTimer;
    [SerializeField] private float _swayDuration = 2f;

    [Header("Event parameter")]
    [SerializeField] private FloatEventChannelSO _speedChannel;

    private CharacterController _characterController;
    private float _sprintProgress;

    private Camera _mainCamera;
    private Transform _mainCameraTransform;

    private float _headbobPhase;
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

        _swayTimer = 0f;
    }

    private void OnEnable()
    {
        _actions.Player.Enable();
        _speedChannel.OnEventRaised += HandleSpeedChanged;
    }

    private void OnDisable()
    {
        _actions.Player.Disable();
        _speedChannel.OnEventRaised -= HandleSpeedChanged;
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

        bool isMovingNow = _characterController.velocity.sqrMagnitude > 0.01f;


        // Position de headbob
        float currentFrequence = _frequenceCurve.Evaluate(_sprintProgress) * _headbobFrequence;
        _headbobPhase += currentFrequence * Time.deltaTime;
        float headBobY = _CameraLocalYPosition + Mathf.Sin(_headbobPhase) * (_amplitudeCurve.Evaluate(_sprintProgress) * _headbobAmplitude);

        // Position de sway
        _swayTimer += Time.deltaTime;
        float swayProgress = (_swayTimer % _swayDuration) / _swayDuration;
        float swayY = _CameraLocalYPosition + _swayCurve.Evaluate(swayProgress) * _swayAmplitude;

        float targetY = isMovingNow ? headBobY : swayY;

        float currentY = _mainCameraTransform.localPosition.y;
        float smoothedY = Mathf.Lerp(currentY, targetY, Time.deltaTime * _transitionSpeed);

        _mainCameraTransform.localPosition = new Vector3(_mainCameraTransform.localPosition.x, smoothedY,
               _mainCameraTransform.localPosition.z);

    }

    private void HandleSpeedChanged(float newSpeedProgress)
    {
        _sprintProgress = newSpeedProgress;
    }
}
