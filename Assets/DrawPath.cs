using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DrawPath : Line {


    public List<GameObject> draw_road;

    List<MapNode> path_finding;
    // Start is called before the first frame update
    new void Start() {
        base.Start();
        draw_road = new List<GameObject>();
    }

    new void Update() {
        base.Update();
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


    void draw() {
        if (path_finding != null) {
            for (int i = 0; i < path_finding.Count - 1; i++) {
                Vector3 start = path_finding[i].position;;
                Vector3 end = path_finding[i + 1].position;
                GameObject line = base.DrawLine(start, end, Color.green);
                draw_road.Add(line);
            }
        }
    }

}

