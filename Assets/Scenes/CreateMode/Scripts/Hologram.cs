using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Description;

[System.Serializable]
public class Hologram
{
    public string Name;
    public Vector3 Position;
    public Quaternion Quaternion;
    [System.NonSerialized]
    public GameObject HologramType;
    [System.NonSerialized]
    public Transform Transform;

    public Hologram(string Name, Vector3 Position, Quaternion Quaternion)
    {
        this.Name = Name;
        this.Position = Position;
        this.Quaternion = Quaternion;
    }

    public Hologram(string Name, Vector3 Position, Quaternion Quaternion, GameObject HologramType)
    {
        this.Name = Name;
        this.Position = Position;
        this.Quaternion = Quaternion;
        this.HologramType = HologramType;
    }
}
