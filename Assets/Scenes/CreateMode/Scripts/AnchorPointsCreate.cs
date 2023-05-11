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

public class AnchorPointsCreate : MonoBehaviour
{
    protected ARAnchorManager AnchorManager;

    public GameObject AnchorPoint;
    public GameObject GizmoTransparent;
    public GameObject GizmoTrackedAnchor;
    public GameObject GizmoUntrackedAnchor;
    public GameObject GizmoSavedAddition;
    public GameObject GizmoNotSavedAddition;
    public GameObject GizmoNotCheckedConstraint;
    public AnchorsSettings _AnchorsSettings;

    private SpacesAnchorStore _anchorStore;
    private SpacesHandManager _handManager;

    [SerializeField]
    private InputActionReference TriggerAction;
    [SerializeField]
    private float _constraintDistance;
    [SerializeField]
    private int maxNumberOfAnchor;

    public GameObject Note;
    public GameObject MenuPanel;

    private GameObject _indicatorGizmo;
    private GameObject _indicatorGizmoUnchecked;
    private float _placementDistance = 2f;
    public bool _saveAnchorsToStore = true;
    private bool _canPlaceAnchorGizmos = true;
    private bool _approvedConstraints = true;
    private GameObject activatedObject;
    private GameObject anchorObject;
    private Transform _cameraTransform;
    private List<GameObject> _anchorGizmos = new List<GameObject>();
    private List<GameObject> _sessionGizmos = new List<GameObject>();
    private TouchScreenKeyboard keyboard;

    public void Awake()
    {
        AnchorManager = FindObjectOfType<ARAnchorManager>();
        _anchorStore = FindObjectOfType<SpacesAnchorStore>();
        _handManager = FindObjectOfType<SpacesHandManager>();
    }

    // Start is called before the first frame update
    public void Start()
    {
        // Set the camera transform
        _cameraTransform = Camera.main.transform;

        // Set the indicatorGizmo
        _indicatorGizmo = Instantiate(GizmoTransparent, transform.position, Quaternion.identity);
        //_indicatorGizmoUnchecked = Instantiate(GizmoNotCheckedConstraint, transform.position, Quaternion.identity);
        //_indicatorGizmoUnchecked.SetActive(false);
        _saveAnchorsToStore = true;
        MenuPanel.transform.position = _cameraTransform.position + _cameraTransform.forward * _placementDistance;

        // Load all the anchors already placed with their objects
        LoadAllSavedAnchors();
    }

    public void Update()
    {
        // Update the positon of the indicator gizmo
        _indicatorGizmo.transform.position = _cameraTransform.position + _cameraTransform.forward * _placementDistance;
        //_indicatorGizmoUnchecked.transform.position = _cameraTransform.position + _cameraTransform.forward * _placementDistance;

        // Check the constraint at each frame
        _approvedConstraints = CheckConstraints();

        // Check which is the ActivatedObject
        activatedObject = _AnchorsSettings.ActivatedObject;

        // Check if all the anchors in the list have been saved in the AnchorStore
        string[] anchorNames = _anchorStore.GetSavedAnchorNames();

        if (anchorNames.Count() < AnchorPointsGlobal._anchors.Count())
        {
            foreach (var anchor in AnchorPointsGlobal._anchors)
            {
                var inStore = false;
                foreach (var name in anchorNames)
                {
                    if (!inStore)
                    {
                        if (name.Equals(anchor.Name))
                        {
                            inStore = true;
                        }
                    }
                }

                if (!inStore)
                {
                    AnchorPointsGlobal._anchors.Remove(anchor);
                }
            }
        }

    }

    public void OnEnable()
    {
        StartCoroutine(LateOnEnable());
    }

    public IEnumerator LateOnEnable()
    {
        yield return new WaitForSeconds(0.1f);
        AnchorManager.anchorsChanged += OnAnchorsChanged;
        TriggerAction.action.performed += OnTriggerAction;
    }

    public void OnDisable()
    {
        AnchorManager.anchorsChanged -= OnAnchorsChanged;
        TriggerAction.action.performed -= OnTriggerAction;
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

            var temporaryAnchor = CheckAnchor(anchor);
            temporaryAnchor.Transform = anchor.transform;

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

                }
                else
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
                    }
                    else
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

    public void OnTriggerAction(InputAction.CallbackContext context)
    {
        StartCoroutine(InstantiateObject());
    }

    public void OnCreateButtonClicked()
    {
        StartCoroutine(InstantiateObject());
    }

    public void OnCreateDescription()
    {
        StartCoroutine(AddDescription());
    }

    public IEnumerator InstantiateObject()
    {
        if (!_canPlaceAnchorGizmos && !_approvedConstraints)
        {
            yield break;
        }

        yield return new WaitForSeconds(0.1f);

        // Takes the Gizmo position
        var targetTransform = Instantiate(_indicatorGizmo.transform);
        targetTransform.gameObject.SetActive(false);

        // Update the rotation
        var targetEulerAngles = _cameraTransform.rotation.eulerAngles;
        targetEulerAngles.x = targetTransform.rotation.eulerAngles.x;
        targetEulerAngles.z = targetTransform.rotation.eulerAngles.z;
        targetTransform.rotation = Quaternion.Euler(targetEulerAngles);

        // Create the Hologram object
        var hologram = new Hologram(activatedObject.name, targetTransform.position, targetTransform.rotation, activatedObject);
        hologram.Transform = targetTransform;

        if (_approvedConstraints)
        {
            /*
             * Create a new anchor and add the hologram to its list
            */

            // Create a new gameobject in the same previous position
            var anchorGizmo = new GameObject { transform = { position = targetTransform.position, rotation = targetTransform.rotation } };
            // Attach to the gameobject the ARAnchor component
            var anchor = anchorGizmo.AddComponent<ARAnchor>();

            if (_saveAnchorsToStore)
            {
                _anchorStore.SaveAnchor(anchor, success =>
                {
                    // Create the Anchor object
                    Anchor a = new Anchor((string)(_anchorStore.GetSavedAnchorNameFromARAnchor(anchor)), targetTransform.position);
                    a.Transform = anchor.transform;

                    Instantiate(activatedObject, targetTransform.position, targetTransform.rotation, a.Transform);

                    // Update hologram's position and rotation in local coordinates
                    hologram.Position = a.Transform.InverseTransformPoint(targetTransform.position);
                    hologram.Quaternion = Quaternion.Inverse(a.Transform.rotation) * targetTransform.rotation;

                    // Add hologram to the list
                    a.AddHologram(hologram);

                    // Set as tracked the anchor
                    a.Tracked = true;

                    // Add the anchor to the list of the anchors
                    AnchorPointsGlobal._anchors.Add(a);
                });
            }
        }
        else
        {
            /*
             * Attach the hologram to the closest anchor
            */

            // Find the closest anchor to the hologram
            Anchor a = CheckClosestAnchorToHologram(hologram);//.AddHologram(hologram);

            // Instantiate hologram with anchor as parent
            Instantiate(activatedObject, targetTransform.position, targetTransform.rotation, a.Transform);

            // Update hologram's position and rotation in local coordinates
            hologram.Position = a.Transform.InverseTransformPoint(targetTransform.position);
            hologram.Quaternion = Quaternion.Inverse(a.Transform.rotation) * targetTransform.rotation;

            // Add hologram to the list
            a.AddHologram(hologram);
        }
    }

    public IEnumerator AddDescription()
    {
        yield return new WaitForSeconds(0.1f);

        // Takes the Gizmo position
        var targetTransform = Instantiate(_indicatorGizmo.transform);
        targetTransform.gameObject.SetActive(false);

        // Update the rotation
        var targetEulerAngles = _cameraTransform.rotation.eulerAngles;
        targetEulerAngles.x = targetTransform.rotation.eulerAngles.x;
        targetEulerAngles.z = targetTransform.rotation.eulerAngles.z;
        targetTransform.rotation = Quaternion.Euler(targetEulerAngles);


        if (_approvedConstraints)
        {
            /*
            * Create a new anchor and add the description to its list
           */

            // Create a new gameobject in the same previous position
            var anchorGizmo = new GameObject { transform = { position = targetTransform.position, rotation = targetTransform.rotation } };
            // Attach to the gameobject the ARAnchor component
            var anchor = anchorGizmo.AddComponent<ARAnchor>();

            // Open the keyboard
            keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, true);
            yield return new WaitUntil(() => (keyboard.status == TouchScreenKeyboard.Status.Done));

            // Check if a description has been added
            if (keyboard.text != "")
            {
                // Save the anchor with the description in the store
                _anchorStore.SaveAnchor(anchor, success =>
                {
                    // Create the Anchor
                    Anchor a = new Anchor((string)(_anchorStore.GetSavedAnchorNameFromARAnchor(anchor)), targetTransform.position);
                    a.Transform = anchor.transform;

                    // Create the description
                    var description = new Description(keyboard.text,
                        a.Transform.InverseTransformPoint(targetTransform.position),
                        Quaternion.Inverse(a.Transform.rotation) * targetTransform.rotation);

                    // Instantiate the note
                    Note.GetComponentInChildren<Text>().text = description.Text;
                    Instantiate(Note, targetTransform.position, targetTransform.rotation, a.Transform);

                    // Add description to the list
                    a.AddDescription(description);

                    // Set as tracked the anchor
                    a.Tracked = true;

                    // Add the anchor to the list of the anchors
                    AnchorPointsGlobal._anchors.Add(a);
                });
            }

        }
        else
        {
            /*
             * Attach the description to the closest anchor
            */

            // Find closest anchor to the description
            Anchor a = CheckClosestAnchorToDescription(targetTransform);

            // Open the keyboard
            keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, true);
            yield return new WaitUntil(() => (keyboard.status == TouchScreenKeyboard.Status.Done));

            // Check if a description has been added
            if (keyboard.text != "")
            {
                // Create the description
                var description = new Description(keyboard.text,
                    a.Transform.InverseTransformPoint(targetTransform.position),
                    Quaternion.Inverse(a.Transform.rotation) * targetTransform.rotation);

                // Instantiate the note
                Note.GetComponentInChildren<Text>().text = description.Text;
                Instantiate(Note, targetTransform.position, targetTransform.rotation, a.Transform);

                // Add description to the list
                a.AddDescription(description);
            }
        }

    }


    public void LoadAllSavedAnchors()
    {
        _anchorStore.LoadAllSavedAnchors();
    }

    private bool CheckConstraints()
    {
        var targetPosition = _indicatorGizmo.transform.position;
        Vector3 checkingPosition = targetPosition;

        // CHeck position of the object w.r.t. the already existing anchors
        for (int i = 0; i < AnchorPointsGlobal._anchors.Count(); i++)
        {
            var distance = Vector3.Distance(targetPosition, AnchorPointsGlobal._anchors[i].Position);
            if (Math.Abs(distance) <= _constraintDistance)
            {
                return false;
            }
        }

        // Add a max number of anchor
        if (AnchorPointsGlobal._anchors.Count() > maxNumberOfAnchor - 1)
        {
            // In this way the program does not create a new anchor but it connect the hologram to an already existing anchor
            return false;
        }

        return true;
    }

    // Function to check the closest anchor to the desired hologram
    private Anchor CheckClosestAnchorToHologram(Hologram _hologram)
    {
        var anchor = AnchorPointsGlobal._anchors[0];
        var distance = Vector3.Distance(_hologram.Position, AnchorPointsGlobal._anchors[0].Position);

        for (int i = 1; i < AnchorPointsGlobal._anchors.Count(); i++)
        {
            if (Vector3.Distance(_hologram.Position, AnchorPointsGlobal._anchors[i].Position) < distance)
            {
                distance = Vector3.Distance(_hologram.Position, AnchorPointsGlobal._anchors[i].Position);
                anchor = AnchorPointsGlobal._anchors[i];
            }
        }

        return anchor;
    }

    // Function to check the closest anchor to the desired description
    private Anchor CheckClosestAnchorToDescription(Transform _transform)
    {
        var anchor = AnchorPointsGlobal._anchors[0];
        var distance = Vector3.Distance(_transform.position, AnchorPointsGlobal._anchors[0].Position);

        for (int i = 1; i < AnchorPointsGlobal._anchors.Count(); i++)
        {
            if (Vector3.Distance(_transform.position, AnchorPointsGlobal._anchors[i].Position) < distance)
            {
                distance = Vector3.Distance(_transform.position, AnchorPointsGlobal._anchors[i].Position);
                anchor = AnchorPointsGlobal._anchors[i];
            }
        }

        return anchor;
    }

    // Function to check which is the corrispetive anchor of the global variables
    public Anchor CheckAnchor(ARAnchor arAnchor)
    {
        for (int i = 0; i < AnchorPointsGlobal._anchors.Count(); i++)
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
