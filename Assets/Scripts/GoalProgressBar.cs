using System.Collections;
using UnityEngine;

public class GoalProgressBar : ProgressBar
{
    [SerializeField] private GoalPointsLabel _goalPointsLabel;
    private void Start() {
        MaxValue = GameManager.Instance.StageTargetPoints;
        CurrentValue = 0;
        _goalPointsLabel.UpdateLabel(GameManager.Instance.TotalPoints, GameManager.Instance.StageTargetPoints);
    }


    void OnEnable() {
        GameManager.OnPointsChanged += UpdateCurrentValue;
    }

    void OnDisable() {
        GameManager.OnPointsChanged -= UpdateCurrentValue;
    }

    private void UpdateCurrentValue() {
        if (MenuManager.Instance.IsInWinLoseState()) {
            return;
        }

        CurrentValue = GameManager.Instance.TotalPoints;
        _goalPointsLabel.UpdateLabel(GameManager.Instance.TotalPoints, GameManager.Instance.StageTargetPoints);
    }
}
