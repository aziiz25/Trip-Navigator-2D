using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class GridArea
{

    private int width, height;
    private string[,] gridArray;
    private float cellSize;
    private TextMesh[,] debugArray;
    private Vector3 originPosition;
    private bool isFirstSelected;

    public GridArea(int width, int height, float cellSize, string[,] GridValues)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = new Vector3(-(width * cellSize / 2), -(height * cellSize / 2) + 6, 0);
        //this.gridArray = new int[width, height];
        this.gridArray = GridValues;
        this.debugArray = new TextMesh[width, height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                debugArray[i, j] = createWorldText(null, /*GridValues[i, j]*/"", GetWorldPosition(i, j) + new Vector3(cellSize, cellSize) * .5f, 20, "white", TextAnchor.MiddleCenter);
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
            if (gridArray[x, y].Equals("0"))
            {
                nearestPosition(ref x, ref y);
            }

            if (!isFirstSelected)
            {
                ControlFirstDot.Instance.Translate(GetWorldPosition(x, y) + new Vector3(cellSize / 2, cellSize / 2));
                isFirstSelected = true;
            }
            else
            {
                ControlSecondDot.Instance.Translate(GetWorldPosition(x, y) + new Vector3(cellSize / 2, cellSize / 2));
                isFirstSelected = false;
            }

        }
    }
    public void SetValue(Vector3 worldPosition, int value)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        SetValue(x, y, value);
    }

    private void nearestPosition(ref int x, ref int y)
    {
        int step = 1;
        while (true)
        {
            //up
            if (y + step < height - 1 && !gridArray[x, y + step].Equals("0"))
            {
                y = y + step;
                break;
            }
            //left
            if (x - step >= 0 && !gridArray[x - step, y].Equals("0"))
            {
                x = x - step;
                break;
            }
            //down
            if (y - step >= 0 && !gridArray[x, y - step].Equals("0"))
            {
                y = y - step;
                break;
            }
            //right
            if (x + step < width - 1 && !gridArray[x + step, y].Equals("0"))
            {
                x = x + step;
                break;
            }
            step++;
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


}
