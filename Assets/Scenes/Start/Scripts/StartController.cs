/******************************************************************************
 * File: MainMenuSampleController.cs
 * Copyright (c) 2022 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
 *
 * Confidential and Proprietary - Qualcomm Technologies, Inc.
 *
 ******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Qualcomm.Snapdragon.Spaces;
using QCHT.Interactions.Hands;
using QCHT.Core;
using UnityEngine.SceneManagement;


public class StartController : Controller
{
    public ARAnchorManager AnchorManager;
    public QCHTManager _QCHTManager;
    //public GameObject ContentGameObject;
    //public GameObject ComponentVersionsGameObject;
    //public Transform ComponentVersionContent;
    //public GameObject ComponentVersionPrefab;

    //public ScrollRect ScrollRect;
    //public Scrollbar VerticalScrollbar;
    //public GameObject GazeScrollButtons;
    //public InputActionReference TouchpadInputAction;
    [SerializeField]
    private HandPresenter leftHandPresenter;

    [SerializeField]
    private HandPresenter rightHandPresenter;

    private SpacesAnchorStore _anchorStore;

    public override void Start()
    {
        base.Start();

        //OnInputSwitch(new InputAction.CallbackContext());

        _anchorStore = FindObjectOfType<SpacesAnchorStore>();
        //for (int i = 0; i < componentVersions.Count; i++)
        //{
        //    var componentVersionObject = Instantiate(ComponentVersionPrefab, ComponentVersionContent);

        //    var componentVersionDisplay = componentVersionObject.GetComponent<ComponentVersionDisplay>();
        //    componentVersionDisplay.ComponentName.text = componentVersions[i].ComponentName;
        //    componentVersionDisplay.BuildIdentifier.text = componentVersions[i].BuildIdentifier;
        //    componentVersionDisplay.VersionIdentifier.text = componentVersions[i].VersionIdentifier;
        //    componentVersionDisplay.BuildDateTime.text = componentVersions[i].BuildDateTime;
        //}
    }

    //public void OnDestroy()
    //{
    //    //_QCHTManager.OnDestroy();
    //    Destroy(FindObjectOfType<QCHTManager>());
    //    Destroy(FindObjectOfType<HandPresenter>());
    //}
    //public override void OnEnable()
    //{
    //    base.OnEnable();
    //    SwitchInputAction.action.performed += OnInputSwitch;
    //    TouchpadInputAction.action.performed += OnTouchpadInput;
    //}

    //public override void OnDisable()
    //{
    //    base.OnDisable();
    //    SwitchInputAction.action.performed -= OnInputSwitch;
    //    TouchpadInputAction.action.performed -= OnTouchpadInput;
    //}

    //private void OnTouchpadInput(InputAction.CallbackContext context)
    //{
    //    var touchpadValue = context.ReadValue<Vector2>();

    //    if (touchpadValue.y > 0f)
    //    {
    //        OnVerticalScrollViewChanged(0.44f);
    //    }
    //    else
    //    {
    //        OnVerticalScrollViewChanged(-0.44f);
    //    }
    //}

    //public void OnInfoButtonPress()
    //{
    //    ContentGameObject.SetActive(!ContentGameObject.activeSelf);
    //    ComponentVersionsGameObject.SetActive(!ComponentVersionsGameObject.activeSelf);
    //}

    //public void OnVerticalScrollViewChanged(float value)
    //{
    //    ScrollRect.verticalNormalizedPosition = Mathf.Clamp01(ScrollRect.verticalNormalizedPosition + value * Time.deltaTime);
    //    VerticalScrollbar.value = ScrollRect.verticalNormalizedPosition;
    //}

    //private void OnInputSwitch(InputAction.CallbackContext ctx)
    //{
    //    if (GazePointer.activeSelf)
    //    {
    //        ScrollRect.verticalScrollbar = null;
    //        VerticalScrollbar.gameObject.SetActive(false);

    //        GazeScrollButtons.SetActive(true);
    //    }
    //    else if (DevicePointer.activeSelf)
    //    {
    //        ScrollRect.verticalScrollbar = VerticalScrollbar;
    //        VerticalScrollbar.gameObject.SetActive(true);

    //        GazeScrollButtons.SetActive(false);
    //    }
    //}
}

