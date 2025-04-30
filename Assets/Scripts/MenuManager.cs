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
    }

    private void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (scene.name == "MainMenu") {
            GameObject.FindWithTag("PlayButton").GetComponent<Button>().onClick.AddListener(() => {
                LoadPlayScene();
            });
            GameObject.FindWithTag("ExitButton").GetComponent<Button>().onClick.AddListener(() => {
                ExitGame();
            });
        } else {
            GameObject.FindWithTag("SettingsButton").GetComponent<Button>().onClick.AddListener(() => {
                ShowPauseMenu();
            });
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
        _winScreen.SetActive(true);
        _winSFX.Play();
        OnGameStopped?.Invoke();
    }
    public void ShowLoseScreen() {
        _loseScreen.SetActive(true);
        _loseSFX.Play();
        OnGameStopped?.Invoke();
    }

    public void ShowPauseMenu() {
        _pauseScreen.SetActive(true);
        OnGameStopped?.Invoke();
        IsPaused = true;
        Time.timeScale = 0.0f;
    }

    public void ResumeGame() {
        _pauseScreen.SetActive(false);
        IsPaused = false;
        Time.timeScale = 1;
        OnGameResumed?.Invoke();
    }

    public bool IsInWinLoseState() {
        return _winScreen.activeSelf || _loseScreen.activeSelf;
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
