using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {
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
}
