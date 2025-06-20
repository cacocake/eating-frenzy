using TMPro;
using UnityEngine;

public class InfoLabel : MonoBehaviour {
    
    protected GameManager _manager;
    protected string _labelTextFormat;

    protected virtual void Start() {
        _manager = GameManager.Instance;
    }

    public void UpdateLabel(params object[] values) {
        if(_labelTextFormat == null) {
            Debug.LogError("Label text prefix is not set!");
            return;
        }
        string formattedText = string.Format(_labelTextFormat, values);
        if(TryGetComponent<TextMeshProUGUI>(out var infoLabel)) {
            infoLabel.SetText(formattedText);
        }
    }

}
