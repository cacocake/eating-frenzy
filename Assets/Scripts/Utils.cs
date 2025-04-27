using UnityEngine;

public class Utils : MonoBehaviour {

    public static Vector3 GetMovementWithEdgeCollisionCheck(Vector3 currentPosition, Vector3 movement) {
        RaycastHit[] playerMovementHits = new RaycastHit[1];
        if (Physics.RaycastNonAlloc(currentPosition, movement.normalized, playerMovementHits, movement.magnitude, LayerMask.GetMask("Edges")) > 0) {
            movement = Vector3.ProjectOnPlane(movement, playerMovementHits[0].normal);
            // Second ray to see if the projected movement will hit another edge to avoid issues against corners
            if(Physics.RaycastNonAlloc(currentPosition, movement.normalized, playerMovementHits, movement.magnitude, LayerMask.GetMask("Edges")) > 0) {
                movement = Vector3.ProjectOnPlane(movement, playerMovementHits[0].normal);
            }
        }
        return movement;
    }
    
}
