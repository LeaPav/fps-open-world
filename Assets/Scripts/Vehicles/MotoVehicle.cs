using UnityEngine;

public class MotoVehicle : MonoBehaviour, IVehicle
{
    [SerializeField] private Transform _entryTransform;
    [SerializeField] private Transform _exitTransform;

    [SerializeField] private float _maxSpeed = 40f;
    [SerializeField] private float _acceleration = 20f;
    [SerializeField] private float _brakeForce = 40f;
    [SerializeField] private float _inertia = 5f;

    [SerializeField] private float _maxSpeedRotation = 60f;
    [SerializeField] private AnimationCurve _turnCurve;

    private Rigidbody _rigidbody;

    private Camera _motoCamera;

    private InputSystem_Actions _actions;
    private Vector2 _moveValue;
    private float _currentSpeed;

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

    }
    private void FixedUpdate()
    {
        HandleMotorcycleMovement();
        HandleMotorcycleRotation();
    }
    public void EnterVehicle()
    {
        Debug.Log("EnterVehicle");
        _motoCamera.enabled = true;
        enabled = true;
    }

    public void ExitVehicle()
    {
        Debug.Log("ExitVehicle");
        _motoCamera.enabled = false;
        enabled = false;
        _currentSpeed = 0;
        _rigidbody.linearVelocity = Vector3.zero;
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
            targetSpeed = 0;
            rate = _brakeForce;
        }
        else
        {
            targetSpeed = 0;
            rate = _inertia;
        }

        float realForwardSpeed = Vector3.Dot(_rigidbody.linearVelocity, transform.forward);
        _currentSpeed = Mathf.Min(_currentSpeed, Mathf.Max(0f, realForwardSpeed));
        _currentSpeed = Mathf.MoveTowards(_currentSpeed, targetSpeed, rate * Time.fixedDeltaTime);

        //transform.position += transform.forward* _currentSpeed * Time.fixedDeltaTime;

        Vector3 velocity = transform.forward * _currentSpeed;
        velocity.y = _rigidbody.linearVelocity.y;
        _rigidbody.linearVelocity = velocity;

    }

    private void HandleMotorcycleRotation()
    {
        
        float speedRatio = _currentSpeed/_maxSpeed;
        float turnFactor = _turnCurve.Evaluate(speedRatio);
        float angle = _moveValue.x * _maxSpeedRotation * turnFactor * Time.fixedDeltaTime;

        //transform.Rotate(0f, angle, 0f);
        _rigidbody.MoveRotation(_rigidbody.rotation * Quaternion.Euler(0f, angle, 0f));
    }
}
