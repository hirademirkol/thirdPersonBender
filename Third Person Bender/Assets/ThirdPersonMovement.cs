using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ThirdPersonMovement : MonoBehaviour
{
    public CharacterController controller;
    public Transform camera;
    public float speed = 6f;
    public float turnSmoothTime = 0.1f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public float runMultiplier = 2f;
    public float damping = 0.9f;

    private float turnSmoothVelocity;
    private Vector3 velocity;
    private bool isGrounded;
    private int jumpNumber = 0;
    private Vector3 moveDir;
    private float currentSpeed;
    private float speedDamper;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        currentSpeed = speed;
        speedDamper = 0.3f * speed;
    }
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            jumpNumber = 0;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if(direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + camera.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            if (isGrounded)
            {
                moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                currentSpeed = (Input.GetButton("Run") ? speed * 2f: speed);
            }else{
                currentSpeed = (currentSpeed<0 ? 0f : currentSpeed - speedDamper*Time.deltaTime);
            }
            
            controller.Move(moveDir.normalized * currentSpeed * Time.deltaTime);
        }
        
        if(Input.GetButtonDown("Jump") && jumpNumber < 2) 
        {   
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpNumber++;
        }
        
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
        

    }
}
