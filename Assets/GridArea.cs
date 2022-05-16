using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.UI;

public class GridArea {

    private int width, height;
    private string[][] gridArray;
    public float cellSize;
    private Vector3 originPosition;
    public bool isFirstSelected;
    float cost;


    public Vector3 start;
    public Vector3 end;

    public List<MapNode> path;

    public List<DrawPath> path_drown;

    public Map map;

    public Astar a_star;

    bool is_blue_dot_moved;

    bool is_red_dot_moved;

    public GridArea(int width, int height, float cellSize, string[][] GridValues) {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = new Vector3(-(width * cellSize / 2), -(height * cellSize / 2) + 6, 0);
        this.gridArray = GridValues;
        this.path_drown = new List<DrawPath>();

    }



    public Vector3 GetWorldPosition(float x, float y) {
        return GetWorldPosition(new Vector3(x, y));
    }
    public Vector3 GetWorldPosition(Vector3 position) {
        return position * cellSize + originPosition;
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
                    GameObject.Find("StartText").GetComponentInChildren<Text>().text = "Start Point: \n"+ x +" x-axis, " + y + " y-axis";
                    is_blue_dot_moved = true;
                } else {
                    ControlSecondDot.Instance.Translate(GetWorldPosition(x, y) + new Vector3(cellSize / 2, cellSize / 2));
                    end = new Vector3(x, y, 0);
                    GameObject.Find("EndText").GetComponentInChildren<Text>().text = "End Point: \n"+ x +" x-axis, " + y + " y-axis";
                    is_red_dot_moved = true;
                }
            }
        }
    }

    public void confirmStart() {
        if (is_blue_dot_moved) {
            isFirstSelected = true;
        }
    }

    public List<List<MapNode>> confirmEnd() {
        if (!is_red_dot_moved) {
            return null;
        }
        isFirstSelected = false;
        this.map = new Map(gridArray, start, end);
        List<List<MapNode>> paths = create_path();
        return paths;
    }



    public void draw_paths(List<List<MapNode>> paths) {
        if (paths != null && path_drown.Count < 2) {
            Color[] color = { Color.green, Color.blue, Color.red };
            for (int i = 0; i < paths.Count; i++) {
                path_drown.Add(new DrawPath(paths[i], color[i], this));
            }
        }
    }

    public void draw_path() {
        path_drown.ForEach(path => path.destroy_road());
        path_drown.Clear();
        path_drown.Add(new DrawPath(path, Color.green, this));
    }


    public void choose_path(int index, List<List<MapNode>> paths) {
        this.path = paths[index];

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



    public List<List<MapNode>> create_path() {
        this.a_star = new Astar(map);
        return this.a_star.get_paths();
    }
}
