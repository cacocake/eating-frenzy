using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using ETouch = UnityEngine.InputSystem.EnhancedTouch;

public class PlayerHole : MonoBehaviour {
    [SerializeField] private float _speed = 5.0f;
    [SerializeField] private FloatingJoystick _joystick;
    [SerializeField] private float _scaleIncreaseFactor = 0.5f;
    private Vector3 _baseScale;
    private float _edgePushBackFactor = 0.5f;

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

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Edge")) {
            Debug.Log("Hit Edge!");
            Vector3 pushDirection = (transform.position - other.transform.position).normalized;
            transform.position += new Vector3(pushDirection.x, 0.0f, pushDirection.z) * _edgePushBackFactor;
        }
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
        transform.localScale = _baseScale + new Vector3(_scaleIncreaseFactor * level, 0.0f, _scaleIncreaseFactor * level);
    }
}
