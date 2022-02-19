using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    // Start is called before the first frame update
    GridArea grid;
    void Start()
    {
        string currentPath = Directory.GetCurrentDirectory();
        int[,] mapValues = readMapData(currentPath + "/Assets/Maps/map 1.csv");
        grid = new GridArea(71, 40, 1.5f, new Vector3(-53.25f, -30), mapValues);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            grid.SetValue(GetMousePositionWorld(), 1);
        }
    }

    public Vector3 GetMousePositionWorld()
    {
        Vector3 vec = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        vec.z = 0f;
        return vec;
    }

    public int[,] readMapData(string filePath)
    {
        int[,] values = new int[88, 40];
        string[] text = System.IO.File.ReadAllLines(@filePath);
        string[][] stringValues = new string[40][];
        for (int i = 0; i < 40; i++)
        {
            stringValues[i] = text[i].Split(',');
        }
        for (int i = 0; i < 88; i++)
        {
            for (int j = 0; j < 40; j++)
            {
                values[i, 39 - j] = int.Parse(stringValues[j][i]);
            }
        }
        return values;
    }
}
