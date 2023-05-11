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
using static Anchor;
using static Hologram;

public class AnchorPointsGlobal : MonoBehaviour
{
    protected ARAnchorManager AnchorManager;
    protected SpacesAnchorStore _anchorStore;
    protected SpacesHandManager _handManager;

    private bool _saveAnchorsToStore = true;
    public Toggle SaveNewAnchorsToggle;
    public Text NumberOfAnchorsStoredText;
    public AnchorsSettings _AnchorsSettings;
    private int _saveAnchorsToStoreInt;

    private Transform _cameraTransform;

    private List<GameObject> _anchorGizmos;
    private List<GameObject> _sessionGizmos;

    public static List<string> _anchorsNames = new List<string>();
    public static List<Anchor> _anchors = new List<Anchor>();

    void Awake()
    {
        AnchorManager = FindObjectOfType<ARAnchorManager>();
        _anchorStore = FindObjectOfType<SpacesAnchorStore>();
        _handManager = FindObjectOfType<SpacesHandManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Set the camera transform
        _cameraTransform = Camera.main.transform;

        // Update the number of anchors
        NumberOfAnchorsStoredText.text = _anchorStore.GetSavedAnchorNames().Length.ToString();
        SaveNewAnchorsToggle.isOn = _saveAnchorsToStore;

        // Read JSON and set global variables
        var names = _anchorStore.GetSavedAnchorNames();

        foreach (var anchorName in names)
        {
            _anchorsNames.Add(anchorName);
            var jsonAnchor = PlayerPrefs.GetString(anchorName);
            var newAnchor = JsonUtility.FromJson<Anchor>(jsonAnchor);
            CheckObject(newAnchor);
            _anchors.Add(newAnchor);
        }
    }

    void OnApplicationQuit()
    {
        // Overwrite JSON with the new variables
        PlayerPrefs.SetString("AnchorsNames", JsonUtility.ToJson(_anchorsNames));
        foreach (var anchor in _anchors)
        {
            anchor.Tracked = false;
            PlayerPrefs.SetString(anchor.Name, JsonUtility.ToJson(anchor));
        }
    }


    void Update()
    {
        // Keep updated the number of anchors
        if (_anchorStore.GetSavedAnchorNames().Length.ToString() != NumberOfAnchorsStoredText.text)
        {
            NumberOfAnchorsStoredText.text = _anchorStore.GetSavedAnchorNames().Length.ToString();
        }
        NumberOfAnchorsStoredText.text = _anchorStore.GetSavedAnchorNames().Length.ToString();
    }


    // To clean the AnchorStore
    public void ClearAnchorStore()
    {
        _anchorStore.ClearStore();
        NumberOfAnchorsStoredText.text = _anchorStore.GetSavedAnchorNames().Length.ToString();
    }

    // Function to set the right prefab for each anchor after read the JSON
    public void CheckObject(Anchor anchor)
    {
        foreach (var hologram in anchor.Holograms)
        {
            for (int i = 0; i < _AnchorsSettings.Objects.Length; i++)
            {
                if (hologram.Name.Equals(_AnchorsSettings.Objects[i].name))
                {
                    hologram.HologramType = _AnchorsSettings.Objects[i];
                }
            }
        }   
    }
}
