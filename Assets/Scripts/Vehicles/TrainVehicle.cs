using UnityEngine;
using UnityEngine.Splines;

public class TrainVehicle : MonoBehaviour, IVehicle
{

    [SerializeField] private float _maxSpeed = 30f;
    [SerializeField] private float _accelerationRate = 1f;
    [SerializeField] private Transform _entryPoint;
    [SerializeField] private Transform _exitPoint;

    [SerializeField] private SplineContainer _splineContainer;

    private float _distanceTravelled;
    private float _currentSpeed;
    private float _trackLength;

    private void Awake()
    {
        enabled = false;
        if (_splineContainer == null)
        {
            Debug.LogError("error : _splineContainer not found");
            enabled = false;
            return;
        }
        _trackLength = _splineContainer.CalculateLength();
    }

    public void EnterVehicle()
    {
        if (_splineContainer == null) return;
        enabled = true;
    }

    public void ExitVehicle()
    {
        enabled = false;
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
        _currentSpeed = Mathf.MoveTowards(_currentSpeed, _maxSpeed, _accelerationRate * Time.deltaTime);
        _distanceTravelled += _currentSpeed * Time.deltaTime;

        Debug.Log("currentspeed :" + _currentSpeed + "distance: "+_distanceTravelled);   

    }
}
