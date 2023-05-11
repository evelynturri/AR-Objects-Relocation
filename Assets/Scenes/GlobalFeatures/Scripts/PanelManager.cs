using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Qualcomm.Snapdragon.Spaces;
using QCHT.Interactions.Hands;
using UnityEngine.SceneManagement;

// Class to manage the UI Panels for each scene
public class PanelManager : MonoBehaviour
{
    public GameObject StartUI;
    public GameObject AdminUI;
    public GameObject GuestUI;
    public GameObject CreateUI;
    public GameObject LoadUI;
    [SerializeField]
    private SceneSettings startSceneSettings;
    [SerializeField]
    private SceneSettings adminSceneSettings;
    [SerializeField]
    private SceneSettings guestSceneSettings;
    [SerializeField]
    private SceneSettings createSceneSettings;
    [SerializeField]
    private SceneSettings loadSceneSettings;

    private GameObject _oldPanel;
    private GameObject _currentPanel;
    private Scene _currentScene;

    private bool activate = true;
    
    void Start()
    {
        _currentScene.name = startSceneSettings.SceneName;
        _currentPanel = StartUI;
        _currentPanel.SetActive(true);
        if (_currentScene.name == startSceneSettings.SceneName)
        {
            AdminUI.SetActive(false);
            GuestUI.SetActive(false);
            CreateUI.SetActive(false);
            LoadUI.SetActive(false);
        } else if (_currentScene.name == guestSceneSettings.SceneName)
        {
            StartUI.SetActive(false);
            AdminUI.SetActive(false);
            CreateUI.SetActive(false);
            LoadUI.SetActive(false);
        } else if (_currentScene.name == adminSceneSettings.SceneName)
        {
            StartUI.SetActive(false);
            GuestUI.SetActive(false);
            CreateUI.SetActive(false);
            LoadUI.SetActive(false);
        } else if (_currentScene.name == createSceneSettings.SceneName)
        {
            StartUI.SetActive(false);
            AdminUI.SetActive(false);
            GuestUI.SetActive(false);
            LoadUI.SetActive(false);
        } else if (_currentScene.name == loadSceneSettings.SceneName)
        {
            StartUI.SetActive(false);
            AdminUI.SetActive(false);
            GuestUI.SetActive(false);
            CreateUI.SetActive(false);
        }

      _oldPanel = _currentPanel;
    }

    
    void Update()
    {
        getPanel();
        if (_oldPanel.name != _currentPanel.name)
        {
            _oldPanel.SetActive(false);
            _currentPanel.SetActive(true);
            _oldPanel = _currentPanel;
        }

    }

    public void EnableDisable()
    {
        getPanel();

        if (activate==true)
        {
            _currentPanel.SetActive(false);
            activate = false;
        } else
        {
            _currentPanel.SetActive(true);
            activate = true;
        }
    }

    void getPanel()
    {
        _currentScene = SceneManager.GetActiveScene();
        if (_currentScene.name == adminSceneSettings.SceneName)
        {
            _currentPanel = AdminUI;
        }

        if (_currentScene.name == guestSceneSettings.SceneName)
        {
            _currentPanel = GuestUI;
        }

        if (_currentScene.name == createSceneSettings.SceneName)
        {
            _currentPanel = CreateUI;
        }

        if (_currentScene.name == startSceneSettings.SceneName)
        {
            _currentPanel = StartUI;
        }
        if (_currentScene.name == loadSceneSettings.SceneName)
        {
            _currentPanel = LoadUI;
        }

    }
}
