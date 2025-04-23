using System;
using UnityEngine;

public class GameManager : MonoBehaviour {
    [SerializeField] private PlayerHole _player;
    [SerializeField] private ushort _stageTargetPoints;
    private ushort _baseTargetPoints = 10;
    private ushort _previousLevelTargetTotalPoints = 0;
    private ushort _nextLevelTargetTotalPoints = 10;
    private ushort _currentLevel = 1;
    private ushort _targetScalingPerLevel = 7;
    public ushort StageTargetPoints => _stageTargetPoints;
    public ushort TotalPoints {get; private set; } = 0;
    public static GameManager Instance { get; private set; }
    public ushort CurrentLevelTargetPoints { get; private set; } = 10;
    public ushort CurrentLevelPoints { get; private set; } = 0;
    private void Awake() {
        Instance = this;
    }

    private void OnEnable() {
        ConsumableObject.OnConsumableObjectSwallowed += HandleConsumableObjectSwallowed;
    }

    private void OnDisable() {
        ConsumableObject.OnConsumableObjectSwallowed -= HandleConsumableObjectSwallowed;
    }

    private void HandleConsumableObjectSwallowed(ConsumableObject consumableObject) {
        TotalPoints += consumableObject.Points;
        if(TotalPoints >= _stageTargetPoints) {
            Debug.Log("You Win!");
            return;
        }
        
        if(TotalPoints >= _nextLevelTargetTotalPoints) {
            LevelUp();
        }
        CurrentLevelPoints = (ushort)(TotalPoints - _previousLevelTargetTotalPoints);
    }

    private void LevelUp() {
        while(TotalPoints >= _nextLevelTargetTotalPoints) {
            _currentLevel++;
            _previousLevelTargetTotalPoints = _nextLevelTargetTotalPoints;
            _nextLevelTargetTotalPoints = (ushort)(_nextLevelTargetTotalPoints + _baseTargetPoints + ((_currentLevel - 1) * _targetScalingPerLevel));
            CurrentLevelTargetPoints = (ushort)(_baseTargetPoints + ((_currentLevel - 1) * _targetScalingPerLevel));
        }
        
        _player.IncreaseSize(_currentLevel);
    }
}
