/******************************************************************************
 * File: PlaneDetectionSampleController.cs
 * Copyright (c) 2022 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
 *
 ******************************************************************************/

using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.OpenXR;

namespace Qualcomm.Snapdragon.Spaces.Samples
{
    public class PlaneDetectionSampleController : SampleController
    {
        private ARPlaneManager _planeManager;

        private PlaneDetectionFeature _planeDetection;

        public Toggle EnableConvexHullToggle;

        public override void Start() {
            base.Start();
            _planeDetection = OpenXRSettings.Instance.GetFeature<PlaneDetectionFeature>();

            if (_planeDetection != null) {
                EnableConvexHullToggle.isOn = _planeDetection.ConvexHullEnabled;
                EnableConvexHullToggle.interactable = _planeDetection.UseSceneUnderstandingPlaneDetection;
            }
        }

        public void OnToggleConvexHull(bool inValue) {
            if (_planeDetection != null) {
                _planeDetection.ConvexHullEnabled = inValue;
            }
        }

        public void Awake() {
            _planeManager = FindObjectOfType<ARPlaneManager>();
        }

        protected override bool CheckSubsystem() {
            return _planeManager.subsystem?.running ?? false;
        }
    }
}
