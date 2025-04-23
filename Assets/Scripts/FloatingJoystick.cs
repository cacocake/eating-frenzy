using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

public class FloatingJoystick : MonoBehaviour {

    private Finger _movementFinger;
    private Vector2 _size = Vector2.zero;
    private RectTransform _rectTransform;
    [SerializeField] private RectTransform _knob;

    [HideInInspector] public Vector2 MovementAmount { get; private set; }

    private void Awake() {
        gameObject.SetActive(false);
        _rectTransform = GetComponent<RectTransform>();
        _size = _rectTransform.rect.size;
    }

    public void HandleFingerDown(Finger finger) {
        if(_movementFinger != null) {
            return;
        }
        
        gameObject.SetActive(true);
        
        MovementAmount = Vector2.zero;
        
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
        if(finger != _movementFinger) {
            return;
        }

        _movementFinger = null;
        _knob.anchoredPosition = Vector2.zero;
        gameObject.SetActive(false);
        MovementAmount = Vector2.zero;
    }

    public void HandleFingerMove(Finger finger) {
        if(finger != _movementFinger) {
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
        MovementAmount = knobPosition / maxMovement;
    }
}
