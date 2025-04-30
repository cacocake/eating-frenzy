using System;
using UnityEngine;

public class ConsumableObject : MonoBehaviour {
    [SerializeField] private ushort _points;
    
    public ushort Points => _points;
    public static event Action<ConsumableObject> OnConsumableObjectSwallowed;
    
    private int _defaultLayer;
    private MeshRenderer _renderer;
    
    private void Start() {
        _defaultLayer = gameObject.layer;
        _renderer = transform.GetComponentInChildren<MeshRenderer>();
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
            Handheld.Vibrate();
        }

        if((!other.gameObject.CompareTag("HoleCharacter") &&
            !other.gameObject.CompareTag("PlayerHoleCharacter")) ||
            this.gameObject.layer != LayerMask.NameToLayer("HoleLayer")) {
            return;
        }
            
        this.gameObject.layer = _defaultLayer;
    }

    public void MakeObjectTransparent() {
        if(_renderer == null) {
            return;
        }
        Material material = _renderer.material;
        material.color = new Color(material.color.r, material.color.g, material.color.b, 0.5f);
    }

    public void ReturnTransparencyToNormal() {
        if(_renderer == null) {
            return;
        }
        Material material = _renderer.material;
        material.color = new Color(material.color.r, material.color.g, material.color.b, 1.0f);
    }

}
