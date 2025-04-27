using System;
using UnityEngine;

public class ConsumableObject : MonoBehaviour {
    [SerializeField] private ushort _points;
    
    public ushort Points => _points;
    public static event Action<ConsumableObject> OnConsumableObjectSwallowed;
    
    private int _defaultLayer;
    
    private void Start() {
        _defaultLayer = gameObject.layer;
    }

    private void OnTriggerEnter(Collider other) {
        if((!other.gameObject.CompareTag("HoleCharacter") &&
            !other.gameObject.CompareTag("PlayerHoleCharacter")) || 
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

        if((!other.gameObject.CompareTag("HoleCharacter") &&
            !other.gameObject.CompareTag("PlayerHoleCharacter")) ||
            this.gameObject.layer != LayerMask.NameToLayer("HoleLayer")) {
            return;
        }
            
        this.gameObject.layer = _defaultLayer;
    }

    public void MakeObjectTransparent() {
        MeshRenderer renderer = transform.GetComponentInChildren<MeshRenderer>();
        if(renderer == null) {
            return;
        }
        renderer.material.color = new Color(1, 1, 1, 0.5f);
    }

    public void ReturnTransparencyToNormal() {
        MeshRenderer renderer = transform.GetComponentInChildren<MeshRenderer>();
        if(renderer == null) {
            return;
        }
        renderer.material.color = new Color(1, 1, 1, 1);
    }

}
