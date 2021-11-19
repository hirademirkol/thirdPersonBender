using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MovementController : MonoBehaviour
{
    public CharacterController Controller;
    public Transform Camera;
    public float Speed = 6f;
    public float TurnSmoothTime = 0.1f;
    public float Gravity = -9.81f;
    public float JumpHeight = 3f;
    public Transform GroundCheck;
    public float GroundDistance = 0.4f;
    public LayerMask GroundMask;
    public float RunMultiplier = 2f;
    public float Damping = 0.9f;

    private float _turnSmoothVelocity;
    private Vector3 _velocity;
    private bool _isGrounded;
    private int _jumpNumber = 0;
    private Vector3 _moveDir;
    private float _currentSpeed;
    private float _speedDamper;

    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        _currentSpeed = Speed;
        _speedDamper = 0.3f * Speed;
    }

    void Update()
    {
        _isGrounded = Physics.CheckSphere(GroundCheck.position, GroundDistance, GroundMask);

        if(_isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
            _jumpNumber = 0;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        var direction = new Vector3(horizontal, 0f, vertical).normalized;

        if(direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + Camera.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, TurnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            if (_isGrounded)
            {
                _moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                _currentSpeed = (Input.GetButton("Run") ? Speed * 2f: Speed);
            } else {
                _currentSpeed = (_currentSpeed<0 ? 0f : _currentSpeed - _speedDamper*Time.deltaTime);
            }
            
            Controller.Move(_moveDir.normalized * _currentSpeed * Time.deltaTime);
        }
        
        if(Input.GetButtonDown("Jump") && _jumpNumber < 2) 
        {   
            _velocity.y = Mathf.Sqrt(JumpHeight * -2f * Gravity);
            _jumpNumber++;
        }
        
        _velocity.y += Gravity * Time.deltaTime;
        Controller.Move(_velocity * Time.deltaTime);
    }
}
