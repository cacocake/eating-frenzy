using UnityEngine;

public class GoalProgressBar : ProgressBar
{
    [SerializeField] private GoalPointsLabel _goalPointsLabel;
    private void Start() {
        MaxValue = GameManager.Instance.StageTargetPoints;
        if(_goalPointsLabel) { 
            _goalPointsLabel.UpdateLabel(GameManager.Instance.TotalPoints, GameManager.Instance.StageTargetPoints);
        }
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

        if(_goalPointsLabel == null) {
            Debug.LogError("Trying to update _goalPointsLabel, but it's null!");
        }

        StartCoroutine(IncreaseBarSmoothly(GameManager.Instance.TotalPoints));
        _goalPointsLabel.UpdateLabel(GameManager.Instance.TotalPoints, GameManager.Instance.StageTargetPoints);
    }

}
