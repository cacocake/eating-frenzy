using TMPro;
using UnityEngine;

public class PointsLabel : MonoBehaviour {
    protected GameManager _manager;
    protected string _labelTextPrefix;

    protected virtual void Start() {
        _manager = GameManager.Instance;
    }

    protected void UpdateLabel(params object[] values) {
        if(_labelTextPrefix == null) {
            Debug.LogError("Label text prefix is not set.");
            return;
        }
        string formattedText = string.Format(_labelTextPrefix, values);
        GetComponent<TextMeshProUGUI>().SetText(formattedText);
    }
}
