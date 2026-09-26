using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonController : MonoBehaviour
{

    [SerializeField] private float _moveSpeed = 15;

    private InputSystem_Actions m_Actions;

    private void Awake()
    {
        m_Actions = new InputSystem_Actions();

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
  
    }

    private void OnEnable()
    {
        m_Actions.Player.Enable();
    }

    private void OnDisable()
    {
        m_Actions.Player.Disable();
    }

    // Update is called once per frame
    void Update()
    {
      HandlePlayerMovemenent();  
    }


    private void HandlePlayerMovemenent()
    {
        var moveValue = m_Actions.Player.Move.ReadValue<Vector2>();
        var moveDirection = transform.forward * moveValue.y + transform.right * moveValue.x;

        transform.position += moveDirection * _moveSpeed * Time.deltaTime;
    }

}
