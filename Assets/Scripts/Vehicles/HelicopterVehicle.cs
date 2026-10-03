using System.Runtime.CompilerServices;
using UnityEngine;

public class HelicopterVehicle : MonoBehaviour, IVehicle
{
    [Header("Transform references")]
    [SerializeField] private Transform _entryTransform;
    [SerializeField] private Transform _exitTransform;

    private Camera _helicopterCamera;

    private void Awake()
    {
        enabled = false;

        _helicopterCamera = GetComponentInChildren<Camera>();
        _helicopterCamera.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnterVehicle()
    {
        _helicopterCamera.enabled = true;
        enabled = true;

    } 

    public void ExitVehicle()
    {
        _helicopterCamera.enabled = false;
        enabled = false;
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
