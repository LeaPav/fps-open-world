using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class TrainVehicle : MonoBehaviour, IVehicle
{

    [SerializeField] private float _maxSpeed = 30f;
    [SerializeField] private float _accelerationRate = 1.5f;
    [SerializeField] private float _brakeForce = 5f;
    [SerializeField] private float _inertia = 1f;
    [SerializeField] private Transform _entryPoint;
    [SerializeField] private Transform _exitPoint;

    [SerializeField] private SplineContainer _splineContainer;

    private Camera _camera;
    private TrainCamera _trainCameraScript;

    private float _distanceTravelled;
    private float _currentSpeed;
    private float _trackLength;

    private InputSystem_Actions _actions;

    private float _moveValue;

    private void Awake()
    {
        enabled = false;
        if (_splineContainer == null)
        {
            Debug.LogError("error : _splineContainer not found");
            return;
        }
        _camera = GetComponentInChildren<Camera>();
        _trackLength = _splineContainer.CalculateLength();
        _camera.enabled = false;
        _trainCameraScript = _camera.GetComponent<TrainCamera>();

        _actions = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        _actions.Train.Enable();
    }

    private void OnDisable()
    {
        _actions.Train.Disable();
    }

    public void EnterVehicle()
    {
        if (_splineContainer == null) return;
        enabled = true;
        _camera.enabled = true;
        _trainCameraScript.enabled = true;
       
    }

    public void ExitVehicle()
    {
        enabled = false;
        _camera.enabled = false;
        _trainCameraScript.enabled = false;
    }

    public Transform GetEnterPoint()
    {
        return _entryPoint;
    }

    public Transform GetExitPoint()
    {
        return _exitPoint;
    }

    private void Update()
    {
        _moveValue = _actions.Train.Throttle.ReadValue<float>();

        HandleTrainMovement();

    }

    private void HandleTrainMovement()
    {
        float targetSpeed;
        float rate;

        if(_moveValue > 0)
        {
            targetSpeed = _maxSpeed;
            rate = _accelerationRate;
        }else if(_moveValue < 0)
        {
            targetSpeed = 0;
            rate = _brakeForce;
        }
        else
        {
            targetSpeed = 0;
            rate = _inertia;
        }

         _currentSpeed = Mathf.MoveTowards(_currentSpeed, targetSpeed, rate * Time.deltaTime);
        _distanceTravelled += _currentSpeed * Time.deltaTime;

        _distanceTravelled = Mathf.Clamp(_distanceTravelled, 0f, _trackLength);

        if (_distanceTravelled == _trackLength)
        {
            _currentSpeed = 0f;
        }

        float t = _distanceTravelled / _trackLength;
        float3 position;
        float3 tangent;
        float3 upVector;

        _splineContainer.Evaluate(t, out position, out tangent, out upVector);
        transform.position = position;
        transform.rotation = Quaternion.LookRotation(tangent, upVector);

    }
}
