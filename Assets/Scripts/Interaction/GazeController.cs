/******************************************************************************
 * File: GazeInteractor.cs
 * Copyright (c) 2021 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
 *
 * Confidential and Proprietary - Qualcomm Technologies, Inc.
 *
 ******************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.OpenXR;
using Qualcomm.Snapdragon.Spaces;

public class GazeController : MonoBehaviour
{
    [Range(-1.0f, 1.0f)]
    public float VerticalBias = 0.2f;
    public float DefaultDistance = 1.0f;
    public Image ReticleOuterRing;
    public float GazeTimerDuration = 2.0f;

    public InputActionReference SelectAction;

    private IPointerClickHandler _activeClickHandler;
    private Camera _mainCamera;
    private float _gazeTimerCurrent;

    private SpacesHandManager _spacesHandManager;
    private float _currentHitDistance;
    private Transform _currentHitTransform;
    private bool _grabbing = false;
    private bool _singlePassRenderMode;

    private void Start()
    {
        _mainCamera = Camera.main;
        _singlePassRenderMode = OpenXRSettings.Instance.renderMode == OpenXRSettings.RenderMode.SinglePassInstanced;
        _spacesHandManager = FindObjectOfType<SpacesHandManager>();
    }

    private void OnEnable()
    {
        SelectAction.action.performed += RaycastClosestHit;
        SelectAction.action.canceled += CancelHoldingState;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnDisable()
    {
        SelectAction.action.performed -= RaycastClosestHit;
        SelectAction.action.canceled -= CancelHoldingState;
        Cursor.lockState = CursorLockMode.None;
    }

    private void Update()
    {
        UpdateGazeUI();
        UpdateHoldingPhysics();
    }

    private void RaycastClosestHit(InputAction.CallbackContext ctx)
    {
        _currentHitDistance = Mathf.Infinity;
        /* Raycast Physics */
        RaycastHit hit;
        if (Physics.Raycast(_mainCamera.transform.position, _mainCamera.transform.forward,
            out hit, Mathf.Infinity))
        {
            if (hit.transform.GetComponent<XRGrabInteractable>())
            {
                _currentHitDistance = hit.distance;
                _currentHitTransform = hit.transform;
                _grabbing = true;
            }
        }

        RaycastUI(out List<RaycastResult> results, out PointerEventData pointerEventData, out Vector2 screenPoint);
        if (results.Count > 0 && results[0].distance < _currentHitDistance)
        {
            /* Pick the first raycast result to set the pointer position. This is the closest hit to the camera. */
            if (_activeClickHandler != null)
            {
                _gazeTimerCurrent = GazeTimerDuration;
                ReticleOuterRing.fillAmount = 1.0f;
                _grabbing = false;
            }
        }
    }

    private void CancelHoldingState(InputAction.CallbackContext ctx)
    {
        _grabbing = false;
    }

    private void UpdateHoldingPhysics()
    {
        if (_grabbing)
        {
            _currentHitTransform.position = _mainCamera.transform.position + _mainCamera.transform.forward * _currentHitDistance;
            _currentHitTransform.rotation = _mainCamera.transform.rotation;
        }
    }

    private void UpdateGazeUI()
    {
        RaycastUI(out List<RaycastResult> results, out PointerEventData pointerEventData, out Vector2 screenPoint);
        if (results.Count > 0 && !_grabbing)
        {
            /* Pick the first raycast result to set the pointer position. This is the closest hit to the camera. */
            var result = results[0];
            SetPointerPosition(result.worldPosition, result.worldNormal);

            /* Check if there is a child click handler on the hit UI element. */
            IPointerClickHandler clickHandler = result.gameObject.GetComponentInParent<IPointerClickHandler>();
            if (clickHandler != null)
            {
                float gazeTimerDuration = GazeTimerDuration;
                bool continuousClick = false;




                ////HandGestureRules(_spacesHandManager.SpacesHandsChangedEventArgs, pointerEventData);
                //var GestureTypeLeft = Enum.GetName(typeof(SpacesHand.GestureType), _spacesHandManager.LeftHand.CurrentGesture.Type);
                //var GestureTypeRight = Enum.GetName(typeof(SpacesHand.GestureType), _spacesHandManager.RightHand.CurrentGesture.Type);

                //if (GestureTypeLeft == "PINCH" || GestureTypeRight == "PINCH")
                //{
                //    _activeClickHandler.OnPointerClick(pointerEventData);
                //}

                /* Check if there is a GazeHoverOverride */
                var gazeHoverOverride = result.gameObject.GetComponentInParent<GazeHoverOverride>();
                if (gazeHoverOverride != null)
                {
                    gazeTimerDuration = gazeHoverOverride.GazeTimerDuration;
                    continuousClick = gazeHoverOverride.ContinuousClick;
                }

                if (_activeClickHandler != clickHandler)
                {
                    _activeClickHandler = clickHandler;
                    _gazeTimerCurrent = 0f;
                }
                else
                {
                    if (_gazeTimerCurrent <= gazeTimerDuration)
                    {
                        _gazeTimerCurrent += Time.deltaTime;
                        /* Increase the fill amount by the normalized value (0.0 to 1.0) */
                        ReticleOuterRing.fillAmount = _gazeTimerCurrent / gazeTimerDuration;
                    }
                    if (ReticleOuterRing.fillAmount >= 1f)
                    {
                        _activeClickHandler.OnPointerClick(pointerEventData);
                        ReticleOuterRing.fillAmount = 0f;
                        if (continuousClick)
                        {
                            _gazeTimerCurrent = 0f;
                            ReticleOuterRing.fillAmount = 1f;
                        }
                    }
                }
                return;
            }
        }

        ResetPointerPosition(screenPoint);
    }

    private void RaycastUI(out List<RaycastResult> results, out PointerEventData pointerEventData, out Vector2 screenPoint)
    {
        /* The horizontal half on the device is a quarter since the rendered output is in stereo and
            * we only take the left eye's center for the sake of simplicity.
            */
        float horizontalHalf = !Application.isEditor && _singlePassRenderMode ? 0.25f : 0.5f;
        float verticalHalf = 0.5f + 0.5f * VerticalBias;
        screenPoint = new Vector2(Screen.width * horizontalHalf, Screen.height * verticalHalf);

        /* Send raycast from the main camera position to test against UI. */
        pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = screenPoint;

        results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, results);
    }

    private void SetPointerPosition(Vector3 position, Vector3 normal)
    {
        /* Additionally offset the position on Z to avoid z-fighting/clipping */
        transform.position = position + normal * 0.1f;
        transform.rotation = Quaternion.LookRotation(-normal);
    }

    private void ResetPointerPosition(Vector3 position)
    {
        /* Keep the pointer at the default distance if no element with a click handler was raycasted. */
        var cameraRay = _mainCamera.ScreenPointToRay(position);
        Vector3 headPosition = cameraRay.origin;
        Vector3 gazeDirection = cameraRay.direction;
        SetPointerPosition(headPosition + gazeDirection * DefaultDistance, -gazeDirection);
        _activeClickHandler = null;
        ReticleOuterRing.fillAmount = 0f;
    }
}

