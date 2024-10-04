using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UITest : MonoBehaviour
{
    [SerializeField]
    private RectTransform test;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("ほい");
        DebugMethod();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DebugMethod()
    {
        Debug.DrawLine(test.position, test.position + new Vector3(20, 0, 0), Color.red, 100f);
    }
}
