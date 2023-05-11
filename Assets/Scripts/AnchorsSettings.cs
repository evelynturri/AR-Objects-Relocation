using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnchorsSettings", menuName = "AnchorsSettings", order = 0)]
public class AnchorsSettings : ScriptableObject
{
    public GameObject[] Objects;
    public GameObject ActivatedObject;
}