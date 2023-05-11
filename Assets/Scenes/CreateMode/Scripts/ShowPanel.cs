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

public class ShowPanel : MonoBehaviour
{
    public GameObject PanelButtons;
    bool _show = true;

    // Start is called before the first frame update
    void Start()
    {
        _show = PanelButtons.activeSelf;
    }

    // Update is called once per frame
    void Update()
    {
        _show = PanelButtons.activeSelf;
    }

    public void NotShow()
    {
        if (_show == true)
        {
            PanelButtons.SetActive(false);
            _show = false;
        } else
        {
            PanelButtons.SetActive(true);
            _show = true;
        }
    }
}
