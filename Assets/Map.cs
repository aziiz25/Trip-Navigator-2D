using System;
using System.Collections.Generic;
using UnityEngine;

public class Map {
    public MapNode start { get; set; }
    public MapNode end { get; set; }
    public List<MapNode> nodes { get; set; }

    float speedFactor = 3f;


    public Map(string[][] gridValues, Vector3 start, Vector3 end) {
        nodes = new List<MapNode>();
        for (int x = 0; x < gridValues.Length; x++) {
            for (int y = 0; y < gridValues[x].Length; y++) {
                if (gridValues[x][y].Equals("1") || x == start.x && y == start.y || x == end.x && y == end.y) {
                    var current = new MapNode(new Vector3(x, y, 0));
                    if (current.position.x == start.x && current.position.y == start.y) {
                        this.start = current;
                    } else if (current.position.x == end.x && current.position.y == end.y) {
                        this.end = current;
                    }

                    MapNode tempLeft = null;
                    MapNode tempDown = null;
                    if (x - 1 >= 0 && !gridValues[x - 1][y].Equals("0"))
                        tempLeft = getLeft(x, y);
                    if (y - 1 >= 0 && !gridValues[x][y - 1].Equals("0"))
                        tempDown = getDown(x, y);
                    if (tempLeft != null) {
                        float distance = current.position.x - tempLeft.position.x;
                        float speed = float.Parse(gridValues[x - 1][y].Substring(0, 1)) * speedFactor;
                        float time = distance / speed;
                        if (gridValues[x - 1][y].Length == 1) {
                            current.left = tempLeft;
                            current.leftCost = time;
                            current.leftSpeed = speed;
                            tempLeft.right = current;
                            tempLeft.rightCost = time;
                            tempLeft.rightSpeed = speed;
                        } else if (gridValues[x - 1][y][1] == 'L') {
                            current.left = tempLeft;
                            current.leftCost = time;
                            current.leftSpeed = speed;
                        } else {
                            tempLeft.right = current;
                            tempLeft.rightCost = time;
                            tempLeft.rightSpeed = speed;
                        }
                    }
                    if (tempDown != null) {
                        float distance = current.position.y - tempDown.position.y;
                        float speed = float.Parse(gridValues[x][y - 1].Substring(0, 1)) * speedFactor;
                        float time = distance / speed;
                        if (gridValues[x][y - 1].Length == 1) {
                            current.down = tempDown;
                            current.downCost = time;
                            current.downSpeed = speed;
                            tempDown.up = current;
                            tempDown.upCost = time;
                            tempDown.upSpeed = speed;
                        } else if (gridValues[x][y - 1][1] == 'D') {
                            current.down = tempDown;
                            current.downCost = time;
                            current.downSpeed = speed;
                        } else {
                            tempDown.up = current;
                            tempDown.upCost = time;
                            tempDown.upSpeed = speed;
                        }
                    }
                    nodes.Add(current);
                }
            }
        }
    }

    private MapNode getLeft(int x, int y) {
        MapNode temp = null;
        foreach (MapNode node in nodes) {
            if (node.position.y == y) {
                if (temp == null)
                    temp = node;
                else if (node.position.x > temp.position.x)
                    temp = node;
            }
        }
        return temp;
    }

    private MapNode getDown(int x, int y) {
        MapNode temp = null;
        foreach (MapNode node in nodes) {
            if (node.position.x == x) {
                if (temp == null)
                    temp = node;
                else if (node.position.y > temp.position.y)
                    temp = node;
            }
        }
        return temp;
    }

    public void printGraph() {
        printGraph(nodes[0]);
    }
    public void printGraph(MapNode node) {
        if (node == null || node.isVisited == true) {
            return;
        } else {
            Debug.Log(node.position.x + " " + node.position.y);
            node.isVisited = true;
            printGraph(node.up);
            printGraph(node.left);
            printGraph(node.down);
            printGraph(node.right);
        }
    }
    public void markUnvisited() {
        foreach (MapNode node in nodes) {
            node.isVisited = false;
        }
    }
}