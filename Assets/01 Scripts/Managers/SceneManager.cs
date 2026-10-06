using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : ManagerBase {
    public SceneBase sceneScript { get; private set; }

    public void Init() {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void Update() {

    }

    public void LoadScene(string sceneToLoad) {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToLoad);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode) {
        sceneScript = Object.FindAnyObjectByType<SceneBase>();
    }
}
