using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class GridArea
{

    private int width, height;
    private int[,] gridArray;
    private float cellSize;
    private TextMesh[,] debugArray;
    private Vector3 originPosition;

    public GridArea(int width, int height, float cellSize, Vector3 originPosition, int[,] GridValues)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;
        this.gridArray = new int[width, height];
        this.debugArray = new TextMesh[width, height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                debugArray[i, j] = createWorldText(null, gridArray[i, j].ToString(), GetWorldPosition(i, j) + new Vector3(cellSize, cellSize) * .5f, 20, "white", TextAnchor.MiddleCenter);
                Debug.DrawLine(GetWorldPosition(i, j), GetWorldPosition(i, j + 1), Color.white, 100f);
                Debug.DrawLine(GetWorldPosition(i, j), GetWorldPosition(i + 1, j), Color.white, 100f);
            }
        }
        Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
        Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);


    }

    public TextMesh createWorldText(Transform parent = null, string text = "", Vector3 localPosition = default(Vector3), int fontSize = 40, string color = "white", TextAnchor anchor = TextAnchor.UpperLeft, TextAlignment alignment = TextAlignment.Left, int sortingOrder = 5000)
    {
        GameObject gameObject = new GameObject("world text", typeof(TextMesh));
        Transform transform = gameObject.transform;
        transform.SetParent(parent, false);
        transform.localPosition = localPosition;
        TextMesh textMesh = gameObject.GetComponent<TextMesh>();
        textMesh.anchor = anchor;
        textMesh.alignment = alignment;
        textMesh.text = text;
        textMesh.fontSize = fontSize;
        textMesh.color = Color.white;
        textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
        return textMesh;
    }

    private Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, y) * cellSize + originPosition;
    }

    private void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
    }

    public void SetValue(int x, int y, int value)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            gridArray[x, y]++;
            debugArray[x, y].text = gridArray[x, y].ToString();
        }
    }
    public void SetValue(Vector3 worldPosition, int value)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        SetValue(x, y, value);
    }


    // Start is called before the first frame update
    void Start()
    {

    }



    // Update is called once per frame
    void Update()
    {

    }


}
