using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    public SonarController SonarController;
    public TerraformingController TerraformingController;
    public Camera Camera;
    public float FiringDistance = 25f;
    public float TurnSmoothTime = 0.1f;
    public LayerMask GroundMask;

    private float _turnSmoothVelocity;
    private RaycastHit hit;
    private Ray ray;

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
                        RockBendingController.PushRock(Quaternion.Euler(Camera.transform.eulerAngles.x, -Mathf.Rad2Deg * Mathf.Atan2(1f, FiringDistance), 0f) * Vector3.forward);
                    }
                    if (Input.GetButtonDown("Fire2"))
                    {
                        if(RockBendingController.DrawRock())
                            TerraformingController.PullOffRock(transform.TransformPoint(RockBendingController.rockOffsetPosition));

                    }
                    if (Input.GetButtonDown("Fire3"))
                    {
                        var position = transform.position + 3f * transform.forward;
                        RockBendingController.RaiseWall(position, TerraformingController.GetHeightAt(position));
                        //TerraformingController.RaiseWall(position, transform.forward);
                    }
                    return;
                
                case Mode.Terraform:
                ray = Camera.ScreenPointToRay(Input.mousePosition);
                    if (Input.GetButton("Fire1"))
                        if(Physics.Raycast(ray, out hit, 30f, GroundMask))
                            TerraformingController.AlterTerrainHeight(hit.point, false);
                    if (Input.GetButton("Fire2"))
                        if(Physics.Raycast(ray, out hit, 30f, GroundMask))
                            TerraformingController.AlterTerrainHeight(hit.point, true);
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
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + Camera.transform.eulerAngles.y;
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
