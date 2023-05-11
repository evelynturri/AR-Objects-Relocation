using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Qualcomm.Snapdragon.Spaces;
using QCHT.Interactions.Hands;
using UnityEngine.SceneManagement;
using static Anchor;
using static Description;
using static Hologram;
using static AnchorPointsGlobal;
using static AnchorPointsCreate;

public class AnchorPointsLoad : MonoBehaviour
{
    protected ARAnchorManager AnchorManager;

    public GameObject AnchorPoint;
    public GameObject GizmoTransparent;
    public GameObject GizmoTrackedAnchor;
    public GameObject GizmoUntrackedAnchor;
    public GameObject GizmoSavedAddition;
    public GameObject GizmoNotSavedAddition;
    public AnchorsSettings _AnchorsSettings;

    public GameObject Note;


    protected SpacesAnchorStore _anchorStore;
    protected SpacesHandManager _handManager;
    private Transform _cameraTransform;
    protected List<GameObject> _anchorGizmos;

    void Awake()
    {
        AnchorManager = FindObjectOfType<ARAnchorManager>();
        _anchorStore = FindObjectOfType<SpacesAnchorStore>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Set the camera transform
        _cameraTransform = Camera.main.transform;

        // Load all the anchors and their objects
        LoadAllSavedAnchors();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void OnEnable()
    {
        StartCoroutine(LateOnEnable());
    }

    public virtual IEnumerator LateOnEnable()
    {
        yield return new WaitForSeconds(0.1f);
        AnchorManager.anchorsChanged += OnAnchorsChanged;
    }

    public virtual void OnDisable()
    {
        AnchorManager.anchorsChanged -= OnAnchorsChanged;
        foreach (var anchor in AnchorPointsGlobal._anchors)
        {
            anchor.Tracked = false;
        }
    }

    public void OnAnchorsChanged(ARAnchorsChangedEventArgs args)
    {
        foreach (var anchor in args.added)
        {
            _anchorGizmos.Add(anchor.gameObject);
        }

        foreach (var anchor in args.updated)
        {
            if (anchor.transform.childCount > 0)
            {
                Destroy(anchor.transform.GetChild(0).gameObject);
            }

            // Check which anchor in the global variable is updated
            var temporaryAnchor = CheckAnchor(anchor);
           
            if (_anchorStore.GetSavedAnchorNameFromARAnchor(anchor) != string.Empty)
            {
                // Check if the anchor has already been tracked since the scene has started
                if (!temporaryAnchor.Tracked)
                {
                    // If the anchor has not already been track
                    if (anchor.trackingState == TrackingState.Tracking)
                    {
                        /* 
                         * If the application has started to track the anchor then load the holograms
                         * In this case, it would be the first time that the anchor is tracked
                        */
                        LoadObjects(temporaryAnchor, anchor.transform);
                        temporaryAnchor.Tracked = true;
                    }
                   
                } else
                {
                    /* 
                    * If the anchor has already been tracked in previous frames, 
                    * then delete the children Transform of all the objects and 
                    * if the anchor is actually tracked then reload them again.
                    */
                    foreach (Transform t in anchor.transform)
                    {
                        GameObject.Destroy(t.gameObject);
                    }
                    if (anchor.trackingState == TrackingState.Tracking)
                    {
                        LoadObjects(temporaryAnchor, anchor.transform);
                    } else
                    {
                        temporaryAnchor.Tracked = false;
                    }
                }
            }
        }

        foreach (var anchor in args.removed)
        {
            _anchorGizmos.Remove(anchor.gameObject);
        }
    }

    public void LoadAllSavedAnchors()
    {
        _anchorStore.LoadAllSavedAnchors();
    }

    // Function to check which is the corrispetive anchor of the global variables
    public Anchor CheckAnchor(ARAnchor arAnchor)
    {
        for (int i = 0; i<AnchorPointsGlobal._anchors.Count(); i++)
        {
            if (_anchorStore.GetSavedAnchorNameFromARAnchor(arAnchor).Equals(AnchorPointsGlobal._anchors[i].Name))
            {
                return AnchorPointsGlobal._anchors[i];
            }
        }

        return AnchorPointsGlobal._anchors[0];
    }

    public void LoadObjects(Anchor anchor, Transform anchorTransform)
    {
        // Instantiate all the holograms of an anchor
        foreach (var hologram in anchor.Holograms)
        {
            Instantiate(hologram.HologramType, anchorTransform.TransformPoint(hologram.Position), anchorTransform.rotation * hologram.Quaternion, anchorTransform);
        }

        // Instantiate all the descriptions of an anchor
        foreach (var description in anchor.Descriptions)
        {
            Note.GetComponentInChildren<Text>().text = description.Text;
            Instantiate(Note, anchorTransform.TransformPoint(description.Position), anchorTransform.rotation * description.Quaternion, anchorTransform);
        }
    }

}
