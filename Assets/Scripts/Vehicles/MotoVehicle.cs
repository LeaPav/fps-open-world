using UnityEngine;

public class MotoVehicle : MonoBehaviour, IVehicle
{
    [SerializeField] private Transform _entryTransform;
    [SerializeField] private Transform _exitTransform;

    private Camera _motoCamera;

    [SerializeField] private float _maxSpeed = 40f;
    [SerializeField] private float _acceleration = 20f;
    [SerializeField] private float _brakeForce = 40f;
    [SerializeField] private float _inertia = 5f;

    private InputSystem_Actions _actions;
    private Vector2 _moveValue;
    private float _currentSpeed;

    private void Awake()
    {
        _actions = new InputSystem_Actions();

        _motoCamera = GetComponentInChildren<Camera>();
        _motoCamera.enabled = false;
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

        HandleMotocycleMovement();
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
    }

    public Transform GetEnterPoint()
    {
        return _entryTransform;
    }
    public Transform GetExitPoint()
    {
        return _exitTransform;
    }

    private void HandleMotocycleMovement()
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
        
        _currentSpeed = Mathf.MoveTowards(_currentSpeed, targetSpeed, rate * Time.deltaTime);

        transform.position += transform.forward* _currentSpeed * Time.deltaTime;
    }
}
