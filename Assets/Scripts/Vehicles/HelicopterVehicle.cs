using UnityEngine;

public class HelicopterVehicle : MonoBehaviour, IVehicle
{
    [Header("Transform references")]
    [SerializeField] private Transform _entryTransform;
    [SerializeField] private Transform _exitTransform;

    [Header("Speed parameters")]
    [SerializeField] private float _maxSpeed = 70f;
    [SerializeField] private float _maxAscentSpeed = 10f;
    [SerializeField] private float _verticalAcceleration = 8f;
    [SerializeField] private float _verticalInertia = 12f;


    [Header("Rotation parameters")]
    [SerializeField] private float _maxAngularRotationSpeed = 60f;
    
    
    private Camera _helicopterCamera;
    private InputSystem_Actions _actions;

    private float _verticalValue;
    private float _yawValue;

    private float _currentVerticalSpeed;

    private Rigidbody _rigidbody;

    private void Awake()
    {
        enabled = false;

        _actions = new InputSystem_Actions();

        _helicopterCamera = GetComponentInChildren<Camera>();
        _helicopterCamera.enabled = false;

        _rigidbody = GetComponent<Rigidbody>();
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
    }

    private void FixedUpdate()
    {
        HandleVerticalMovement();   
        HandleYaw();
    }

    public void EnterVehicle()
    {
        _helicopterCamera.enabled = true;
        enabled = true;

    } 

    public void ExitVehicle()
    {
        _helicopterCamera.enabled = false;
        _currentVerticalSpeed = 0;
        enabled = false;
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
}
