using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class FirstPersonController : MonoBehaviour
{

    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _jumpHeight = 1.5f;

    [SerializeField] private AnimationCurve _jumpCurve;
    [SerializeField] private float _jumpDuration = 0.5f;
    [SerializeField] private float _jumpTimer;
    private float gravityValue = -9.81f;

    private InputSystem_Actions m_Actions;
    private CharacterController m_characterController;
    private Vector3 m_playerVelocity;

    bool _isOnGround;
    bool _IsJumping;

    private void Awake()
    {
        m_Actions = new InputSystem_Actions();
        m_characterController = GetComponent<CharacterController>();

        _isOnGround = false;
        _IsJumping = false;
        _jumpTimer = 0f;


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
        CheckGrounded();
        HandlePlayerJumping();
        HandlePlayerMovemenent();  
  
    }
    private void FixedUpdate()
    {
   
    }

    private void HandlePlayerMovemenent()
    {
        var moveValue = m_Actions.Player.Move.ReadValue<Vector2>();
        var moveDirection = transform.forward * moveValue.y + transform.right * moveValue.x;

        moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);

        var finalMove = moveDirection * _moveSpeed + transform.up * m_playerVelocity.y;
        m_characterController.Move(finalMove * Time.deltaTime);
    }

    private void HandlePlayerJumping()
    {
        if(_isOnGround && !_IsJumping && m_Actions.Player.Jump.WasPressedThisFrame())
        {
            _IsJumping = true;
            _jumpTimer = 0f;
        }
        if (_IsJumping)
        {
            _jumpTimer += Time.deltaTime;

            m_playerVelocity.y = _jumpCurve.Evaluate(_jumpTimer / _jumpDuration) * _jumpHeight;

            if (_jumpTimer >= _jumpDuration)
            {
                _IsJumping = false;
            }
        }
        else
        {
            m_playerVelocity.y += gravityValue * Time.deltaTime;
        }

    }

    private void CheckGrounded()
    {

        _isOnGround = m_characterController.isGrounded;
        Debug.Log(_isOnGround);

    }
}
