using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using ETouch = UnityEngine.InputSystem.EnhancedTouch;

public class PlayerHole : MonoBehaviour {
    
    [SerializeField] private float _speed = 5.0f;
    [SerializeField] private FloatingJoystick _joystick;
    [SerializeField] private float _scaleIncreaseFactor = 0.3f;
    [SerializeField] private float _scaleDurationPerLevelUp = 0.5f;
    [SerializeField] private AnimationCurve _scaleAnimationCurve;
    [SerializeField] private float _joystickKnobDistanceMaxSpeedThreshold = 0.35f;
    [SerializeField] private float _joystickKnobDeadzoneThreshold = 0.15f;
    
    private Vector3 _baseScale;

    private void Awake() {
        _baseScale = transform.localScale;
    }

    private void Update() {
        if (MenuManager.Instance.IsInWinLoseState()) {
            return;
        }
        
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

    private void GetTouchInput() {
        if (_joystick == null) {
            return;
        }

        float moveSpeed = _speed * Time.deltaTime;
        Vector3 originalScaledMovement = new Vector3(Mathf.Max(-1, Mathf.Min(_joystick.KnobDistanceFactorFromCenter.x / _joystickKnobDistanceMaxSpeedThreshold, 1)), 
                                                     0.0f, Mathf.Max(-1, Mathf.Min(_joystick.KnobDistanceFactorFromCenter.y / _joystickKnobDistanceMaxSpeedThreshold, 1))) * moveSpeed;
        Vector3 scaledMovement = Utils.GetMovementWithEdgeCollisionCheck(transform.position, 
                                                                         new Vector3(Mathf.Max(-1, Mathf.Min(_joystick.KnobDistanceFactorFromCenter.x / _joystickKnobDistanceMaxSpeedThreshold, 1)), 
                                                                                     0.0f, Mathf.Max(-1, Mathf.Min(_joystick.KnobDistanceFactorFromCenter.y / _joystickKnobDistanceMaxSpeedThreshold, 1))) * moveSpeed);
        if(originalScaledMovement == scaledMovement) {
            scaledMovement = GetDeadzonedMovement() * moveSpeed;
        }
        transform.Translate(scaledMovement);
    }

    private Vector3 GetDeadzonedMovement() {
        float xAxisDeadzone = _joystick.KnobDistanceFactorFromCenter.x < -_joystickKnobDeadzoneThreshold ? _joystick.KnobDistanceFactorFromCenter.x : 
                                                                                                           _joystick.KnobDistanceFactorFromCenter.x > _joystickKnobDeadzoneThreshold ? 
                                                                                                                                                      _joystick.KnobDistanceFactorFromCenter.x : 0.0f;  
        float yAxisDeadzone = _joystick.KnobDistanceFactorFromCenter.y < -_joystickKnobDeadzoneThreshold ? _joystick.KnobDistanceFactorFromCenter.y : 
                                                                                                           _joystick.KnobDistanceFactorFromCenter.y > _joystickKnobDeadzoneThreshold ? 
                                                                                                           _joystick.KnobDistanceFactorFromCenter.y : 0.0f;
        return new Vector3(xAxisDeadzone, 0.0f, yAxisDeadzone);
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
                                                _scaleAnimationCurve.Evaluate(progressPercentage));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.localScale = targetScale;
    }
}
