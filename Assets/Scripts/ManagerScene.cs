using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using QCHT.Interactions.Hands;
using UnityEngine.UI;
using static AnchorPointsGlobal;

public class ManagerScene : MonoBehaviour
{

    [SerializeField]
    private SceneSettings startSceneSettings;

    [SerializeField]
    private SceneSettings adminSceneSettings;

    [SerializeField]
    private SceneSettings createSceneSettings;

    [SerializeField]
    private SceneSettings guestSceneSettings;

    [SerializeField]
    private SceneSettings loadSceneSettings;

    [SerializeField]
    private Button backButton;

    private SceneSettings _sceneToLoadSetting;
    private SceneSettings _currentSceneSetting;
    private SceneSettings _oldSceneSettings;
    private Scene _currentScene;
    private Scene _sceneToLoad;
    
    public void Start()
    {
        if (startSceneSettings)
        {
            LoadingScene(startSceneSettings);
            backButton.interactable = false;
        }
            
    }

    IEnumerator LoadAsyncScene(string SceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    public void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void BackScene()
    {
        if (_currentSceneSetting.SceneName == loadSceneSettings.SceneName)
        {
            LoadingScene(_oldSceneSettings);
        } else if (_currentSceneSetting.SceneName == adminSceneSettings.SceneName)
        {
            LoadingScene(startSceneSettings);
        } else if (_currentSceneSetting.SceneName == createSceneSettings.SceneName)
        {
            LoadingScene(adminSceneSettings);
        } else if (_currentSceneSetting.SceneName == guestSceneSettings.SceneName)
        {
            LoadingScene(startSceneSettings);
        }
    }

    #region Scene Loading

    /// <summary>
    /// Loads a scene and unload the current scene if exists.
    /// </summary>
    public void LoadingScene(SceneSettings sceneToLoadSettings)
    {
        if (_sceneToLoadSetting || sceneToLoadSettings.name.Equals(_currentScene.name))
            return;
        _oldSceneSettings = _currentSceneSetting;
        // Unload current scene if exists
        if (_currentScene.IsValid() && _currentScene.isLoaded)
        {
            SceneManager.UnloadSceneAsync(_currentScene);
            _currentSceneSetting = null;
        }

        // Load the new sample scene by name
        _sceneToLoadSetting = sceneToLoadSettings;
        _currentScene = SceneManager.GetActiveScene();
        if (_currentScene.name != "Global" && _currentScene.name != null)
        {
            SceneManager.UnloadSceneAsync(_currentScene);
            _currentSceneSetting = null;
        }

        // Set to false the tracked of the anchor when loading another scene
        foreach (var a in AnchorPointsGlobal._anchors)
        {
            a.Tracked = false;
        }

        SceneManager.LoadScene(sceneToLoadSettings.SceneName, LoadSceneMode.Additive);

    }

    private void OnSceneLoaded(Scene sceneToLoad, LoadSceneMode sceneMode)
    {
        if (!_sceneToLoadSetting || sceneToLoad.name != _sceneToLoadSetting.SceneName)
            return;

        _currentScene = sceneToLoad;
        _currentSceneSetting = _sceneToLoadSetting;
        _sceneToLoadSetting = null;

        if (_currentSceneSetting.SceneName == startSceneSettings.SceneName)
        {
            backButton.interactable = false;
        } else
        {
            backButton.interactable = true;
        }
        // Sets the current sample scene as active once loaded
        SceneManager.SetActiveScene(_currentScene);
    }

    #endregion

}

