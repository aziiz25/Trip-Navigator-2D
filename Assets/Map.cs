using System;
using System.Collections.Generic;
using UnityEngine;

public class Map {
    public MapNode start { get; set; }
    public MapNode end { get; set; }
    public List<MapNode> nodes { get; set; }



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
                        var weight = current.position.x - tempLeft.position.x;
                        if (gridValues[x - 1][y].Length == 1) {
                            current.left = tempLeft;
                            current.leftCost = weight;
                            tempLeft.right = current;
                            tempLeft.rightCost = weight;

                        } else if (gridValues[x - 1][y][1] == 'L') {
                            current.left = tempLeft;
                            current.leftCost = weight;
                        } else {
                            tempLeft.right = current;
                            tempLeft.rightCost = weight;
                        }
                    }
                    if (tempDown != null) {
                        var weight = current.position.y - tempDown.position.y;
                        if (gridValues[x][y - 1].Length == 1) {
                            current.down = tempDown;
                            current.downCost = weight;

                            tempDown.up = current;
                            tempDown.upCost = weight;
                        } else if (gridValues[x][y - 1][1] == 'D') {
                            current.down = tempDown;
                            current.downCost = weight;
                        } else {
                            tempDown.up = current;
                            tempDown.upCost = weight;
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