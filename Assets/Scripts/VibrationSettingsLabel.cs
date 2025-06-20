using UnityEngine;
using TMPro;

public class VibrationSettingsLabel : MonoBehaviour {

    private const string k_enableVibrationLabel = "ENABLE VIBRATION";
    private const string k_disableVibrationLabel = "DISABLE VIBRATION";

    private void Start() {
        HandleVibrationSettingChanged();
    }
    
    private void OnEnable() {
        MenuManager.OnVibrationSettingChanged += HandleVibrationSettingChanged;
    }

    private void OnDisable() {
        MenuManager.OnVibrationSettingChanged -= HandleVibrationSettingChanged;
    }

    private void HandleVibrationSettingChanged() {
        if(TryGetComponent<TextMeshProUGUI>(out var vibrationSettingsLabel)) {
            vibrationSettingsLabel.SetText(MenuManager.AreVibrationsEnabled ? k_disableVibrationLabel : k_enableVibrationLabel); 
        }
    }

}
