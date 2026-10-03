using UnityEngine;

public class HelicopterVehicle : MonoBehaviour, IVehicle
{
    [Header("References")]
    [SerializeField] private Transform _entryTransform;
    [SerializeField] private Transform _exitTransform; 

    [Header("Horizontal Speed parameters")]
    [SerializeField] private float _maxHorizontalSpeed = 70f;
    [SerializeField] private float _horizontalAcceleration = 35f;
    [SerializeField] private float _horizontalInertia = 20f;

    [Header("Vertical Speed parameters")]
    [SerializeField] private float _maxAscentSpeed = 10f;
    [SerializeField] private float _verticalAcceleration = 8f;
    [SerializeField] private float _verticalInertia = 12f;

    [Header("Rotation parameters")]
    [SerializeField] private float _maxAngularRotationSpeed = 60f;

    [Header("Events")]
    [SerializeField]  private FloatEventChannelSO _forwardSpeedRatioChannel;
    [SerializeField]  private FloatEventChannelSO _rightwardSpeedRatioChannel;


    private Camera _helicopterCamera;
    private InputSystem_Actions _actions;

    private float _verticalValue;
    private float _yawValue;
    private Vector2 _moveValue;
    private Vector3 _horizontalVelocity;

    private float _currentVerticalSpeed;

    private Rigidbody _rigidbody;
    private HelicopterCameraOrbit _cameraOrbitScript;

    private void Awake()
    {
        enabled = false;

        _actions = new InputSystem_Actions();

        _cameraOrbitScript = GetComponent<HelicopterCameraOrbit>();

        _helicopterCamera = GetComponentInChildren<Camera>();
        _helicopterCamera.enabled = false;

        _rigidbody = GetComponent<Rigidbody>();

        _horizontalVelocity = Vector3.zero;
    }

    private void OnEnable()
    {
        _actions.Helicopter.Enable();
    }
    private void OnDisable()
    {
        _actions.Helicopter.Disable();
    }

    void Update()
    {
        _verticalValue = _actions.Helicopter.Vertical.ReadValue<float>();
        _yawValue = _actions.Helicopter.Yaw.ReadValue<float>();
        _moveValue = _actions.Helicopter.Move.ReadValue<Vector2>();

        BroadcastTiltSpeed();
    }

    private void FixedUpdate()
    {
        HandleVerticalMovement();   
        HandleYaw();
        HandleMovement();
    }

    public void EnterVehicle()
    {
        _helicopterCamera.enabled = true;
        enabled = true;
        _cameraOrbitScript.enabled = true;


    } 

    public void ExitVehicle()
    {
        _helicopterCamera.enabled = false;
        _currentVerticalSpeed = 0;
        _horizontalVelocity = Vector3.zero;
        enabled = false;
        _rigidbody.linearVelocity = Vector3.zero;
        _forwardSpeedRatioChannel.Raise(0f);
        _rightwardSpeedRatioChannel.Raise(0f);
        _cameraOrbitScript.enabled = false;
    }

    public Transform GetEnterPoint()
    {
        return _entryTransform;
    }

    public Transform GetExitPoint()
    {
        return _exitTransform;
    }

    private void HandleVerticalMovement()
    {
        float targetSpeed;
        float rate;

        if(_verticalValue > 0)
        {
            targetSpeed = _maxAscentSpeed;
            rate = _verticalAcceleration;
        }
        else if(_verticalValue < 0)
        {
            targetSpeed= -_maxAscentSpeed;
            rate = _verticalAcceleration;
        }
        else
        {
            targetSpeed = 0;
            rate = _verticalInertia;
        }

        _currentVerticalSpeed = Mathf.MoveTowards(_currentVerticalSpeed, targetSpeed, rate * Time.fixedDeltaTime);

        Vector3 velocity = _rigidbody.linearVelocity;
        velocity.y = _currentVerticalSpeed;
        _rigidbody.linearVelocity = velocity;
    }

    private void HandleYaw()
    {
        float angle = _yawValue * _maxAngularRotationSpeed * Time.fixedDeltaTime;

        _rigidbody.MoveRotation(_rigidbody.rotation * Quaternion.Euler(0f, angle, 0f));
    }

    private void HandleMovement()
    {
        Vector3 target;
        float rate;

        var moveDirection = transform.forward * _moveValue.y + transform.right * _moveValue.x;
        moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);

        rate = (_moveValue == Vector2.zero) ? _horizontalInertia : _horizontalAcceleration;
        target = moveDirection * _maxHorizontalSpeed;

        _horizontalVelocity = Vector3.MoveTowards(_horizontalVelocity, target, rate * Time.fixedDeltaTime);

        Vector3 velocity = _rigidbody.linearVelocity;
        velocity.x = _horizontalVelocity.x; 
        velocity.z = _horizontalVelocity.z;
        _rigidbody.linearVelocity = velocity;
    }

    private void BroadcastTiltSpeed()
    {
        Vector3 localVelocity = transform.InverseTransformDirection(_horizontalVelocity);

        float forwardRatio = localVelocity.z / _maxHorizontalSpeed;
        float rightwardRatio  = localVelocity.x / _maxHorizontalSpeed;

        _forwardSpeedRatioChannel.Raise(forwardRatio);
        _rightwardSpeedRatioChannel.Raise(rightwardRatio);

    }
}
