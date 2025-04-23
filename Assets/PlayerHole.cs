using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using ETouch = UnityEngine.InputSystem.EnhancedTouch;

public class PlayerHole : MonoBehaviour {
    [SerializeField] private float _speed = 5.0f;
    [SerializeField] private FloatingJoystick _joystick;
    private Finger _movementFinger;
    private Vector2 _movementAmount;
    private Vector2 _joystickSize = Vector2.zero;

    private void Start() {
        if(_joystick == null) {
            return;
        }
        _joystickSize = _joystick.RectTransform.sizeDelta;
    }

    private void OnEnable() {
        EnhancedTouchSupport.Enable();
        ETouch.Touch.onFingerDown += HandleFingerDown;
        ETouch.Touch.onFingerUp += HandleFingerUp;
        ETouch.Touch.onFingerMove += HandleFingerMove;
    }

    private void OnDisable() {
        ETouch.Touch.onFingerDown -= HandleFingerDown;
        ETouch.Touch.onFingerUp -= HandleFingerUp;
        ETouch.Touch.onFingerMove -= HandleFingerMove;
        EnhancedTouchSupport.Disable();
    }

    private void HandleFingerDown(Finger finger) {
        if(_movementFinger != null) {
            return;
        }

        _movementFinger = finger;
        _movementAmount = Vector2.zero;
        _joystick.gameObject.SetActive(true);
        _joystick.RectTransform.anchoredPosition = ClampStartPositionToScreenBounds(finger.screenPosition);
    }

    private Vector2 ClampStartPositionToScreenBounds(Vector2 startPosition) {
        startPosition.x = Math.Clamp(startPosition.x, _joystickSize.x / 2.0f, 
                                     Screen.width - _joystickSize.x / 2.0f);
        startPosition.y = Math.Clamp(startPosition.y, _joystickSize.y / 2.0f, 
                                     Screen.height - _joystickSize.y / 2.0f);

        return startPosition;
    }

    private void HandleFingerUp(Finger finger) {
        if(finger != _movementFinger) {
            return;
        }

        _movementFinger = null;
        _joystick.Knob.anchoredPosition = Vector2.zero;
        _joystick.gameObject.SetActive(false);
        _movementAmount = Vector2.zero;
    }

    private void HandleFingerMove(Finger finger) {
        if(finger != _movementFinger) {
            return;
        }

        Vector2 knobPosition;
        float maxMovement = _joystickSize.x / 2.0f;
        ETouch.Touch currentTouch = finger.currentTouch;

        if(Vector2.Distance(currentTouch.screenPosition, _joystick.RectTransform.anchoredPosition) > maxMovement) {
            knobPosition = (currentTouch.screenPosition - _joystick.RectTransform.anchoredPosition).normalized * maxMovement;
        } else {
            knobPosition = currentTouch.screenPosition - _joystick.RectTransform.anchoredPosition;
        }

        _joystick.Knob.anchoredPosition = knobPosition;
        _movementAmount = knobPosition / maxMovement;
    }

    private void Update() {
        GetKeyboardInput();
        GetTouchInput();
    }

    private void GetKeyboardInput() {
        float moveSpeed = _speed * Time.deltaTime;
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        transform.Translate(horizontalInput * moveSpeed, 0.0f, verticalInput * moveSpeed);
    }

    private void GetTouchInput() {
        float moveSpeed = _speed * Time.deltaTime;
        Vector3 scaledMovement = new Vector3(_movementAmount.x, 0.0f, _movementAmount.y) * moveSpeed;
        transform.Translate(scaledMovement);
    }
}
