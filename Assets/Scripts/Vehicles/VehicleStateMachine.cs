using UnityEngine;

public enum VehicleState {
    OnFoot,
    Driving,
}

public class VehicleStateMachine : MonoBehaviour
{
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private LayerMask _vehicleLayer;
    [SerializeField] private float _overlapRadius = 3f;

    private InputSystem_Actions _actions;
    private VehicleState _state;
    private IVehicle _vehicleInterface;

    private CharacterController _characterController;
    private FirstPersonController _playerController;
    private PlayerCameraLook _playerCameraLook;
    private Camera _playerCamera;

    private void Awake()
    {
        _actions = new InputSystem_Actions();
        _state = VehicleState.OnFoot;

        _characterController = _playerTransform.GetComponent<CharacterController>();
        _playerController = _playerTransform.GetComponent<FirstPersonController>();
        _playerCameraLook = _playerTransform.GetComponent<PlayerCameraLook>();
        _playerCamera = _playerTransform.GetComponentInChildren<Camera>();
    }
    private void OnEnable()
    {
        _actions.Player.Enable();
    }

    private void OnDisable()
    {
        _actions.Player.Disable();
    }

    private void Update()
    {
        if(_actions.Player.Interact.WasPressedThisFrame())
        {
            if (_state == VehicleState.OnFoot)
            {
                Collider[] hits = Physics.OverlapSphere(_playerTransform.position, _overlapRadius, _vehicleLayer);

                if(hits.Length == 0)
                {
                    Debug.Log("Aucun véhicule proche");
                    return;
                }

                _vehicleInterface = hits[0].GetComponentInParent<IVehicle>();

                if(_vehicleInterface == null)
                {
                    Debug.LogError("Collider trouvé mais aucun IVehicule dessus");
                    return;
                }

                _vehicleInterface.EnterVehicle();

                TeleportPlayer(_vehicleInterface.GetEnterPoint());
                _characterController.enabled = false;
                _playerTransform.SetParent(_vehicleInterface.GetEnterPoint(), true);

                _playerController.enabled = false;
                _playerCameraLook.enabled = false;
                _playerCamera.enabled = false;

                _state = VehicleState.Driving;


            }
            else if(_state == VehicleState.Driving)
            {

                _vehicleInterface.ExitVehicle();

                TeleportPlayer(_vehicleInterface.GetExitPoint());
                _playerTransform.SetParent(null);

                _playerController.enabled = true;
                _playerCameraLook.enabled = true;
                _playerCamera.enabled = true;

                _state = VehicleState.OnFoot;
            }
        }
    }

    private void TeleportPlayer(Transform destination)
    {
        _characterController.enabled = false;
        _playerTransform.position = destination.position;
        _characterController.enabled = true;
    }


}
