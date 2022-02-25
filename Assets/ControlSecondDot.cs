using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class ControlSecondDot : MonoBehaviour
{

    private static ControlSecondDot _instance;

    private string _text;

    public static ControlSecondDot Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Translate(Vector3 posetion)
    {
        transform.position = posetion;
    }
    public Vector3 getPosetion()
    {
        return this.getPosetion();
    }

}