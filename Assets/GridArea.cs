using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class GridArea {

    private int width, height;
    private string[][] gridArray;
    public float cellSize;
    private Vector3 originPosition;
    public bool isFirstSelected;


    public Vector3 start;
    public Vector3 end;

    public List<MapNode> path;
    

    public Map map;

    public GridArea(int width, int height, float cellSize, string[][] GridValues) {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = new Vector3(-(width * cellSize / 2), -(height * cellSize / 2) + 6, 0);
        this.gridArray = GridValues;

    }



    public Vector3 GetWorldPosition(float x, float y) {
        return new Vector3(x, y) * cellSize + originPosition;
    }

    private void GetXY(Vector3 worldPosition, out int x, out int y) {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
    }

    public void SetValue(int x, int y, int value) {
        if (x >= 0 && y >= 0 && x < width && y < height) {
            if (gridArray[x][y].Equals("0")) {
                nearestPosition(ref x, ref y);
            }
            if (!gridArray[x][y].Equals("0")) {
                if (!isFirstSelected) {
                    ControlFirstDot.Instance.Translate(GetWorldPosition(x, y) + new Vector3(cellSize / 2, cellSize / 2));
                    start = new Vector3(x, y, 0);
                } else {
                    ControlSecondDot.Instance.Translate(GetWorldPosition(x, y) + new Vector3(cellSize / 2, cellSize / 2));
                    end = new Vector3(x, y, 0);
                }
            }
        }
    }

    public void confirmStart() {
        isFirstSelected = true;
    }

    public void confirmEnd() {
        isFirstSelected = false;
        this.map = new Map(gridArray, start, end);
        this.path = create_path();
    }
    public void SetValue(Vector3 worldPosition, int value) {
        int x, y;
        GetXY(worldPosition, out x, out y);
        SetValue(x, y, value);
    }

    private void nearestPosition(ref int x, ref int y) {
        int step = 1;
        while (step < width) {
            try {
                //up
                if (y + step < height - 1 && !gridArray[x][y + step].Equals("0")) {
                    y = y + step;
                    break;
                }
                //left
                if (x - step >= 0 && !gridArray[x - step][y].Equals("0")) {
                    x = x - step;
                    break;
                }
                //down
                if (y - step >= 0 && !gridArray[x][y - step].Equals("0")) {
                    y = y - step;
                    break;
                }
                //right
                if (x + step < width - 1 && !gridArray[x + step][y].Equals("0")) {
                    x = x + step;
                    break;
                }
                step++;
            } catch (Exception e) {
                Debug.Log(e);
                break;
            }
        }
    }

    public List<MapNode> create_path() {
        return new Astar(map).get_path();
    }
}
