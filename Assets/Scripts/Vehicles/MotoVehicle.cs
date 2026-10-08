using UnityEngine;

public class MotoVehicle : MonoBehaviour, IVehicle
{
    [Header("Transform entries")]
    [SerializeField] private Transform _entryTransform;
    [SerializeField] private Transform _exitTransform;

    [Header("Driving parameters")]
    [SerializeField] private float _maxSpeed = 40f;
    [SerializeField] private float _backwardSpeed = -1.4f;
    [SerializeField] private float _acceleration = 20f;
    [SerializeField] private float _reverseAcceleration = 5f;
    [SerializeField] private float _brakeForce = 30f;
    [SerializeField] private float _inertia = 8f;

    [Header("Motocycle Rotation parameters")]
    [SerializeField] private float _maxSpeedRotation = 90f;
    [SerializeField] private AnimationCurve _turnCurve;


    [SerializeField] private float _steerSpeed = 4f;

    [Header("Collision")]
    [SerializeField] private float _bounceFactor = 0.2f;
    [SerializeField] private float _minSpeedImpact = 0.3f;
    [SerializeField] private float _knockbackDecay = 8f;

    [Header("Events")]
    [SerializeField] private FloatEventChannelSO _speedRatioChannel;
    [SerializeField] private FloatEventChannelSO _steerChannel;

    private Vector3 _knockback;

    private float _currentSpeed;
    private float _speedRatio;

    private float _steer;

    private Rigidbody _rigidbody;

    private Camera _motoCamera;

    private InputSystem_Actions _actions;
    private Vector2 _moveValue;

    private void Awake()
    {
        _actions = new InputSystem_Actions();

        _motoCamera = GetComponentInChildren<Camera>();
        _motoCamera.enabled = false;

        _rigidbody = GetComponent<Rigidbody>();
    }
    private void OnEnable()
    {
        _actions.Vehicle.Enable();
    }

    private void OnDisable()
    {
        _actions.Vehicle.Disable();
    }

    private void Update()
    {
        _moveValue = _actions.Vehicle.Move.ReadValue<Vector2>();
        _steer = Mathf.MoveTowards(_steer, _moveValue.x, _steerSpeed * Time.deltaTime);

        _speedRatioChannel.Raise(_speedRatio);
        _steerChannel.Raise(_steer);


    }
    private void FixedUpdate()
    {
        HandleMotorcycleMovement();
        HandleMotorcycleRotation();
    }
    public void EnterVehicle()
    {
        _motoCamera.enabled = true;
        enabled = true;
    }

    public void ExitVehicle()
    {
        _motoCamera.enabled = false;
        enabled = false;
        _currentSpeed = 0;
        _rigidbody.linearVelocity = Vector3.zero;
        _speedRatio = 0;
        _speedRatioChannel.Raise(0f);
        _steerChannel.Raise(0f);
        _steer= 0;
        _knockback = Vector3.zero;
    }

    public Transform GetEnterPoint()
    {
        return _entryTransform;
    }
    public Transform GetExitPoint()
    {
        return _exitTransform;
    }

    private void HandleMotorcycleMovement()
    {
        float targetSpeed;
        float rate;

        if(_moveValue.y > 0)
        {
            targetSpeed = _maxSpeed ;
            rate = _acceleration;
        }
        else if(_moveValue.y < 0)
        {
            if(_currentSpeed > 0)
            {
                targetSpeed = 0;
                rate = _brakeForce;
            }
            else
            {
                targetSpeed = _backwardSpeed;
                rate = _reverseAcceleration;
            }
        }
        else
        {
            targetSpeed = 0;
            rate = _inertia;
        }

        float realForwardSpeed = Vector3.Dot(_rigidbody.linearVelocity, transform.forward);
        if (_currentSpeed > 0)
        {
            _currentSpeed = Mathf.Min(_currentSpeed, Mathf.Max(0f, realForwardSpeed));
        }
        else
        {
            _currentSpeed = Mathf.Max(_currentSpeed, Mathf.Min(0f, realForwardSpeed));
        }

        _currentSpeed = Mathf.MoveTowards(_currentSpeed, targetSpeed, rate * Time.fixedDeltaTime);

 

        _knockback = Vector3.MoveTowards(_knockback, Vector3.zero, _knockbackDecay * Time.fixedDeltaTime);

        Vector3 velocity = transform.forward * _currentSpeed + _knockback;
        velocity.y = _rigidbody.linearVelocity.y;
        _rigidbody.linearVelocity = velocity;

    }

    private void HandleMotorcycleRotation()
    {

        float referenceSpeed = _currentSpeed >= 0 ? _maxSpeed : Mathf.Abs(_backwardSpeed);

        _speedRatio = Mathf.Abs( _currentSpeed)/referenceSpeed;

        float turnFactor = _turnCurve.Evaluate(_speedRatio);
        float angle = _steer * _maxSpeedRotation * turnFactor * Time.fixedDeltaTime;
        if(_currentSpeed < 0) { angle = -angle; }

        _rigidbody.MoveRotation(_rigidbody.rotation * Quaternion.Euler(0f, angle, 0f));
    }

    private void OnCollisionEnter(Collision collision)
    {
        float impactSpeed = collision.relativeVelocity.magnitude;
        if (impactSpeed < _minSpeedImpact) return;

        ContactPoint contactPoint = collision.GetContact(0);
        Vector3 moveDirection = transform.forward * Mathf.Sign(_currentSpeed);

        float headOn = Mathf.Clamp01(-Vector3.Dot(contactPoint.normal, moveDirection));

        Vector3 pushDirection = Vector3.ProjectOnPlane(contactPoint.normal, Vector3.up).normalized;
        _knockback = pushDirection * impactSpeed * headOn * _bounceFactor;

        _currentSpeed *= 1f - headOn * 0.8f;

    }
}
