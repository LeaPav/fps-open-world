using UnityEngine;

public class HelicopterFeedback : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _tiltTransform;
    [SerializeField] private Transform _mainRotor;
    [SerializeField] private Transform _tailRotor;

    [Header("Tilt parameters")]
    [SerializeField] private float _maxPitchAngle = 20f;
    [SerializeField] private float _maxRollAngle = 10f;
    [SerializeField] private float _tiltSmoothing = 8f;

    [Header("Rotors")]
    [SerializeField] private float _maxRotorSpeed = 1500f;
    [SerializeField] private float _idleRotorSpeed = 300f;
    [SerializeField] private float _tailRotorMultiplier = 1.5f;
    [SerializeField] private float _rotorAcceleration = 500f;
    [SerializeField] private Vector3 _mainRotorAxis = Vector3.up;
    [SerializeField] private Vector3 _tailRotorAxis = Vector3.right;

    [Header("Events")]
    [SerializeField] private FloatEventChannelSO _forwardSpeedRatioChannel;
    [SerializeField] private FloatEventChannelSO _rightwardSpeedRatioChannel;
    [SerializeField] private BoolEventChannelSO _pilotedChannel;

    private bool _isPiloted;
    private float _currentPitch;
    private float _currentRoll;

    private float _pitchRatio;
    private float _rollRatio;

    private float _currentRotorSpeed;

    private void OnEnable()
    {
        _forwardSpeedRatioChannel.OnEventRaised += HandleForwardSpeedRatioChanged;
        _rightwardSpeedRatioChannel.OnEventRaised += HandleRightwardSpeedRatioChanged;
        _pilotedChannel.OnEventRaised += HandlePilotedChannel;
    }

    private void OnDisable()
    {
        _forwardSpeedRatioChannel.OnEventRaised -= HandleForwardSpeedRatioChanged;
        _rightwardSpeedRatioChannel.OnEventRaised -=HandleRightwardSpeedRatioChanged;
        _pilotedChannel.OnEventRaised -= HandlePilotedChannel;
    }

    void Update()
    {
        HandleTilt();
        HandleRotorsRotation();
    }

    private void HandleForwardSpeedRatioChanged(float value)
    {
        _pitchRatio = value;
    }

    private void HandleRightwardSpeedRatioChanged(float value)
    {
        _rollRatio = value;
    }

    private void HandlePilotedChannel(bool value)
    {
        _isPiloted = value;
    }
    private void HandleTilt()
    {

        float pitchAngle = _pitchRatio * _maxPitchAngle;
        float rollAngle = -_rollRatio * _maxRollAngle;

        _currentPitch = Mathf.Lerp(_currentPitch, pitchAngle, Time.deltaTime * _tiltSmoothing);
        _currentRoll = Mathf.Lerp(_currentRoll, rollAngle, Time.deltaTime * _tiltSmoothing);

        _tiltTransform.localRotation = Quaternion.Euler(_currentPitch, 0f, _currentRoll);
    }

    private void HandleRotorsRotation()
    {

        float effort = Mathf.Clamp01(new Vector2(_pitchRatio, _rollRatio).magnitude);
        float targetSpeed = _idleRotorSpeed + effort * (_maxRotorSpeed - _idleRotorSpeed);

        if (!_isPiloted) { targetSpeed = 0; }

        _currentRotorSpeed = Mathf.MoveTowards(_currentRotorSpeed, targetSpeed, _rotorAcceleration * Time.deltaTime);

        float angle = _currentRotorSpeed * Time.deltaTime;
        _mainRotor.Rotate(_mainRotorAxis, angle, Space.Self);
        _tailRotor.Rotate(_tailRotorAxis, angle * _tailRotorMultiplier, Space.Self);
    }
}
