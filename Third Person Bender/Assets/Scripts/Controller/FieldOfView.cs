using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public Transform Eye;
    public float Radius = 60f;
    [Range(0,360)]
    public float Angle = 120f;
    [Range(0,360)]
    public float AimingAngle = 30f;
    public LayerMask TargetMask;
    public LayerMask ObstructionMask;

    private bool _canSeePlayer = false;
    public bool CanSeePlayer
    {
        get { return _canSeePlayer; }
    }
    private Transform _player;
    public Transform Player    {
        get { return _player; }
    }
    private Vector3 _directionToTarget;
    

    void Start()
    {
        StartCoroutine(FOVRoutine());
    }

    IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while(true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(Eye.position, Radius, TargetMask);

        if(rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            _directionToTarget = (target.position - Eye.position).normalized;
            
            if(Vector3.Angle(Eye.forward, _directionToTarget) < Angle/2)
            {
                float distanceToTarget = Vector3.Distance(Eye.position, target.position);

                if(!Physics.Raycast(Eye.position, _directionToTarget, distanceToTarget, ObstructionMask))
                {
                    _canSeePlayer = true;
                    _player = target;
                }
                else
                    _canSeePlayer = false;
            }
            else
                _canSeePlayer = false;
        }
        else if(_canSeePlayer)
            _canSeePlayer = false;
    }

    public bool AimingAtPlayer()
    {
        return Vector3.Angle(Eye.forward, _directionToTarget) < AimingAngle/2;
    }
}