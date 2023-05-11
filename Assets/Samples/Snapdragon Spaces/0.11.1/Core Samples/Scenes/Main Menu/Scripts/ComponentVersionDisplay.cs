/******************************************************************************
 * File: ComponentVersionDisplay.cs
 * Copyright (c) 2022 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
 *
 ******************************************************************************/

using System;
using UnityEngine;
using UnityEngine.UI;

public class ComponentVersionDisplay : MonoBehaviour
{
    public string ComponentNameText {
        get => ComponentName.text;
        set {
            if (string.IsNullOrEmpty(value)) {
                throw new ArgumentException("Value cannot be null or empty.", nameof(value));
            }
            ComponentName.text = value;
        }
    }

    public string VersionIdentifierText {
        get => VersionIdentifier.text;
        set {
            if (string.IsNullOrEmpty(value)) {
                VersionIdentifierGameObject.SetActive(false);
            }
            VersionIdentifier.text = value;
        }
    }

    public string BuildIdentifierText {
        get => BuildIdentifier.text;
        set {
            if (string.IsNullOrEmpty(value)) {
                BuildIdentifierGameObject.SetActive(false);
            }
            BuildIdentifier.text = value;
        }
    }

    public string BuildDateTimeText {
        get => BuildDateTime.text;
        set {
            if (string.IsNullOrEmpty(value)) {
                BuildDateTimeGameObject.SetActive(false);
            }
            BuildDateTime.text = value;
        }
    }

    public Text ComponentName;
    public Text VersionIdentifier;
    public Text BuildIdentifier;
    public Text BuildDateTime;

    public GameObject VersionIdentifierGameObject;
    public GameObject BuildIdentifierGameObject;
    public GameObject BuildDateTimeGameObject;
}