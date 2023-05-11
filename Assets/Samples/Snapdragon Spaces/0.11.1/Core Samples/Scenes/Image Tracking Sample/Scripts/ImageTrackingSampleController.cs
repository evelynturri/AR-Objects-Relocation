/******************************************************************************
 * File: ImageTrackingSampleController.cs
 * Copyright (c) 2022 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
 *
 ******************************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace Qualcomm.Snapdragon.Spaces.Samples
{
    public class ImageTrackingSampleController : SampleController
    {
        public ARTrackedImageManager arImageManager;

        [Serializable]
        public struct TrackableInfo {
            public Text TrackingStatusText;
            public Text[] PositionTexts;
        }
        public TrackableInfo[] trackableInfos;

        private Dictionary<TrackableId, TrackableInfo> _trackedImages = new Dictionary<TrackableId, TrackableInfo>();

        public override void OnEnable() {
            base.OnEnable();
            arImageManager.trackedImagesChanged += OnTrackedImagesChanged;
        }

        public override void OnDisable() {
            base.OnDisable();
            arImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
        }

        private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs args) {
            foreach (var trackedImage in args.added) {
                if (trackedImage.referenceImage.name == "Spaces Town")
                    _trackedImages.Add(trackedImage.trackableId, trackableInfos[0]);
            }

            foreach (var trackedImage in args.updated) {
                if (_trackedImages.TryGetValue(trackedImage.trackableId, out TrackableInfo info)) {
                    Vector3 position = trackedImage.transform.position;

                    info.TrackingStatusText.text = trackedImage.trackingState.ToString();
                    info.PositionTexts[0].text = position.x.ToString("#0.00");
                    info.PositionTexts[1].text = position.y.ToString("#0.00");
                    info.PositionTexts[2].text = position.z.ToString("#0.00");
                }
            }

            foreach (var trackedImage in args.removed) {
                if (_trackedImages.TryGetValue(trackedImage.trackableId, out TrackableInfo info)) {
                    info.TrackingStatusText.text = "None";
                    info.PositionTexts[0].text = "0.00";
                    info.PositionTexts[1].text = "0.00";
                    info.PositionTexts[2].text = "0.00";
                    _trackedImages.Remove(trackedImage.trackableId);
                }
            }
        }

        protected override bool CheckSubsystem() {
            return arImageManager.subsystem?.running ?? false;
        }
    }
}