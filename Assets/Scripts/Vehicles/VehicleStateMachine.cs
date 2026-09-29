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

    CharacterController _cc;

    private void Awake()
    {
        _actions = new InputSystem_Actions();
        _state = VehicleState.OnFoot;

        _cc = _playerTransform.GetComponent<CharacterController>();
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

                _cc.enabled = false;
                _playerTransform.localPosition = _vehicleInterface.GetEnterPoint().position;
                _cc.enabled = true;

                _state = VehicleState.Driving;
            }
            else if(_state == VehicleState.Driving)
            {

                _vehicleInterface.ExitVehicle();

                _cc.enabled = false;
                _playerTransform.localPosition = _vehicleInterface.GetExitPoint().position;
                _cc.enabled = true;

                _state = VehicleState.OnFoot;
            }
        }
    }
}
