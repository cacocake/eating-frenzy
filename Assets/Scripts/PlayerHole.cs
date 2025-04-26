using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using ETouch = UnityEngine.InputSystem.EnhancedTouch;

public class PlayerHole : MonoBehaviour {
    [SerializeField] private float _speed = 5.0f;
    [SerializeField] private FloatingJoystick _joystick;
    [SerializeField] private float _scaleIncreaseFactor = 0.3f;
    [SerializeField] private float _scaleDurationPerLevelUp = 0.5f;
    private Vector3 _baseScale;
    private float _edgePushBackFactor = 0.01f;   

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

    private void OnDestroy() {
        StopAllCoroutines();
    }

    private void OnEnable() {
        EnhancedTouchSupport.Enable();
        ETouch.Touch.onFingerDown += _joystick.HandleFingerDown;
        ETouch.Touch.onFingerUp += _joystick.HandleFingerUp;
        ETouch.Touch.onFingerMove += _joystick.HandleFingerMove;
        GameManager.OnLevelUp += TriggerIncreaseSize;
    }

    private void OnDisable() {
        ETouch.Touch.onFingerDown -= _joystick.HandleFingerDown;
        ETouch.Touch.onFingerUp -= _joystick.HandleFingerUp;
        ETouch.Touch.onFingerMove -= _joystick.HandleFingerMove;
        EnhancedTouchSupport.Disable();
        GameManager.OnLevelUp -= TriggerIncreaseSize;
    }

    void OnTriggerStay(Collider other) {
        if (other.CompareTag("Edge")) {
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

    public void TriggerIncreaseSize() {
        StartCoroutine(IncreaseSizeForNewLevelOverTime());
    }

    private IEnumerator IncreaseSizeForNewLevelOverTime() {
        Vector3 targetScale = _baseScale + new Vector3(_scaleIncreaseFactor * GameManager.Instance.CurrentLevel, 
                                                       0.0f, 
                                                       _scaleIncreaseFactor * GameManager.Instance.CurrentLevel);
        Vector3 currentScale = transform.localScale;
        float elapsedTime = 0.0f;
        while (elapsedTime < _scaleDurationPerLevelUp) {
            float progressPercentage = elapsedTime / _scaleDurationPerLevelUp;
            transform.localScale = Vector3.Lerp(currentScale, 
                                                targetScale,
                                                Mathf.SmoothStep(0.0f ,1.0f, progressPercentage));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.localScale = targetScale;
    }
}
