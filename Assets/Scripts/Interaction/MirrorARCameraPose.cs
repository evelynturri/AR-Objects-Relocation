using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Qualcomm.Snapdragon.Spaces;

public class MirrorARCameraPose : MonoBehaviour
{
    public bool TrackPosition = true;
    public bool TrackRotation = true;
    public Vector3 PositionOffset = new Vector3(0.0f, -0.5f, 0.0f);

    private Transform _cameraTransform;

    private void Start()
    {
        _cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        if (TrackPosition)
        {
            transform.position = _cameraTransform.position + PositionOffset;
        }

        if (TrackRotation)
        {
            transform.rotation = _cameraTransform.rotation;
        }
    }
}

