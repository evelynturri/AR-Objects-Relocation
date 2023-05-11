/******************************************************************************
 * File: EditorCameraController.cs
 * Copyright (c) 2021-2023 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
 *
 ******************************************************************************/

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;

namespace Qualcomm.Snapdragon.Spaces.Samples
{
    public class EditorCameraController : MonoBehaviour
    {
#if UNITY_EDITOR
        public GameObject GazePointer;
        public Transform DeviceController;
        public InputActionAsset InputActionControls;

        public float MoveSpeed = 2.0f;
        public float RotationSensitivity = 5.0f;

        private InputAction _cameraTranslationAction;
        private InputAction _cameraRotationAction;

        private Vector3 _moveDirection;
        private Vector2 _mouseDelta = Vector2.zero;
        private ARSessionOrigin _arSessionOrigin;
        private Transform _arCameraTransform;
        private Camera _arCamera;

        private bool IsGazeControl => GazePointer && GazePointer.activeSelf;

        private void Awake() {
            _arSessionOrigin ??= FindObjectOfType<ARSessionOrigin>();
            _arCamera = _arSessionOrigin.camera;
            _arCameraTransform = _arCamera.transform;

            /* Setup the input actions for translation and rotation */
            if (InputActionControls != null) {
                var actionMap = InputActionControls.FindActionMap("EditorCamera");

                if (actionMap != null) {
                    _cameraTranslationAction = actionMap.FindAction("Translate");
                    _cameraRotationAction = actionMap.FindAction("Rotate");
                }
            }

            Debug.Log("Press Key " + Keyboard.current[Key.LeftShift].name +
                      " to switch between gaze input and the simulated device controller.");
        }

        private void OnEnable() {
            if (_cameraTranslationAction != null) {
                _cameraTranslationAction.performed += OnTranslate;
                _cameraTranslationAction.canceled += OnTranslate;
            }

            if (_cameraRotationAction != null) {
                _cameraRotationAction.performed += OnRotate;
                _cameraRotationAction.canceled += OnRotate;
            }
        }

        private void OnDisable() {
            if (_cameraTranslationAction != null) {
                _cameraTranslationAction.performed -= OnTranslate;
                _cameraTranslationAction.canceled -= OnTranslate;
            }

            if (_cameraRotationAction != null) {
                _cameraRotationAction.performed -= OnRotate;
                _cameraRotationAction.canceled -= OnRotate;
            }
        }

        private void LateUpdate() {
            var moveDelta = (_arCameraTransform.right * _moveDirection.x + _arCameraTransform.forward * _moveDirection.z) *
                            (Time.deltaTime * MoveSpeed);
            _arCameraTransform.Translate(moveDelta, Space.World);

            if (IsGazeControl) {
                UpdateMouseRotation();
            }
            else {
                UpdateDeviceControllerRotation();
            }
        }

        private void UpdateMouseRotation() {
            var rt = RotationSensitivity * Time.fixedDeltaTime;
            var pitch = _mouseDelta.y * rt;
            var yaw = _mouseDelta.x * rt;

            _arCameraTransform.localRotation =
                Quaternion.AngleAxis(yaw, Vector3.up) * _arCameraTransform.localRotation *
                Quaternion.AngleAxis(pitch, Vector3.left);

            _mouseDelta = Vector2.zero;
        }

        private void UpdateDeviceControllerRotation() {
            if (!DeviceController) {
                return;
            }

            Vector3 mousePosition = Mouse.current.position.ReadValue();
            mousePosition = _arCamera.ScreenToWorldPoint(mousePosition + Vector3.forward);
            DeviceController.LookAt(mousePosition);
        }

        private void OnTranslate(InputAction.CallbackContext context) {
            var value = context.ReadValue<Vector2>();
            _moveDirection = new Vector3(value.x, 0.0f, value.y).normalized;
        }

        private void OnRotate(InputAction.CallbackContext context) {
            /* Only rotate if gaze control is active. */
            _mouseDelta = IsGazeControl ? _mouseDelta + context.ReadValue<Vector2>() : Vector2.zero;
        }
#endif
    }
}