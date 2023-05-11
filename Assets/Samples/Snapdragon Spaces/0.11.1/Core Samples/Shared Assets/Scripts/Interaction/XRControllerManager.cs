/******************************************************************************
 * File: XRControllerManager.cs
 * Copyright (c) 2022 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
 *
 ******************************************************************************/
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.OpenXR.Input;

namespace Qualcomm.Snapdragon.Spaces.Samples
{
    public class XRControllerManager : MonoBehaviour
    {
        public GameObject HostController;
        public GameObject XRControllers;

        public InputActionReference HapticInputAction;

        private XRControllerProfile _xrControllerProfile;

        public void ActivateController(XRControllerProfile xrControllerProfile) {
            _xrControllerProfile = xrControllerProfile;
            switch (xrControllerProfile) {
                case XRControllerProfile.XRControllers: {
                    HostController.SetActive(false);
                    XRControllers.SetActive(true);
                    break;
                }
                default: {
                    XRControllers.SetActive(false);
                    HostController.SetActive(true);
                    break;
                }
            }
        }

        public void ResetPositionAndRotation(Transform newTransform) {
            transform.rotation = newTransform.rotation;
            switch (_xrControllerProfile) {
                case XRControllerProfile.XRControllers: {
                    transform.position = newTransform.position;
                    break;
                }
            }
        }

        public void SendHapticImpulse(float amplitude = 0.5f, float frequency = 60f, float duration = 0.1f) {
            OpenXRInput.SendHapticImpulse(HapticInputAction, amplitude, frequency, duration);
        }
    }
}