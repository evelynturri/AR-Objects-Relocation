/******************************************************************************
 * File: HandTrackingSampleController.cs
 * Copyright (c) 2021-2023 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
 *
 ******************************************************************************/

using System;
using UnityEngine;
using UnityEngine.UI;

namespace Qualcomm.Snapdragon.Spaces.Samples
{
    public class HandTrackingSampleController : SampleController
    {
        public Text LeftHandGestureName;
        public Text LeftHandGestureRatio;
        public Text LeftHandFlipRatio;
        public Text RightHandGestureName;
        public Text RightHandGestureRatio;
        public Text RightHandFlipRatio;

        public GameObject Mirror;
        public GameObject MirroredPlayer;
        public GameObject MirroredPlayerHead;
        public GameObject MirroredJointObject;


        private GameObject[] _leftMirroredHandJoints;
        private GameObject[] _rightMirroredHandJoints;
        private SpacesHandManager _spacesHandManager;

        public void Awake() {
            _spacesHandManager = FindObjectOfType<SpacesHandManager>();
        }

        public override void Start() {
            base.Start();
            
            if (!SubsystemChecksPassed) {
                return;
            }
            _spacesHandManager.handsChanged += UpdateGesturesUI;

            _leftMirroredHandJoints = CreateHandJoints();
            _rightMirroredHandJoints = CreateHandJoints();
        }

        private GameObject[] CreateHandJoints() {
            var handJointCount = Enum.GetNames(typeof(SpacesHand.JointType)).Length;
            var joints = new GameObject[handJointCount];

            for (var i = 0; i < joints.Length; i++) {
                joints[i] = Instantiate(MirroredJointObject);
                joints[i].hideFlags = HideFlags.HideInHierarchy;
                joints[i].GetComponent<Renderer>().material.color = NormalizedColorForJoint(i);
            }

            return joints;
        }

        private static Color NormalizedColorForJoint(int jointId) {
            return Enum.GetName(typeof(SpacesHand.JointType), jointId).Split('_')[0] switch {
                "PALM" => Color.white,
                "WRIST" => new Color(200f / 255f, 200f / 255f, 200f / 255f),
                "THUMB" => new Color(255f / 255f, 196f / 255f, 0f / 255f),
                "INDEX" => new Color(26f / 255f, 201f / 255f, 56f / 255f),
                "MIDDLE" => new Color(0f / 255f, 215f / 255f, 255f / 255f),
                "RING" => new Color(139f / 255f, 0f / 255f, 226f / 255f),
                "LITTLE" => new Color(200f / 255f, 0f / 255f, 200f / 255f),
                _ => Color.black
            };
        }

        public override void Update() {
            base.Update();
            
            if (!SubsystemChecksPassed) {
                return;
            }
            UpdateMirroredPlayer();
        }

        private void UpdateGesturesUI(SpacesHandsChangedEventArgs args) {
            foreach (var hand in args.updated) {
                var gestureNameTextField = hand.IsLeft ? LeftHandGestureName : RightHandGestureName;
                var gestureRatioTextField = hand.IsLeft ? LeftHandGestureRatio : RightHandGestureRatio;
                var flipRatioTextField = hand.IsLeft ? LeftHandFlipRatio : RightHandFlipRatio;

                gestureNameTextField.text = Enum.GetName(typeof(SpacesHand.GestureType), hand.CurrentGesture.Type);
                gestureRatioTextField.text = (int) (hand.CurrentGesture.GestureRatio * 100f) + " %";
                flipRatioTextField.text = hand.CurrentGesture.FlipRatio.ToString("0.00");
            }

            foreach (var hand in args.removed) {
                var gestureNameTextField = hand.IsLeft ? LeftHandGestureName : RightHandGestureName;
                var gestureRatioTextField = hand.IsLeft ? LeftHandGestureRatio : RightHandGestureRatio;
                var flipRatioTextField = hand.IsLeft ? LeftHandFlipRatio : RightHandFlipRatio;

                gestureNameTextField.text = "-";
                gestureRatioTextField.text = "-";
                flipRatioTextField.text = "-";
            }
        }

        private void UpdateMirroredPlayer() {
            MirroredPlayer.transform.position = GetMirroredPosition(ARCameraTransform.transform.position);

            var reflectedForward = Vector3.Reflect(ARCameraTransform.transform.rotation * Vector3.forward, Mirror.transform.forward);
            var reflectedUp = Vector3.Reflect(ARCameraTransform.transform.rotation * Vector3.up, Mirror.transform.forward);
            MirroredPlayerHead.transform.rotation = Quaternion.LookRotation(reflectedForward, reflectedUp);

            UpdateMirroredHand(true);
            UpdateMirroredHand(false);
        }

        private void UpdateMirroredHand(bool leftHand) {
            var joints = leftHand ? _leftMirroredHandJoints : _rightMirroredHandJoints;
            for (var i = 0; i < _leftMirroredHandJoints.Length; i++) {
                var hand = leftHand ? _spacesHandManager.LeftHand : _spacesHandManager.RightHand;
                if (hand == null) {
                    joints[i].SetActive(false);
                    continue;
                }
                joints[i].SetActive(true);
                joints[i].transform.position = GetMirroredPosition(hand.Joints[i].Pose.position);
            }
        }

        private Vector3 GetMirroredPosition(Vector3 positionToMirror) {
            /* Maths for reflection across a line can be found here: https://en.wikipedia.org/wiki/Reflection_(mathematics) */
            var mirrorNormal = Mirror.transform.forward;
            var mirrorPosition = Mirror.transform.position;
            /* Position to be reflected in a hyperplane through the origin. Therefore offset, the position by the mirror position. */
            var adjustedPosition = positionToMirror - mirrorPosition;
            var reflectedPosition = adjustedPosition - 2  * Vector3.Dot(adjustedPosition, mirrorNormal) / Vector3.Dot(mirrorNormal, mirrorNormal) * mirrorNormal;

            /* Offset the origin of the reflection again by the mirror position. */
            return mirrorPosition + reflectedPosition;
        }

        protected override bool CheckSubsystem() {
#if UNITY_ANDROID && !UNITY_EDITOR
            var activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
            var runtimeChecker = new AndroidJavaClass("com.qualcomm.snapdragon.spaces.unityserviceshelper.RuntimeChecker");

            if ( !runtimeChecker.CallStatic<bool>("CheckCameraPermissions", new object[] { activity }) ) {
                Debug.LogError("Snapdragon Spaces Services has no camera permissions! Hand Tracking feature disabled.");
                return false;
            }
            return true;
#endif
            return false;
        }
    }
}