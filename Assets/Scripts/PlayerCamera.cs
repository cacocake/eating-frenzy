using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    private ConsumableObject _consumableObjectInBetweenPlayerAndCamera;
    void Update() {
        PlayerHole player = GameObject.FindWithTag("PlayerHoleCharacter").GetComponent<PlayerHole>();
        if (player == null) {
            Debug.LogError("PlayerHole component not found on GameObject with tag 'Player'.");
            return;
        }

        Vector3 direction = player.transform.position - transform.position;
        Ray ray = new Ray(transform.position, direction);

        if (Physics.Raycast(ray, out RaycastHit hit)) {
            if (hit.collider == null) {
                return;
            }

            if (hit.collider.CompareTag("PlayerHoleCharacter"))
            {
                if (_consumableObjectInBetweenPlayerAndCamera != null) {
                    _consumableObjectInBetweenPlayerAndCamera.ReturnTransparencyToNormal();
                    _consumableObjectInBetweenPlayerAndCamera = null;
                }
            } else if (hit.collider.CompareTag("Consumable")) {
                _consumableObjectInBetweenPlayerAndCamera = hit.collider.GetComponent<ConsumableObject>();
                if (_consumableObjectInBetweenPlayerAndCamera != null) {
                    _consumableObjectInBetweenPlayerAndCamera.MakeObjectTransparent();
                }
            }
        }
    }
}
