/******************************************************************************
 * File: HitTestingSampleController.cs
 * Copyright (c)2021-2023 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
 *
 ******************************************************************************/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace Qualcomm.Snapdragon.Spaces.Samples
{
    public class HitTestingSampleController : SampleController
    {
        public GameObject HitIndicator;
        public GameObject NoHitIndicator;

        private ARRaycastManager _raycastManager;
        private GameObject _activeIndicator;
        private bool _isHit = false;
        private Vector3 _desiredPosition;

        public void Awake() {
            _raycastManager = FindObjectOfType<ARRaycastManager>();
        }
        
        public override void Start() {
            base.Start();

            if (!SubsystemChecksPassed) {
                return;
            }
            _activeIndicator = NoHitIndicator;
            _activeIndicator.SetActive(true);
        }

        public override void Update() {
            base.Update();
            
            if (!SubsystemChecksPassed) {
                return;
            }
            CastRay();
            _activeIndicator.transform.position = _desiredPosition;
        }

        public void CastRay() {
            Ray ray = new Ray(ARCameraTransform.position, ARCameraTransform.forward);

            List<ARRaycastHit> hitResults = new List<ARRaycastHit>();
            if (_raycastManager.Raycast(ray, hitResults)) {
                _desiredPosition = hitResults[0].pose.position;

                if (!_isHit) {
                    _activeIndicator.SetActive(false);
                    _activeIndicator = HitIndicator;
                    _activeIndicator.SetActive(true);
                    _isHit = true;
                }
            }
            else {
                _desiredPosition = ARCameraTransform.position + ARCameraTransform.forward;

                if (_isHit) {
                    _activeIndicator.SetActive(false);
                    _activeIndicator = NoHitIndicator;
                    _activeIndicator.SetActive(true);
                    _isHit = false;
                }
            }
        }
        
        protected override bool CheckSubsystem() {
#if UNITY_ANDROID && !UNITY_EDITOR
            var activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
            var runtimeChecker = new AndroidJavaClass("com.qualcomm.snapdragon.spaces.unityserviceshelper.RuntimeChecker");

            if ( !runtimeChecker.CallStatic<bool>("CheckCameraPermissions", new object[] { activity }) ) {
                Debug.LogError("Snapdragon Spaces Services has no camera permissions! Hit Testing feature disabled.");
                return false;
            }
            return _raycastManager.subsystem?.running ?? false;
#endif
            return false;
        }
    }
}
