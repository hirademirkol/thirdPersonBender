using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    public SonarController SonarController;
    public TerraformingController TerraformingController;
    public Transform Camera;
    public float FiringDistance = 25f;
    public float TurnSmoothTime = 0.1f;

    private float _turnSmoothVelocity;

    enum Mode {Fire, Terraform}

    private Mode _mode;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _mode = Mode.Fire;
    }
    
    void Update()
    {
        if (IsAlive)
        {
            if(Input.GetButtonDown("SwitchMode"))
            {
                _mode = 1 - _mode;
                Debug.Log(_mode);
            }

            //Movement Controller Input
            CalculateMovement();
            if (Input.GetButtonDown("Jump"))
                MovementController.Jump();

            //Sonar Controller Input
            if (Input.GetButtonDown("Blind"))
                SonarController.Blind();

            //Bending Controller Input

            switch(_mode)
            {
                case Mode.Fire:
                    if (Input.GetButtonDown("Fire1"))
                    {
                        //MovementController.RotateWithAngle(Camera.eulerAngles.y);
                        RockBendingController.PushRock(Quaternion.Euler(Camera.eulerAngles.x, -Mathf.Rad2Deg * Mathf.Atan2(1f, FiringDistance), 0f) * Vector3.forward);
                    }
                    if (Input.GetButtonDown("Fire2"))
                    {
                        if(RockBendingController.DrawRock())
                            TerraformingController.PullOffRock(transform.TransformPoint(RockBendingController.rockOffsetPosition));

                    }
                    if (Input.GetButtonDown("Fire3"))
                        //RockBendingController.RaiseWall();
                        TerraformingController.RaiseWall(transform.position + 2f * transform.forward, transform.forward);
                    return;
                
                case Mode.Terraform:
                    if (Input.GetButtonDown("Fire1"))
                        TerraformingController.ColumnAtMiddle(20, 11);
                    return;
                
                default:
                    return;
            }
            
        }
    }

    void CalculateMovement()
    {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            var direction = new Vector3(horizontal, 0f, vertical).normalized;
            
            if(direction.magnitude >= 0.1f)
            {
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + Camera.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, TurnSmoothTime);
                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                MovementController.Move(moveDir, angle, Input.GetButton("Run"));
            }
    }

    override public void Die()
    {
        IsAlive = false;
        transform.Rotate(Vector3.left, 90f);
        Debug.Log("You dead.");
    }
}
