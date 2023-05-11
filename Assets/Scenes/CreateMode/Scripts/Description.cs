using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Description 
{
    public string Text;
    public Vector3 Position;
    public Quaternion Quaternion;
    [System.NonSerialized]
    public Transform Transform;

    public Description(string Text, Vector3 Position, Quaternion Quaternion)
    {
        this.Text = Text;
        this.Position = Position;
        this.Quaternion = Quaternion;
    }

}
