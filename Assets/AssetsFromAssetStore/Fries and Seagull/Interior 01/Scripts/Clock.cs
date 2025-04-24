using UnityEngine;

public class Clock : MonoBehaviour {

    [SerializeField] private float _rotationSpeed = 6.0f;

    [SerializeField] private float _rotationAmount = 0.25f;

    private void Update() {
        transform.Rotate(Mathf.Sin(Time.time * _rotationSpeed) * _rotationAmount, 0.0f, Mathf.Sin(Time.time * _rotationSpeed) * _rotationAmount);
    }

}