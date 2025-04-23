using System;
using UnityEngine;

public class ConsumableObject : MonoBehaviour {
    [SerializeField] private ushort _points;
    public ushort Points => _points;
    public static event Action<ConsumableObject> OnConsumableObjectSwallowed;

    private void OnTriggerEnter(Collider other) {
        if(!other.gameObject.CompareTag("HoleCharacter") || 
             this.gameObject.layer == LayerMask.NameToLayer("HoleLayer")) {
            return;
        }

        this.gameObject.layer = LayerMask.NameToLayer("HoleLayer");
    }

    private void OnTriggerExit(Collider other) {
        if(other.gameObject.CompareTag("ConsumeAreaPlayer")) {
            OnConsumableObjectSwallowed?.Invoke(this);
            Destroy(this.gameObject);
        }

        if(!other.gameObject.CompareTag("HoleCharacter") || 
             this.gameObject.layer != LayerMask.NameToLayer("HoleLayer")) {
            return;
        }
            
        this.gameObject.layer = LayerMask.NameToLayer("Default");
    }

}
