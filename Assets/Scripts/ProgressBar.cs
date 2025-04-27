using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour {
    
    [SerializeField] private float _smoothProgressUpdateDuration = 0.25f;
    
    private Slider _progressBar;
    
    protected float MaxValue {get { return _progressBar.maxValue;} set{ _progressBar.maxValue = value;}}
    protected float CurrentValue {get { return _progressBar.value;} set{ _progressBar.value = value; }}
    
    private void Awake() {
        _progressBar = GetComponent<Slider>();
        CurrentValue = 0;
    }

    private void OnDestroy() {
        StopAllCoroutines();
    }

    protected IEnumerator IncreaseBarSmoothly(int newProgress) {
        float elapsedTime = 0.0f;
        float initialValue = CurrentValue;
        while (elapsedTime < _smoothProgressUpdateDuration) {
            float progressPercentage = elapsedTime / _smoothProgressUpdateDuration;
            CurrentValue = Mathf.SmoothStep(initialValue, newProgress, progressPercentage);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        CurrentValue = newProgress;
    }
}
