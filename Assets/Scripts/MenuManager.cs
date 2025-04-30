using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {

    [SerializeField] private GameObject _winScreen;
    [SerializeField] private GameObject _loseScreen;
    [SerializeField] private GameObject _pauseScreen;
    
    public static MenuManager Instance { get; private set; }
    public static event Action OnGameStopped;
    public static event Action OnGameResumed;
    public static bool isPaused = false;

    private void Start() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
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
        _winScreen.SetActive(false);
        _loseScreen.SetActive(false);
        _pauseScreen.SetActive(false);
        isPaused = false;
        Time.timeScale = 1.0f;

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
        OnGameStopped?.Invoke();
    }
    public void ShowLoseScreen() {
        _loseScreen.SetActive(true);
        OnGameStopped?.Invoke();
    }

    public void ShowPauseMenu() {
        _pauseScreen.SetActive(true);
        OnGameStopped?.Invoke();
        isPaused = true;
        Time.timeScale = 0.0f;
    }

    public void HidePauseMenu() {
        _pauseScreen.SetActive(false);
        isPaused = false;
        Time.timeScale = 1;
        OnGameResumed?.Invoke();
    }

    public bool IsInWinLoseState() {
        return _winScreen.activeSelf || _loseScreen.activeSelf;
    }
    
}
