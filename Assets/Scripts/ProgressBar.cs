using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour {
    private Slider _progressBar;
    protected float MaxValue {get { return _progressBar.maxValue;} set{ _progressBar.maxValue = value;}}
    protected float CurrentValue {get { return _progressBar.value;} set{ _progressBar.value = value; }}
    
    private void Awake() {
        _progressBar = GetComponent<Slider>();
    }
}
