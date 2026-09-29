using Unity.VisualScripting;
using UnityEngine;

public class VehicleScript : MonoBehaviour, IVehicle
{
    [SerializeField] private Transform _entryTransform;
    [SerializeField] private Transform _exitTransform;

    private Camera _carCamera;

    private void Awake()
    {
        _carCamera = GetComponentInChildren<Camera>();
    }

    private void Start()
    {
        _carCamera.enabled = false;
    }

    public void EnterVehicle()
    {
        Debug.Log("EnterVehicle");
        _carCamera.enabled = true;
    }

    public void ExitVehicle()
    {
        Debug.Log("ExitVehicle");
        _carCamera.enabled = false;
    }

    public Transform GetEnterPoint()
    {
        return _entryTransform;
    }
    public Transform GetExitPoint()
    {
        return _exitTransform;
    }

}
