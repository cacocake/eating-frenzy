using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UI;

public class FloatingJoystick : MonoBehaviour {

    [SerializeField] private RectTransform _knob;
    
    private Finger _movementFinger;
    private Vector2 _size = Vector2.zero;
    private RectTransform _rectTransform;
    private const float k_maxScreenPositionHeightFactorForJoystick = 0.75f;

    public Vector2 KnobDistanceFactorFromCenter { get; private set; }

    private void Awake() {
        gameObject.SetActive(false);
        _rectTransform = GetComponent<RectTransform>();
        _size = _rectTransform.rect.size;
    }

    private bool shouldIgnoreFingerEvents() {
        return MenuManager.Instance.IsInWinLoseState() || MenuManager.isPaused;
    }

    public void HandleFingerDown(Finger finger) {
        if(_movementFinger != null || shouldIgnoreFingerEvents()) {
            return;
        }

        if(_rectTransform == null) {
            Debug.LogError("_rectTransform is null!");
            return;
        }

        if(finger.screenPosition.y > Screen.height * k_maxScreenPositionHeightFactorForJoystick) {
            return;
        }
        
        gameObject.SetActive(true);
        
        KnobDistanceFactorFromCenter = Vector2.zero;
        
        _movementFinger = finger;
        _rectTransform.anchoredPosition = ClampStartPositionToScreenBounds(finger.screenPosition);
    }

    public Vector2 ClampStartPositionToScreenBounds(Vector2 startPosition) {
        startPosition.x = Mathf.Clamp(startPosition.x, _size.x / 2.0f, 
                                      Screen.width - _size.x / 2.0f);
        startPosition.y = Mathf.Clamp(startPosition.y, _size.y / 2.0f, 
                                      Screen.height - _size.y / 2.0f);

        return startPosition;
    }

    public void HandleFingerUp(Finger finger) {
        if(finger != _movementFinger || shouldIgnoreFingerEvents()) {
            return;
        }

        if(_knob == null) {
            Debug.LogError("_knob is null!");
            return;
        }

        _movementFinger = null;
        _knob.anchoredPosition = Vector2.zero;
        gameObject.SetActive(false);
        KnobDistanceFactorFromCenter = Vector2.zero;
    }

    public void HandleFingerMove(Finger finger) {
        if(finger != _movementFinger || shouldIgnoreFingerEvents()) {
            return;
        }

        if(_knob == null) {
            Debug.LogError("_knob is null!");
            return;
        }

        Vector2 knobPosition;
        float maxMovement = _size.x / 2.0f;
        Vector2 touchScreenPosition = finger.currentTouch.screenPosition;

        if(Vector2.Distance(touchScreenPosition, _rectTransform.anchoredPosition) > maxMovement) {
            knobPosition = (touchScreenPosition - _rectTransform.anchoredPosition).normalized * maxMovement;
        } else {
            knobPosition = touchScreenPosition - _rectTransform.anchoredPosition;
        }

        _knob.anchoredPosition = knobPosition;
        KnobDistanceFactorFromCenter = knobPosition / maxMovement;
    }

}
