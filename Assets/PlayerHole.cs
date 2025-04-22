using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHole : MonoBehaviour {
    [SerializeField] private float speed = 5.0f;

    void OnTriggerStay(Collider collider) {
        if(!(collider.gameObject.tag == "Consumable")) {
            return;
        }

        collider.gameObject.layer = LayerMask.NameToLayer("HoleLayer");
    }

    void Update() {
        float moveSpeed = speed * Time.deltaTime;
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        transform.Translate(horizontalInput * moveSpeed, 0.0f, verticalInput * moveSpeed);
    }
}
