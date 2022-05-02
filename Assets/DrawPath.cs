using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DrawPath {


    public List<GameObject> draw_road;

    public Line line;

    public List<MapNode> path;

    Color color;


    public DrawPath(List<MapNode> path, Color color, GridArea grid) {
        draw_road = new List<GameObject>();
        this.path = path;
        this.color = color;
        line = new Line(grid);
        draw();
    }



    void draw() {
        for (int i = 0; i < path.Count - 1; i++) {
            Vector3 start = path[i].position; ;
            Vector3 end = path[i + 1].position;
            GameObject line = this.line.DrawLine(start, end, color);
            draw_road.Add(line);
        }
    }

    public void destroy_road() {
        foreach (GameObject road in draw_road) {
            GameObject.Destroy(road);
        }
        draw_road.Clear();
    }

}

