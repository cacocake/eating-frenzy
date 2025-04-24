using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using ETouch = UnityEngine.InputSystem.EnhancedTouch;

public class PlayerHole : MonoBehaviour {
    [SerializeField] private float _speed = 5.0f;
    [SerializeField] private FloatingJoystick _joystick;
    private Vector3 _baseScale;
    private const float ScaleIncreaseFactor = 0.2f;

    private void Awake() {
        _baseScale = transform.localScale;
    }

    private void Update() {
        if (MenuManager.Instance.IsInWinLoseState()) {
            return;
        }
        
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
        Vector3 scaledMovement = new Vector3(_joystick.KnobDistanceFactorFromCenter.x, 0.0f, _joystick.KnobDistanceFactorFromCenter.y) * moveSpeed;
        transform.Translate(scaledMovement);
    }

    public void IncreaseSize(float level) {
        transform.localScale = _baseScale + new Vector3(ScaleIncreaseFactor * level, 0.0f, ScaleIncreaseFactor * level);
    }
}
