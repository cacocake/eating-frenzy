using System;
using UnityEngine;

public class GameManager : MonoBehaviour {
    
    [SerializeField] private PlayerHole _player;
    [SerializeField] private ushort _stageTargetPoints;
    [SerializeField] private float _stageTimeLimit;
    [SerializeField] private Canvas _levelGUI;
    [SerializeField] private Canvas _floatingJoystickGUI;
    [SerializeField] private ushort _targetScalingPerLevel = 5;
    
    private ushort _baseTargetPoints = 10;
    private ushort _previousLevelTargetTotalPoints = 0;
    private ushort _nextLevelTargetTotalPoints = 10;
    
    public ushort StageTargetPoints => _stageTargetPoints;
    public float StageTimeLimit => _stageTimeLimit;

    public ushort CurrentLevel { get; private set; } = 1;
    public ushort TotalPoints {get; private set; } = 0;
    public static GameManager Instance { get; private set; }
    public ushort CurrentLevelTargetPoints { get; private set; } = 10;
    public ushort CurrentLevelPoints { get; private set; } = 0;
    
    public static event Action OnLevelUp;
    public static event Action OnPointsChanged;
    
    private void Awake() {
        Instance = this;
    }

    private void Update() {
        if (MenuManager.Instance.IsInWinLoseState()) {
            return;
        }
        
        if(_stageTimeLimit <= 1) {
            MenuManager.Instance.ShowLoseScreen();
            return;
        }

        _stageTimeLimit -= Time.deltaTime;
    }

    private void OnEnable() {
        ConsumableObject.OnConsumableObjectSwallowed += HandleConsumableObjectSwallowed;
        MenuManager.OnGameStopped += HandleOnGameStopped;
        MenuManager.OnGameResumed += HandleOnGameResumed;
    }

    private void HandleOnGameStopped(){
        _levelGUI.gameObject.SetActive(false);
        _floatingJoystickGUI.gameObject.SetActive(false);
    }

    private void HandleOnGameResumed(){
        _levelGUI.gameObject.SetActive(true);
        _floatingJoystickGUI.gameObject.SetActive(true);
    }

    private void OnDisable() {
        ConsumableObject.OnConsumableObjectSwallowed -= HandleConsumableObjectSwallowed;
        MenuManager.OnGameStopped -= HandleOnGameStopped;
        MenuManager.OnGameResumed -= HandleOnGameResumed;
    }

    private void HandleConsumableObjectSwallowed(ConsumableObject consumableObject) {
        if(MenuManager.Instance.IsInWinLoseState()) {
            return;
        }

        TotalPoints = (ushort)Math.Min(TotalPoints + consumableObject.Points, _stageTargetPoints);
        if(TotalPoints >= _stageTargetPoints) {
            MenuManager.Instance.ShowWinScreen();
            return;
        }
        
        if(TotalPoints >= _nextLevelTargetTotalPoints) {
            LevelUp();
            if(MenuManager.AreVibrationsEnabled) {
                Utils.ExecuteHapticVibration(Utils.HapticType.LevelUp);
            }
        } else {
            if(MenuManager.AreVibrationsEnabled) {
                Utils.ExecuteHapticVibration(Utils.HapticType.ConsumedObject);
            }
        }
        CurrentLevelPoints = (ushort)(TotalPoints - _previousLevelTargetTotalPoints);
        OnPointsChanged?.Invoke();
    }

    private void LevelUp() {
        while(TotalPoints >= _nextLevelTargetTotalPoints) {
            CurrentLevel++;
            _previousLevelTargetTotalPoints = _nextLevelTargetTotalPoints;
            _nextLevelTargetTotalPoints = (ushort)(_nextLevelTargetTotalPoints + _baseTargetPoints + ((CurrentLevel - 1) * _targetScalingPerLevel));
            CurrentLevelTargetPoints = (ushort)(_baseTargetPoints + ((CurrentLevel - 1) * _targetScalingPerLevel));
        }
        Utils.ExecuteHapticVibration(Utils.HapticType.LevelUp);
        OnLevelUp?.Invoke();
    }
}
