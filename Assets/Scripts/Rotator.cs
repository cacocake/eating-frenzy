using UnityEngine;

public class Rotator : MonoBehaviour {

    [SerializeField] private float _speed = 10.0f;

    void Update() {
        transform.Rotate(Vector3.up, _speed * Time.deltaTime);
    }

}
