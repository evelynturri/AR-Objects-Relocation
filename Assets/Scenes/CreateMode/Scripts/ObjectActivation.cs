using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectActivation : MonoBehaviour
{
    public AnchorsSettings _AnchorsSettings;

    // Start is called before the first frame update
    void Start()
    {
        _AnchorsSettings.ActivatedObject = _AnchorsSettings.Objects[0];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Activation(string NameObject)
    {
        for (int i = 0; i < _AnchorsSettings.Objects.Length; i++)
        {
            if (_AnchorsSettings.Objects[i].name == NameObject )
            {
                _AnchorsSettings.ActivatedObject = _AnchorsSettings.Objects[i];
                _AnchorsSettings.ActivatedObject.name = _AnchorsSettings.Objects[i].name;
                return;
            }
        }

        _AnchorsSettings.ActivatedObject = _AnchorsSettings.Objects[0];
        _AnchorsSettings.ActivatedObject.name = _AnchorsSettings.Objects[0].name;
        return; 
    }
}
