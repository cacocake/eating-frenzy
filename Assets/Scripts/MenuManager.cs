using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {

    [SerializeField] private GameObject _winScreen;
    [SerializeField] private GameObject _loseScreen;
    [SerializeField] private GameObject _pauseScreen;
    [SerializeField] private AudioSource _winSFX;
    [SerializeField] private AudioSource _loseSFX;
    
    public static MenuManager Instance { get; private set; }
    public static event Action OnGameStopped;
    public static event Action OnGameResumed;
    public static event Action OnVibrationSettingChanged;
    public static bool IsPaused = false;
    public static bool AreVibrationsEnabled = true;
    public static bool IsSoundMuted = false;

    private void Start() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Application.targetFrameRate = 60;
        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (scene.name == "MainMenu") {
            TryAddListenerToButtonObject("PlayButton", LoadPlayScene);
            TryAddListenerToButtonObject("ExitButton", ExitGame);
        } else {
            TryAddListenerToButtonObject("SettingsButton", ShowPauseMenu);
            TryAddListenerToButtonObject("SoundButton", ToggleSound);
        }
        
        if(_winScreen) {
            _winScreen.SetActive(false);
        }

        if(_winScreen) {
            _loseScreen.SetActive(false);
        }

        if(_pauseScreen) {
            _pauseScreen.SetActive(false);
        }
        ResumeGame();
    }

    private void TryAddListenerToButtonObject(string tag, Action listener) {
        GameObject buttonObject = GameObject.FindWithTag(tag);
        if(buttonObject == null) {
            Debug.LogError("Could not find object with tag: " + tag);
            return;
        }
        if(buttonObject.TryGetComponent<Button>(out var button)) {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => {
                listener.Invoke();
            });
        }
    }

    public void LoadPlayScene() {
        SceneManager.LoadScene("Forest");
    }

    public void LoadMainMenu() {
        SceneManager.LoadScene("MainMenu");
    }

    public void ExitGame() {
        Application.Quit();
    }

    public void ShowWinScreen() {
        if(_winScreen) {
            _winScreen.SetActive(true);
        }

        if(_winSFX) {
            _winSFX.Play();
        }

        OnGameStopped?.Invoke();
    }
    public void ShowLoseScreen() {
        if(_loseScreen) {
            _loseScreen.SetActive(true);
        }

        if(_loseSFX) {
            _loseSFX.Play();
        }

        OnGameStopped?.Invoke();
    }

    public void ShowPauseMenu() {
        if(_pauseScreen) {
            _pauseScreen.SetActive(true);
        }
        OnGameStopped?.Invoke();
        IsPaused = true;
        Time.timeScale = 0.0f;
    }

    public void ResumeGame() {
        if(_pauseScreen) {
            _pauseScreen.SetActive(false);
        }
        IsPaused = false;
        Time.timeScale = 1;
        OnGameResumed?.Invoke();
    }

    public bool IsInWinLoseState() {
        return (_winScreen && _winScreen.activeSelf) || 
               (_loseScreen && _loseScreen.activeSelf);
    }

    public void ToggleVibrations() {
        AreVibrationsEnabled = !AreVibrationsEnabled;
        if(AreVibrationsEnabled) {
            Utils.ExecuteHapticVibration(Utils.HapticType.ActivateVibrationSetting);
        }
        OnVibrationSettingChanged?.Invoke();
    }

    public void ToggleSound() {
        IsSoundMuted = !IsSoundMuted;
        AudioListener.pause = IsSoundMuted;
        AudioListener.volume = IsSoundMuted ? 0.0f : 0.75f;
    }
    
}
