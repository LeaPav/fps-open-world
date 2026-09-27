using UnityEngine;
using UnityEngine.InputSystem;


public class FirstPersonController : MonoBehaviour
{

    [Header("Speed parameters")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _sprintSpeed = 7f;

    [Header("Jump parameters")]
    [SerializeField] private float _jumpHeight = 5.5f;
    [SerializeField] private AnimationCurve _jumpCurve;
    [SerializeField] private AnimationCurve _sprintCurve;

    private Vector2 _moveValue;
    private float _currentSpeed;
    private float _gravityValue = -9.81f;

    private float _jumpDuration = 0.5f;
    private float _jumpTimer;

    private float _sprintRampDuration = 1f;
    private float _sprintDecayDuration = 0.5f;
    private float _sprintProgress;

    private InputSystem_Actions _actions;
    private CharacterController _characterController;
    private Vector3 _playerVelocity;
 
    bool _isOnGround;
    bool _isJumping;

    private void Awake()
    {
        _actions = new InputSystem_Actions();
        _characterController = GetComponent<CharacterController>();

        _isOnGround = false;
        _isJumping = false;
        _jumpTimer = 0f;

        _currentSpeed = _moveSpeed;

    }

    private void OnEnable()
    {
        _actions.Player.Enable();
    }

    private void OnDisable()
    {
        _actions.Player.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        _moveValue = _actions.Player.Move.ReadValue<Vector2>();

        CheckGrounded();
        HandlePlayerJumping();
        HandlePlayerMovemenent();
        HandlePlayerSprint();
    }

    private void HandlePlayerMovemenent()
    {
        var moveDirection = transform.forward * _moveValue.y + transform.right * _moveValue.x;

        moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);

        var finalMove = moveDirection * _currentSpeed + transform.up * _playerVelocity.y;
        _characterController.Move(finalMove * Time.deltaTime);
    }

    private void HandlePlayerJumping()
    {
        if(_isOnGround && !_isJumping && _actions.Player.Jump.WasPressedThisFrame())
        {
            _isJumping = true;
            _jumpTimer = 0f;
        }
        if (_isJumping)
        {
            _jumpTimer += Time.deltaTime;

            _playerVelocity.y = _jumpCurve.Evaluate(_jumpTimer / _jumpDuration) * _jumpHeight;

            if (_jumpTimer >= _jumpDuration)
            {
                _isJumping = false;
            }
        }
        else
        {
            _playerVelocity.y += _gravityValue * Time.deltaTime;
        }

    }

    private void HandlePlayerSprint()
    {

        if (_actions.Player.Sprint.IsPressed() && _moveValue.y > 0)
        {
            _sprintProgress += Time.deltaTime / _sprintRampDuration;

        }
        else
        {
            _sprintProgress -= Time.deltaTime / _sprintDecayDuration;
        }

        _sprintProgress = Mathf.Clamp01(_sprintProgress);

        _currentSpeed = _moveSpeed + _sprintCurve.Evaluate(_sprintProgress) * _sprintSpeed;
    }
    private void CheckGrounded()
    {
        _isOnGround = _characterController.isGrounded;
    }
}
