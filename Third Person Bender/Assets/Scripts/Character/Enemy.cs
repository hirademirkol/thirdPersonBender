using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    public FieldOfView FOV;
    public float RunTriggerDistance = 40;
    public float FireTriggerDistance = 10;
    public float TurnSmoothTime = 0.1f;
    public float FireCooldown = 3f;

    private float _timeAfterFire = 0f;
    private float _turnSmoothVelocity;
    
    void Update()
    {
        if (IsAlive)
        {
            if (FOV.CanSeePlayer)
            {
                CalculateMovement();
                if (ReadyToFireAtPlayer())
                {
                    if (_timeAfterFire > FireCooldown)
                        StartCoroutine(Fire());
                }
            }
            _timeAfterFire += Time.deltaTime;
        }
    }

    void CalculateMovement()
    {
        var direction = FOV.Player.position - transform.position;
        var run = (direction.magnitude > RunTriggerDistance);

        if(direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;// + transform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, TurnSmoothTime);
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            MovementController.Move(moveDir, angle, run);
        }
    }

    bool ReadyToFireAtPlayer(){
        var distance = Vector3.Distance(transform.position, FOV.Player.position);
        return (distance < FireTriggerDistance) && FOV.AimingAtPlayer();

    }

    override public void Die()
    {
        IsAlive = false;
        transform.Rotate(Vector3.left, 90f);
        Debug.Log("Enemy dead.");
    }

    IEnumerator Fire()
    {
        var drawTime = RockBendingController.RockRaiseTime;
        RockBendingController.DrawRock();
        while(drawTime > 0f)
        {
            drawTime -= Time.deltaTime;
            yield return null;
        }
        Vector3 fireDirection = transform.InverseTransformPoint(FOV.Player.position) - RockBendingController.rockOffsetPosition;
        RockBendingController.PushRock(fireDirection);
        _timeAfterFire = 0f;
    }

}
