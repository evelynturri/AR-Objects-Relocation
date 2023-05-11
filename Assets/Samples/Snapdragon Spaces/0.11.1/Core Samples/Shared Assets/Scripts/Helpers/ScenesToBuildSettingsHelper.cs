/******************************************************************************
 * File: ScenesToBuildSettingsHelper.cs
 * Copyright (c) 2021-2022 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
 * 
 ******************************************************************************/

#if UNITY_EDITOR

using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ScenesToBuildSettingsHelper : MonoBehaviour
{
    [MenuItem("Window/Snapdragon Spaces/Add Scenes to Build Settings")]
    public static void AddScenesToBuildSettings() {
        var sampleScenes = AssetDatabase.FindAssets("t:Scene", new[] { Path.Combine("Assets", "Samples", "Snapdragon Spaces")});
        var editorBuildSettingsScenes = sampleScenes.Select(scene => new EditorBuildSettingsScene(AssetDatabase.GUIDToAssetPath(scene), true));
        /* Order list of scenes by path length, because the Main Menu scene will have the shortest one. */
        var orderedScenes = editorBuildSettingsScenes.OrderByDescending(scene => scene.path.Contains("Main Menu"));
        if (!orderedScenes.Any()) {
            Debug.Log("Can't find all sample scenes to add to the Editor Build settings.");
        } else {
            EditorBuildSettings.scenes = orderedScenes.ToArray();
            EditorWindow.GetWindow(Type.GetType("UnityEditor.BuildPlayerWindow,UnityEditor"));
        }
    }
}

#endif
