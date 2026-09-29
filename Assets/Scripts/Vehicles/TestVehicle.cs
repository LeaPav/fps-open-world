using UnityEngine;

public class VehicleScript : MonoBehaviour, IVehicle
{
    [SerializeField] private Transform _entryTransform;
    [SerializeField] private Transform _exitTransform;

    public void EnterVehicle()
    {
        Debug.Log("EnterVehicle");
    }

    public void ExitVehicle()
    {
        Debug.Log("ExitVehicle");
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
