using UnityEngine;

public class HelicopterCameraOrbit : MonoBehaviour
{
    [Header("Camera deplacement parameters")]
    [SerializeField] private Transform _helicopterCamera;
    [SerializeField] private Vector3 _targetPoint = (new Vector3(0, 1, 0));
    [SerializeField] private float _sensitivity = 0.2f;
    [SerializeField] private float _minPitch = -10f;
    [SerializeField] private float _maxPitch = 80f;

    [Header("Zoom parameters")]
    [SerializeField] private float _startDistance = 15f;
    [SerializeField] private float _minDistance = 5f;
    [SerializeField] private float _maxDistance = 30f;
    [SerializeField] private float _zoomSpeed = 1f;
    [SerializeField] private float _zoomSmoothing = 8f;

    private float _yaw;
    private float _pitch;

    private float _targetDistance;
    private float _currentDistance;

    private InputSystem_Actions _actions;
 
    private void Awake()
    {
        _actions = new InputSystem_Actions();
        enabled = false;

        if(_helicopterCamera == null)
        {
            Debug.LogError("HelicopterCamera is not assigned", this);
        }
        _targetDistance = _startDistance;
        _currentDistance = _startDistance;
    }
    private void OnEnable()
    {
        _actions.Helicopter.Enable();
    }

    private void OnDisable()
    {
        _actions.Helicopter.Disable();
    }

    private void LateUpdate()
    {
        Vector2 lookValue = _actions.Helicopter.Look.ReadValue<Vector2>();

        _yaw += lookValue.x * _sensitivity;
        _pitch += lookValue.y * _sensitivity;
        _pitch = Mathf.Clamp(_pitch, _minPitch, _maxPitch);

        float zoomValue = _actions.Helicopter.Zoom.ReadValue<float>();

        _targetDistance -= zoomValue * _zoomSpeed;
        _targetDistance = Mathf.Clamp(_targetDistance, _minDistance, _maxDistance);
        _currentDistance = Mathf.Lerp(_currentDistance, _targetDistance, Time.deltaTime * _zoomSmoothing);

        Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0f);
        _helicopterCamera.localRotation = rotation;
        _helicopterCamera.localPosition = _targetPoint - rotation * Vector3.forward * _currentDistance;
    }
}
