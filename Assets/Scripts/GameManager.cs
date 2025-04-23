using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }
    [SerializeField] private PlayerHole _player;
    private ushort _totalPoints = 0;
    private ushort _baseTargetPoints = 10;
    private ushort _targetPoints = 10;
    private ushort _currentLevel = 1;
    private ushort _targetScalingPerLevel = 7;
    private void Awake() {
        Instance = this;
    }

    private void OnEnable() {
        Debug.Log($"Target Points: {_baseTargetPoints + ((_currentLevel - 1) * _targetScalingPerLevel)}");
        ConsumableObject.OnConsumableObjectSwallowed += HandleConsumableObjectSwallowed;
    }

    private void OnDisable() {
        ConsumableObject.OnConsumableObjectSwallowed -= HandleConsumableObjectSwallowed;
    }

    private void HandleConsumableObjectSwallowed(ConsumableObject consumableObject) {
        _totalPoints += consumableObject.Points;
        Debug.Log($"Total Points: {_totalPoints}");
        if(_totalPoints >= _targetPoints) {
            LevelUp();
        }
    }

    private void LevelUp() {
        while(_totalPoints >= _targetPoints) {
            _currentLevel++;
            _targetPoints = (ushort)(_targetPoints + _baseTargetPoints + ((_currentLevel - 1) * _targetScalingPerLevel));
        }
        
        _player.IncreaseSize(_currentLevel);
        Debug.Log($"Level Up! Current Level: {_currentLevel}");
        Debug.Log($"Current Level Target Points: {_baseTargetPoints + ((_currentLevel - 1) * _targetScalingPerLevel)}");
    }
}
