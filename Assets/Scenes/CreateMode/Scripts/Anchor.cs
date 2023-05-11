using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Hologram;
using static Description;
using UnityEngine.XR.ARFoundation;

[Serializable]
public class Anchor 
{
    public string Name;
    public Vector3 Position;
    public List<Hologram> Holograms;
    public List<Description> Descriptions;
    public bool Tracked;
    [System.NonSerialized]
    public Transform Transform;
    

    public Anchor() { }

    public Anchor(string Name, Vector3 Position)
    {
        this.Name = Name;
        this.Position = Position;
        this.Holograms = new List<Hologram>();
        this.Descriptions = new List<Description>();
        this.Tracked = false;
    }

    public void AddHologram(Hologram hologram)
    {
        this.Holograms.Add(hologram);
    }

    public void AddDescription(Description description)
    {
        this.Descriptions.Add(description);
    }
}
