using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {

    [SerializeField] private GameObject _winScreen;
    [SerializeField] private GameObject _loseScreen;
    public static MenuManager Instance { get; private set; }
    public static event Action OnEndGameReached;

    private void Start() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable(){
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable(){
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        _winScreen.SetActive(false);
        _loseScreen.SetActive(false);

        if (scene.name == "MainMenu") {
            GameObject.FindWithTag("PlayButton").GetComponent<Button>().onClick.AddListener(() => {
                LoadPlayScene();
            });
            GameObject.FindWithTag("ExitButton").GetComponent<Button>().onClick.AddListener(() => {
                ExitGame();
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
        OnEndGameReached?.Invoke();
    }
    public void ShowLoseScreen() {
        _loseScreen.SetActive(true);
        OnEndGameReached?.Invoke();
    }

    public bool IsInWinLoseState() {
        return _winScreen.activeSelf || _loseScreen.activeSelf;
    }
}
