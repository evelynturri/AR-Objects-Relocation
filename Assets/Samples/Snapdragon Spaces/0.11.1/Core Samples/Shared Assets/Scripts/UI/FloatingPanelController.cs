/******************************************************************************
 * File: FloatingPanelController.cs
 * Copyright (c) 2022-2023 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
 *
 ******************************************************************************/

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;

namespace Qualcomm.Snapdragon.Spaces.Samples
{
    public class FloatingPanelController : MonoBehaviour
    {
        public bool FollowGaze = true;
        public float TargetDistance = 1.0f;
        public float MovementSmoothness = 0.2f;

        private ARSessionOrigin _arSessionOrigin;
        private Transform _arCameraTransform;
        private Camera _arCamera;
        private XRControllerManager _controllerManager;

        private void Start() {
            _arSessionOrigin ??= FindObjectOfType<ARSessionOrigin>();
            _arCamera = _arSessionOrigin.camera;
            _arCameraTransform = _arCamera.transform;
        }

        private void Update() {
            if (FollowGaze) {
                AdjustPanelPosition();
            }
        }

        public void SwitchToScene(string name) {
            _controllerManager ??= FindObjectOfType<XRControllerManager>(true);
            _controllerManager?.SendHapticImpulse();
            SceneManager.LoadScene(name);
        }

        /* Ajdusts the position of the Panel if the gaze moves outside of the inner rectangle of the FOV,
         * which is half the length in both axis.
         */
        private void AdjustPanelPosition() {
            var headPosition = _arCameraTransform.position;
            var gazeDirection = _arCameraTransform.forward;

            var direction = (transform.position - headPosition).normalized;
            var targetPosition = headPosition + gazeDirection * TargetDistance;
            var targetDirection = (targetPosition - headPosition).normalized;

            var eulerAngles = Quaternion.LookRotation(direction).eulerAngles;
            var targetEulerAngles = Quaternion.LookRotation(targetDirection).eulerAngles;

            var verticalHalfAngle = _arCamera.fieldOfView * 0.8f;
            eulerAngles.x += GetAdjustedDelta(targetEulerAngles.x - eulerAngles.x, verticalHalfAngle);

            var horizontalHalfAngle = _arCamera.fieldOfView * 0.5f * _arCamera.aspect;
            eulerAngles.y += GetAdjustedDelta(targetEulerAngles.y - eulerAngles.y, horizontalHalfAngle);

            targetPosition = headPosition + Quaternion.Euler(eulerAngles) * Vector3.forward * TargetDistance;

            transform.position = Vector3.Lerp(transform.position, targetPosition, MovementSmoothness);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(transform.position - headPosition), MovementSmoothness);
        }

        /* Returns the normalized delta to a certain threshold, if it exceeds that threshold. Otherwise return 0. */
        private float GetAdjustedDelta(float angle, float threshold) {
            /* Normalize angle to be between 0 and 360. */
            angle = (540f + angle) % 360f - 180f;
            if(Mathf.Abs(angle) > threshold) {
                return -angle / Mathf.Abs(angle) * (threshold - Mathf.Abs(angle));
            } else {
                return 0f;
            }
        }
    }
}