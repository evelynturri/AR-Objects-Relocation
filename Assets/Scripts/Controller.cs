/******************************************************************************
 * File: SampleController.cs
 * Copyright (c) 2021-2022 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
 *
 * Confidential and Proprietary - Qualcomm Technologies, Inc.
 *
 ******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.OpenXR;
using Qualcomm.Snapdragon.Spaces;

public enum ControllerType
{
    GazePointer = 0,
    DevicePointer = 1
}

public class Controller : MonoBehaviour
{
    
    public GameObject GazePointer;
    public GameObject DevicePointer;
    public InputActionReference SwitchInputAction;

    protected virtual bool ResetSessionOriginOnStart => true;

    private bool _isSessionOriginMoved = false;
    private Transform _camera;
    private const string _controllerTypePrefsKey = "Qualcomm.Snapdragon.Spaces.Samples.Prefs.ControllerType";


    public virtual void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        _camera = Camera.main.transform;

        var baseRuntimeFeature = OpenXRSettings.Instance.GetFeature<BaseRuntimeFeature>();
        if (!baseRuntimeFeature)
        {
            Debug.LogWarning("Base Runtime Feature isn't available.");
            return;
        }

        //var componentVersions = baseRuntimeFeature.ComponentVersions;

        int controllerType = PlayerPrefs.GetInt(_controllerTypePrefsKey, 0);
        //GazePointer.SetActive(controllerType == (int)ControllerType.GazePointer);
        //DevicePointer.SetActive(controllerType == (int)ControllerType.DevicePointer);
    }

    public virtual void Update()
    {
        if (ResetSessionOriginOnStart && !_isSessionOriginMoved && _camera.position != Vector3.zero)
        {
            OffsetSessionOrigin();
            _isSessionOriginMoved = true;
        }
    }

    public void Quit()
    {
        Application.Quit();
    }

    protected void OffsetSessionOrigin()
    {
        ARSessionOrigin sessionOrigin = FindObjectOfType<ARSessionOrigin>();
        sessionOrigin.transform.Rotate(0.0f, -_camera.rotation.eulerAngles.y, 0.0f, Space.World);
        sessionOrigin.transform.position = -_camera.position;
    }

    public virtual void OnEnable()
    {
        SwitchInputAction.action.performed += OnSwitchInput;
    }

    public virtual void OnDisable()
    {
        SwitchInputAction.action.performed -= OnSwitchInput;
    }

    private void OnSwitchInput(InputAction.CallbackContext ctx)
    {
        if (ctx.interaction is TapInteraction)
        {
            GazePointer.SetActive(!GazePointer.activeSelf);
            DevicePointer.SetActive(!DevicePointer.activeSelf);
        }

        if (ctx.interaction is HoldInteraction)
        {
            Quit();
        }

        int controllerType = GazePointer.activeSelf ? (int)ControllerType.GazePointer :
                                DevicePointer.activeSelf ? (int)ControllerType.DevicePointer : 0;

        PlayerPrefs.SetInt(_controllerTypePrefsKey, controllerType);
    }
}