using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    public SO_Data_HandGun testObject;
    private IObjectData objectData;
    // Start is called before the first frame update
    void Start()
    {
        objectData = testObject;
        Debug.Log(objectData.StackableNum);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
