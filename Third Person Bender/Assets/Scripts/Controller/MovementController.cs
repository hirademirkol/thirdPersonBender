using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MovementController : MonoBehaviour
{
    public CharacterController Controller;
    public Animator Animator;
    public float Speed = 6f;
    public float Gravity = -9.81f;
    public float JumpHeight = 3f;
    public Transform GroundCheck;
    public float GroundDistance = 0.4f;
    public LayerMask GroundMask;
    public float RunMultiplier = 2f;
    public float Damping = 0.9f;
    public float AnimationBlendRate = 0.1f;

    private Vector3 _velocity;
    private bool _isGrounded;
    private int _jumpNumber = 0;
    private Vector3 _moveDir;
    private float _currentSpeed;
    private float _speedDamper;
    void Start()
    {
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
        
        _velocity.y += Gravity * Time.deltaTime;
        Controller.Move(_velocity * Time.deltaTime);

    }

    public void Move(Vector3 moveDir, float angle, bool run)
    {  
        if (_isGrounded)
        {
            _currentSpeed = (run ? Speed * 2f : Speed);
        }
        else
        {
            _currentSpeed = (_currentSpeed < 0f ? 0f : _currentSpeed - _speedDamper * Time.deltaTime);
        }

        Controller.Move(moveDir.normalized * _currentSpeed * Time.deltaTime);

        var animDirX = Vector3.Dot(moveDir, transform.right);
        var animDirZ = Vector3.Dot(moveDir, transform.forward);
        var turnAngle = Vector3.Angle(transform.forward, moveDir);

        var projector = Mathf.Max(Mathf.Abs(animDirX), Mathf.Abs(animDirZ));
        Animator.SetFloat("Angle", turnAngle/90f, AnimationBlendRate, Time.deltaTime);
        animDirX /= projector;
        animDirZ /= projector;

        if(turnAngle > 45)
        {
            RotateWithAngle(angle);
        }
        
        if (run)
        {
            Animator.SetFloat("MotionX", animDirX, AnimationBlendRate, Time.deltaTime);
            Animator.SetFloat("MotionZ", animDirZ, AnimationBlendRate, Time.deltaTime);
        }
        else
        {
            Animator.SetFloat("MotionX", animDirX / 2, AnimationBlendRate, Time.deltaTime);
            Animator.SetFloat("MotionZ", animDirZ / 2, AnimationBlendRate, Time.deltaTime);
        }
    }

    public void SlowDown()
    {
            Animator.SetFloat("MotionX", 0, AnimationBlendRate, Time.deltaTime);
            Animator.SetFloat("MotionZ", 0, AnimationBlendRate, Time.deltaTime);
    }

    public void Stop()
    {
        Animator.SetFloat("MotionX", 0);
        Animator.SetFloat("MotionZ", 0);
    }
    
    public void Jump()
    {
        if(_jumpNumber < 2)
        {
            _velocity.y = Mathf.Sqrt(JumpHeight * -2f * Gravity);
            _jumpNumber++;
            Animator.SetTrigger("Jump");
        }
    }

    private void RotateWithAngle(float angle)
    {
        transform.rotation = Quaternion.Euler(0f, angle, 0f);
    }
}
