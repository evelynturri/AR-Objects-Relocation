using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{   

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void EnableButton(GameObject _buttonToEnable)
    {
        _buttonToEnable.SetActive(true);
    }

    public void DisableButton(GameObject _buttonToDisable)
    {
        _buttonToDisable.SetActive(false);
    }



}
