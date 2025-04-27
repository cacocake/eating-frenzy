using UnityEngine;

public class TimeLabel : InfoLabel {
    
    private void Awake() {
        _labelTextFormat = "{0}";
    }

    private void Update() {
        UpdateLabel((ushort)Mathf.Floor(_manager.StageTimeLimit));   
    }

}
