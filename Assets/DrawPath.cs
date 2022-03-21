using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DrawPath : MonoBehaviour {

    TestScript script;
    GridArea grid;

    public List<GameObject> draw_road;

    Astar path_finding;
    // Start is called before the first frame update
    void Start() {
        draw_road = new List<GameObject>();
        if (GameObject.FindWithTag(("Grid")) != null) {
            script = GameObject.FindWithTag(("Grid")).GetComponent<TestScript>();
        }
    }

    void Update() {
        if (grid == null) {
            grid = script.grid;
        }
        if (grid != null) {
            // i want the square to appear after the path is found
            if (grid.path != null) {
                path_finding = grid.path;
                if (!grid.isFirstSelected) {
                    draw();
                } else {
                    foreach (GameObject draw in draw_road) {
                        Destroy(draw);
                    }
                    draw_road.Clear();
                }
            }
        }
    }

    public Vector3 get_position(float x, float y) {
        return grid.GetWorldPosition(x, y) + new Vector3(grid.cellSize / 2, grid.cellSize / 2);
    }

    void draw() {
        if (path_finding != null) {
            for (int i = 0; i < path_finding.path.Count - 1; i++) {
                Vector3 start = get_position(path_finding.path[i].position.x, path_finding.path[i].position.y);
                Vector3 end = get_position(path_finding.path[i + 1].position.x, path_finding.path[i + 1].position.y);
                var line = DrawLine(start, end, Color.green);
                draw_road.Add(line);
            }
        }
    }

    GameObject DrawLine(Vector3 start, Vector3 end, Color color, float width = 0.55f) {
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended"));
        lr.startColor = color;
        lr.endColor = color;
        lr.startWidth = width;
        lr.endWidth = width;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        return myLine;
    }
}

