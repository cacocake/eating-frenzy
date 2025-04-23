using UnityEngine;

public class FloatingJoystick : MonoBehaviour {

    [HideInInspector] public RectTransform RectTransform;
    public RectTransform Knob;

    private void Awake() {
        gameObject.SetActive(false);
        RectTransform = GetComponent<RectTransform>();
    }
}
