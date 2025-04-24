using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {

    [SerializeField] private GameObject _winScreen;
    [SerializeField] private GameObject _loseScreen;
    public static MenuManager Instance { get; private set; }
    private void Start() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadPlayScene() {
        SceneManager.LoadScene("Forest");
    }

    public void ExitGame() {
        Application.Quit();
    }

    public void ShowWinScreen() {
        _winScreen.SetActive(true);
    }
    public void ShowLoseScreen() {
        _loseScreen.SetActive(true);
    }

    public bool IsInWinLoseState() {
        return _winScreen.activeSelf || _loseScreen.activeSelf;
    }
}
