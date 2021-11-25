using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FieldOfView))]
public class FieldOfViewEditor : Editor
{
    void OnSceneGUI()
    {
        FieldOfView fov = (FieldOfView)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fov.Eye.position, Vector3.up, Vector3.forward, 360, fov.Radius);
        
        Vector3 direction1 = DirectionFromAngle(fov.Eye.eulerAngles.y, -fov.Angle/2);
        Vector3 direction2 = DirectionFromAngle(fov.Eye.eulerAngles.y, fov.Angle/2);

        Vector3 direction3 = DirectionFromAngle(fov.Eye.eulerAngles.y, -fov.AimingAngle/2);
        Vector3 direction4 = DirectionFromAngle(fov.Eye.eulerAngles.y, fov.AimingAngle/2);

        Handles.color = Color.green;
        Handles.DrawLine(fov.Eye.position, fov.Eye.position + direction1 * fov.Radius);
        Handles.DrawLine(fov.Eye.position, fov.Eye.position + direction2 * fov.Radius);

        Handles.color = Color.yellow;
        Handles.DrawLine(fov.Eye.position, fov.Eye.position + direction3 * fov.Radius);
        Handles.DrawLine(fov.Eye.position, fov.Eye.position + direction4 * fov.Radius);


        if(fov.CanSeePlayer)
        {
            Handles.color = Color.red;
            Handles.DrawLine(fov.Eye.position, fov.Player.position);
        }


    }

    Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;
        
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0f, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
