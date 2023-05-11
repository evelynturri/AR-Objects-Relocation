using QCHT.Interactions.Hands;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "SceneSettings", menuName = "SceneSettings", order = 0)]
public class SceneSettings : ScriptableObject
{
    public string SceneName;
    public HandInteractionType HandType;
    public bool EnablePhysicRaycast;
}

