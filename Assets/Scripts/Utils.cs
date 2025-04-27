using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
    public static Vector3 GetMovementWithEdgeCollisionCheck(Vector3 currentPosition, Vector3 movement) {
        if (Physics.Raycast(currentPosition, movement.normalized, out RaycastHit hit, movement.magnitude + 3.0f) && hit.collider.CompareTag("Edge")) {
            Vector3 edgeNormal = hit.normal;
            movement = Vector3.ProjectOnPlane(movement, edgeNormal);
        }
        return movement;
    }
}
