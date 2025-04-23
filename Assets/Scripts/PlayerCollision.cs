using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;

public class PlayerCollision : MonoBehaviour {

    void OnTriggerEnter(Collider other) {
        if(!(other.gameObject.tag == "HoleCharacter") || this.gameObject.layer == LayerMask.NameToLayer("HoleLayer")) {
            return;
        }

        this.gameObject.layer = LayerMask.NameToLayer("HoleLayer");
    }

    void OnTriggerExit(Collider other) {
        if(!(other.gameObject.tag == "HoleCharacter") || this.gameObject.layer != LayerMask.NameToLayer("HoleLayer")) {
            return;
        }
            
        this.gameObject.layer = LayerMask.NameToLayer("Default");
    }
}
