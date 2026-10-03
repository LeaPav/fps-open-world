using UnityEngine;

public class HelicopterFeedback : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private Transform _tiltTransform;

    [Header("Tilt parameters")]
    [SerializeField] private float _maxPitchAngle = 20f;
    [SerializeField] private float _maxRollAngle = 10f;
    [SerializeField] private float _tiltSmoothing = 8f;

    [Header("Events")]
    [SerializeField] private FloatEventChannelSO _forwardSpeedRatioChannel;
    [SerializeField] private FloatEventChannelSO _rightwardSpeedRatioChannel;

    private float _currentPitch;
    private float _currentRoll;

    private float _pitchRatio;
    private float _rollRatio;

    private void OnEnable()
    {
        _forwardSpeedRatioChannel.OnEventRaised += HandleForwardSpeedRatioChanged;
        _rightwardSpeedRatioChannel.OnEventRaised += HandleRightwardSpeedRatioChanged;
    }

    private void OnDisable()
    {
        _forwardSpeedRatioChannel.OnEventRaised -= HandleForwardSpeedRatioChanged;
        _rightwardSpeedRatioChannel.OnEventRaised -=HandleRightwardSpeedRatioChanged;
    }

    void Update()
    {
        HandleTilt();
    }

    private void HandleForwardSpeedRatioChanged(float value)
    {
        _pitchRatio = value;
    }

    private void HandleRightwardSpeedRatioChanged(float value)
    {
        _rollRatio = value;
    }
    private void HandleTilt()
    {

        float pitchAngle = _pitchRatio * _maxPitchAngle;
        float rollAngle = -_rollRatio * _maxRollAngle;

        _currentPitch = Mathf.Lerp(_currentPitch, pitchAngle, Time.deltaTime * _tiltSmoothing);
        _currentRoll = Mathf.Lerp(_currentRoll, rollAngle, Time.deltaTime * _tiltSmoothing);

        _tiltTransform.localRotation = Quaternion.Euler(_currentPitch, 0f, _currentRoll);
    }
}
