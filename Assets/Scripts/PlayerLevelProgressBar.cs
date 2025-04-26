using UnityEngine;

public class PlayerLevelProgressBar : ProgressBar {
    [SerializeField] private NextLevelProgressLabel _nextLevelProgressLabel;
    [SerializeField] private CurrentLevelLabel _currentLevelLabel;
    [SerializeField] private ParticleSystem _levelUpParticles;
    private void Start() {
        MaxValue = GameManager.Instance.CurrentLevelTargetPoints;
        CurrentValue = GameManager.Instance.CurrentLevelPoints;
    }

    void OnEnable() {
        GameManager.OnPointsChanged += UpdateCurrentValue;
        GameManager.OnLevelUp += MaxValueUpdate;
        _levelUpParticles.Stop();
    }

    void OnDisable() {
        GameManager.OnPointsChanged -= UpdateCurrentValue;
        GameManager.OnLevelUp -= MaxValueUpdate;
    }

    private void UpdateCurrentValue() {
        if (MenuManager.Instance.IsInWinLoseState()) {
            return;
        }

        CurrentValue = GameManager.Instance.CurrentLevelPoints;
        _nextLevelProgressLabel.UpdateLabel(GameManager.Instance.CurrentLevelPoints, GameManager.Instance.CurrentLevelTargetPoints);
    }

    private void MaxValueUpdate() {
        if (MenuManager.Instance.IsInWinLoseState()) {
            return;
        }

        MaxValue = GameManager.Instance.CurrentLevelTargetPoints;
        _currentLevelLabel.UpdateLabel(GameManager.Instance.CurrentLevel);
        _levelUpParticles.Play();
    }
}
