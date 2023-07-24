using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FieldOfView))]

public class FieldOfViewEditor : Editor
{
    private void OnSceneGUI()
    {
        FieldOfView fov = (FieldOfView)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fov.eyes.position, Vector3.up, Vector3.forward, 360, fov.radius); //draws circle around NPC to indicate range of FOV

        Vector3 viewAngle1 = DirectionFromAngle(fov.eyes.eulerAngles.y, -fov.angle / 2); //left side
        Vector3 viewAngle2 = DirectionFromAngle(fov.eyes.eulerAngles.y, fov.angle / 2); //right side

        Handles.color = Color.red;
        Handles.DrawLine(fov.eyes.position, fov.eyes.position + viewAngle1 * fov.radius); //draw line along angle to radius limit
        Handles.DrawLine(fov.eyes.position, fov.eyes.position + viewAngle2 * fov.radius); //draw line along angle to radius limit

        if(fov.canSeeTarget)
        {
            Handles.color = Color.green;
            Handles.DrawLine(fov.eyes.position, fov.target.position);
        }
    }

    private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad)); //copy pasted math to get the vector for the line
    }
}
