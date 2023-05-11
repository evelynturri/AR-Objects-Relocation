/******************************************************************************
 * File: SpatialMeshingSampleController.cs
 * Copyright (c) 2022-2023 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
 *
 ******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.OpenXR;

namespace Qualcomm.Snapdragon.Spaces.Samples
{
    public class SpatialMeshingSampleController : SampleController
    {
        private ARMeshManager _meshManager;

        public void Awake() {
            _meshManager = FindObjectOfType<ARMeshManager>();
        }

        public override void OnEnable() {
            base.OnEnable();
            if (!GetSubsystemCheck()) {
                return;
            }
            _meshManager.meshesChanged += OnMeshesChanged;
        }

        public override void OnDisable() {
            base.OnDisable();
            _meshManager.meshesChanged -= OnMeshesChanged;
        }

        void OnMeshesChanged(ARMeshesChangedEventArgs args) {
            foreach (var meshFilter in args.added) {
                Debug.Log("Added meshFilter: " + meshFilter.name);
            }

            foreach (var meshFilter in args.updated) {
                Debug.Log("Updated meshFilter: " + meshFilter.name);
            }

            foreach (var meshFilter in args.removed) {
                Debug.Log("Removed meshFilter: " + meshFilter.name);
            }
        }
        
        protected override bool CheckSubsystem() {
#if UNITY_ANDROID && !UNITY_EDITOR
            var activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
            var runtimeChecker = new AndroidJavaClass("com.qualcomm.snapdragon.spaces.unityserviceshelper.RuntimeChecker");

            if ( !runtimeChecker.CallStatic<bool>("CheckCameraPermissions", new object[] { activity }) ) {
                Debug.LogError("Snapdragon Spaces Services has no camera permissions! Spatial Meshing feature disabled.");
                return false;
            }
#endif

            return _meshManager.subsystem?.running ?? false;
        }
    }
}
