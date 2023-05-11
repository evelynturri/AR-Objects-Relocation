/******************************************************************************
 * File: XRControllerInputAnimation.cs
 * Copyright (c) 2022 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
 *
 ******************************************************************************/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Qualcomm.Snapdragon.Spaces.Samples 
{
    public class XRControllerInputAnimation : MonoBehaviour 
    {
        public enum XRController {
            LeftController,
            RightController
        }
        public XRController Controller;
        
        private List<InputDevice> _leftDevices;
        private List<InputDevice> _rightDevices;
        private SkinnedMeshRenderer _controllerSkinnedMeshRenderer;
        private InputDataBlendshapes _inputDataBlendshapes = new InputDataBlendshapes();
        private const float _blendshapeMultiplyFactor = 100f;

        private void Awake() {
            _controllerSkinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
            _leftDevices = new List<InputDevice>();
            _rightDevices = new List<InputDevice>();
        }

        private void OnEnable() {
            List<InputDevice> devices = new List<InputDevice>();
            InputDevices.GetDevices(devices);
            foreach (var device in devices) {
                DeviceConnected(device);
            }
            InputDevices.deviceConnected += DeviceConnected;
            InputDevices.deviceDisconnected += DeviceDisconnected;
        }

        private void OnDisable() {
            InputDevices.deviceConnected -= DeviceConnected;
            InputDevices.deviceDisconnected -= DeviceDisconnected;
            _leftDevices.Clear();
            _rightDevices.Clear();
        }

        private void DeviceConnected(InputDevice device) {
            if ((device.characteristics & InputDeviceCharacteristics.Left) != 0) {
                _leftDevices.Add(device);
            }
            
            if ((device.characteristics & InputDeviceCharacteristics.Right) != 0) {
                _rightDevices.Add(device);
            }
        }

        private void DeviceDisconnected(InputDevice device) {
            if (_leftDevices.Contains(device)) {
                _leftDevices.Remove(device);
            }
            
            if (_rightDevices.Contains(device)) {
                _rightDevices.Remove(device);
            }
        }

        private void Update() {
            switch (Controller) {
                case (XRController.LeftController): {
                    UpdateAnimations(_leftDevices);
                    break;
                }
                case (XRController.RightController): {
                    UpdateAnimations(_rightDevices);
                    break;
                }
            }
        }

        private void UpdateAnimations(List<InputDevice> devices) {
            foreach (var device in devices) {
                if (device.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue)) {
                    SetBlendshapeValue(_inputDataBlendshapes.Trigger, triggerValue);
                }

                if (device.TryGetFeatureValue(CommonUsages.grip, out float gripValue)) {
                    SetBlendshapeValue(_inputDataBlendshapes.Grip, gripValue);
                }

                if (device.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 joystickValue)) {
                    SetBlendshapeValue(_inputDataBlendshapes.JoystickX, joystickValue.x);
                    SetBlendshapeValue(_inputDataBlendshapes.JoystickY, joystickValue.y);
                }

                if (device.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryButton)) {
                    if (primaryButton) {
                        SetBlendshapeValue(_inputDataBlendshapes.PrimaryButtonPress, 1);
                    }
                    else {
                        ResetBlendshapeValue(_inputDataBlendshapes.PrimaryButtonPress);
                    }
                }

                if (device.TryGetFeatureValue(CommonUsages.secondaryButton, out bool secondaryButton)) {
                    if (secondaryButton) {
                        SetBlendshapeValue(_inputDataBlendshapes.SecondaryButtonPress, 1);
                    }
                    else {
                        ResetBlendshapeValue(_inputDataBlendshapes.SecondaryButtonPress);
                    }
                }
            }
        }

        private void SetBlendshapeValue(int blendshapeButton, float buttonValue) {
            _controllerSkinnedMeshRenderer.SetBlendShapeWeight(blendshapeButton, buttonValue * _blendshapeMultiplyFactor);
        }

        private void ResetBlendshapeValue(int blendshapeButton) {
            SetBlendshapeValue(blendshapeButton, 0f);
        }
    }
}