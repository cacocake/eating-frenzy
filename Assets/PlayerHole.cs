using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using ETouch = UnityEngine.InputSystem.EnhancedTouch;

public class PlayerHole : MonoBehaviour {
    [SerializeField] private float _speed = 5.0f;
    [SerializeField] private FloatingJoystick _joystick;

    private void Update() {
        GetKeyboardInput();
        GetTouchInput();
    }

    private void OnEnable() {
        EnhancedTouchSupport.Enable();
        ETouch.Touch.onFingerDown += _joystick.HandleFingerDown;
        ETouch.Touch.onFingerUp += _joystick.HandleFingerUp;
        ETouch.Touch.onFingerMove += _joystick.HandleFingerMove;
    }

    private void OnDisable() {
        ETouch.Touch.onFingerDown -= _joystick.HandleFingerDown;
        ETouch.Touch.onFingerUp -= _joystick.HandleFingerUp;
        ETouch.Touch.onFingerMove -= _joystick.HandleFingerMove;
        EnhancedTouchSupport.Disable();
    }

    private void GetKeyboardInput() {
        float moveSpeed = _speed * Time.deltaTime;
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        transform.Translate(horizontalInput * moveSpeed, 0.0f, verticalInput * moveSpeed);
    }

    private void GetTouchInput() {
        float moveSpeed = _speed * Time.deltaTime;
        Vector3 scaledMovement = new Vector3(_joystick.MovementAmount.x, 0.0f, _joystick.MovementAmount.y) * moveSpeed;
        transform.Translate(scaledMovement);
    }
}
