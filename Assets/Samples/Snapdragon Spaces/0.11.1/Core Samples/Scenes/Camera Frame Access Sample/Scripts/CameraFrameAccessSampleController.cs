/******************************************************************************
 * File: CameraFrameAccessSampleController.cs
 * Copyright (c) 2023 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
 *
 ******************************************************************************/

using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace Qualcomm.Snapdragon.Spaces.Samples
{
    public class CameraFrameAccessSampleController : SampleController
    { 
        public RawImage CameraRawImage;
        public Text[] ResolutionTexts;
        public Text[] FocalLengthTexts;
        public Text[] PrincipalPointTexts;
        public Text DeviceNotSupportedText;
        

        private ARCameraManager _cameraManager;
        private NativeArray<XRCameraConfiguration> _cameraConfigs;
        private float _targetFPS;
        private float _frameTime;
        private float _currentFrameTime;
        private bool _feedPaused;
        private bool _deviceSupported;

        private XRCameraIntrinsics _intrinsics;
        private XRCpuImage _lastCpuImage;
        private Texture2D _cameraTexture;

        public void Awake() {
            _cameraManager = FindObjectOfType<ARCameraManager>();
        }

        public override void Start() {
            base.Start();

            _deviceSupported = CheckDeviceSupported();
            if (!_deviceSupported) {
                OnDeviceNotSupported();
                return;
            }
            
            if (!SubsystemChecksPassed) {
                return;
            }

            _deviceSupported = FindSupportedConfiguration();
            if (!_deviceSupported) {
                OnDeviceNotSupported();
                return;
            }

            _targetFPS = (int) _cameraConfigs[0].framerate;
            _frameTime = 1 / _targetFPS;
            _currentFrameTime = _frameTime;
            UpdateCameraIntrinsics();
        }

        public override void Update() {
            base.Update();
            
            if (!SubsystemChecksPassed || !_deviceSupported) {
                return;
            }
            if (_feedPaused || _targetFPS <= 0) {
                return;
            }

            _currentFrameTime -= Time.deltaTime;
            if (_currentFrameTime <= 0) {
                _currentFrameTime = _frameTime;
                _lastCpuImage = new XRCpuImage();
                if (!_cameraManager.TryAcquireLatestCpuImage(out _lastCpuImage)) {
                    Debug.Log("Failed to acquire latest cpu image.");
                    return;
                }
                
                UpdateCameraTexture(_lastCpuImage);
            }
        }

        private unsafe void UpdateCameraTexture(XRCpuImage image) {
                var format = TextureFormat.RGBA32;

                if (_cameraTexture == null || _cameraTexture.width != image.width || _cameraTexture.height != image.height)
                {
                    _cameraTexture = new Texture2D(image.width, image.height, format, false);
                }

                var conversionParams = new XRCpuImage.ConversionParams(image, format);
                var rawTextureData = _cameraTexture.GetRawTextureData<byte>();
                
                try
                {
                    image.Convert(conversionParams, new IntPtr(rawTextureData.GetUnsafePtr()), rawTextureData.Length);
                }
                finally
                {
                    image.Dispose();
                }

                _cameraTexture.Apply();
                CameraRawImage.texture = _cameraTexture;
        }

        private void UpdateCameraIntrinsics() {
            if (!_cameraManager.TryGetIntrinsics(out _intrinsics)) {
                Debug.Log("Failed to acquire camera intrinsics.");
                return;
            }
            
            ResolutionTexts[0].text = _intrinsics.resolution.x.ToString();
            ResolutionTexts[1].text = _intrinsics.resolution.y.ToString();
            FocalLengthTexts[0].text = _intrinsics.focalLength.x.ToString("#0.00");
            FocalLengthTexts[1].text = _intrinsics.focalLength.y.ToString("#0.00");
            PrincipalPointTexts[0].text = _intrinsics.principalPoint.x.ToString("#0.00");
            PrincipalPointTexts[1].text = _intrinsics.principalPoint.y.ToString("#0.00");
        }

        private bool FindSupportedConfiguration() {
            _cameraConfigs = _cameraManager.GetConfigurations(Allocator.Persistent);
            return _cameraConfigs.Length > 0;
        }

        private bool CheckDeviceSupported() {
            /* Currently support only Motorola Rogue */
            bool deviceSupported = SystemInfo.deviceModel.ToLower().Contains("motorola edge");
            return deviceSupported;
        }

        private void OnDeviceNotSupported() {
            foreach (var content in ContentOnPassed) {
                content.SetActive(false);
            }
            foreach (var content in ContentOnFailed) {
                content.SetActive(true);
            }

            DeviceNotSupportedText.text = "This feature is not currently supported on this device.";
        }

        public void OnPausePress() {
            _feedPaused = true;
        }

        public void OnResumePress() {
            _feedPaused = false;
            _currentFrameTime = _frameTime;
        }
        
        protected override bool CheckSubsystem() {
            return _cameraManager.subsystem?.running ?? false;
        }
    }
}
