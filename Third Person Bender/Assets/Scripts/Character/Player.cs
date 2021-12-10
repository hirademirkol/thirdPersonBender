using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    public SonarController SonarController;
    public Transform Camera;
    public float FiringDistance = 25f;
    public float TurnSmoothTime = 0.1f;
    private float _turnSmoothVelocity;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    void Update()
    {
        if (IsAlive)
        {
            //Movement Controller Input
            CalculateMovement();
            if (Input.GetButtonDown("Jump"))
                MovementController.Jump();

            //Sonar Controller Input
            if (Input.GetButtonDown("Blind"))
                SonarController.Blind();

            //Bending Controller Input
            if (Input.GetButtonDown("Fire1"))
            {
                //MovementController.RotateWithAngle(Camera.eulerAngles.y);
                RockBendingController.PushRock(Quaternion.Euler(Camera.eulerAngles.x, -Mathf.Rad2Deg * Mathf.Atan2(1f, FiringDistance) , 0f) * Vector3.forward);
            }
            if (Input.GetButtonDown("Fire2"))
                RockBendingController.DrawRock();
            if (Input.GetButtonDown("Fire3"))
                RockBendingController.RaiseWall();
        }
    }

    void CalculateMovement()
    {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            var direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            var run = Input.GetButton("Run");
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + Camera.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, TurnSmoothTime);
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            MovementController.Move(moveDir, angle, run);  
        }
        else
        {
            MovementController.SlowDown();
        }
    }

    override public void Die()
    {
        IsAlive = false;
        transform.Rotate(Vector3.left, 90f);
        Debug.Log("You dead.");
    }
}
