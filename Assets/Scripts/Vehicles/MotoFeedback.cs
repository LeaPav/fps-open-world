using UnityEngine;

public class MotoFeedback : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _leanPoint;
    [SerializeField] private Camera _motoCamera;

    [Header("Events")]
    [SerializeField] private FloatEventChannelSO _speedRatioChannel;
    [SerializeField] private FloatEventChannelSO _steerChannel;

    [Header("Lean")]
    [SerializeField] private float _maxLeanAngle = 25f;
    [SerializeField] private float _leanSmoothing = 10f;

    [Header("FOV")]
    [SerializeField] private float _minFOV = 70f;
    [SerializeField] private float _maxFOV = 100f;
    [SerializeField] private float _fovSmoothing = 7f;

    private float _currentLean;
    private float _speedRatio;
    private float _steer;


    private void OnEnable()
    {
        _speedRatioChannel.OnEventRaised += HandleSpeedRatioChanged;
        _steerChannel.OnEventRaised += HandleSteerChanged;
    }
    private void OnDisable()
    {
        _speedRatioChannel.OnEventRaised -= HandleSpeedRatioChanged;
        _steerChannel.OnEventRaised -= HandleSteerChanged;
    }

    void Update()
    {
        HandleLean();
        HandleFOV();
    }

    private void HandleSpeedRatioChanged(float value)
    {
        _speedRatio = value;
    }

    private void HandleSteerChanged(float value)
    {
        _steer = value;
    }

    private void HandleLean()
    {
        float targetLean = -_steer * _maxLeanAngle * _speedRatio;
        _currentLean = Mathf.Lerp(_currentLean, targetLean, Time.deltaTime * _leanSmoothing);
        _leanPoint.localRotation = Quaternion.Euler(0f, 0f, _currentLean);
    }

    private void HandleFOV()
    {
        float targetFOV = Mathf.Lerp(_minFOV, _maxFOV, _speedRatio);
        _motoCamera.fieldOfView = Mathf.Lerp(_motoCamera.fieldOfView, targetFOV, _fovSmoothing * Time.deltaTime);
    }

}
