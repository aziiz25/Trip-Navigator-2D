using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DrawPath : MonoBehaviour {

    TestScript script;
    GridArea grid;

    public List<GameObject> draw_road;

    public GameObject road;

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
                }
            }
        }
    }

    public Vector3 set_position(int x, int y) {
        return grid.GetWorldPosition(x, y) + new Vector3(grid.cellSize / 2, grid.cellSize / 2);
    }


    //not working as intended; :(
    public void draw() {
        if (path_finding != null) {
            for (int i = 0; i < path_finding.path.Count - 1; i++) {
                float dx = path_finding.path[i].position.x - path_finding.path[i + 1].position.x;
                float dy = path_finding.path[i].position.y - path_finding.path[i + 1].position.y;
                if (dx == 0) {
                    int x = (int)(path_finding.path[i].position.x);
                    int y = (int)(path_finding.path[i].position.y - dy / 2);
                    if (dy < 0) {
                        //up
                        add_road(x, y, 0.5f, (dy * -1) + 1f);
                    } else if (dy > 0) {
                        // down
                        add_road(x, y, 0.5f, (dy * +1) + 1f);
                    }
                } else {
                    int x = (int)(path_finding.path[i].position.x - dx / 2);
                    int y = (int)path_finding.path[i].position.y;
                    if (dx < 0) {
                        // left 
                        add_road(x, y, (dx * -1) + 1, 0.5f);
                    } else {
                        // right
                        add_road(x, y, (dx * +1));
                    }
                }
            }
        }
    }
    void add_road(int x, int y, float dx = 0.5f, float dy = 0.5f) {
        GameObject lane = Instantiate(road);
        lane.transform.position = set_position(x, y);
        lane.transform.localScale = new Vector3(dx, dy, 0f);
        draw_road.Add(lane);
    }
}
