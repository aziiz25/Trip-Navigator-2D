using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class DrawTraffic : MonoBehaviour {
    // Start is called before the first frame update



    public System.Random random;
    List<MapNode> nodes;


    GameObject traffic;

    float multiplier = 2f;


    public Line line;


    public GridArea grid;

    public TestScript script;

    PathFollower car;
    void Start() {
        if (GameObject.FindWithTag(("Grid")) != null) {
            script = GameObject.FindWithTag(("Grid")).GetComponent<TestScript>();
        }
        random = new System.Random();
    }

    // Update is called once per frame

    void Update() {
        if (this.grid == null) {
            this.grid = script.grid;
            line = new Line(grid);
        }
        if (grid != null) {
            // i want the square to appear after the path is found
            if (grid.path != null) {
                nodes = grid.path;
                StartCoroutine(traffic_delay());
            }
        }
    }

    public IEnumerator traffic_delay() {
        yield return new WaitForSeconds(2);
        add_traffic();
    }

    public void add_traffic() {
        if (traffic != null) {
            return;
        }
        int index = random.Next(0, nodes.Count);
        int direction = random.Next(0, 4); // 0 == top, 1 == bottom, 2 == right, 3 == left
        float cost = 0;
        if (direction == 0) {
            MapNode up = nodes[index].up;
            if (up != null) {
                cost = nodes[index].upCost + nodes[index].upCost * multiplier + 500000;
                traffic = line.DrawLine(nodes[index].position, up.position, Color.red, 1);
            }
        } else if (direction == 1) {
            MapNode down = nodes[index].down;
            if (down != null) {
                cost = nodes[index].downCost + nodes[index].downCost * multiplier + 500000;
                traffic = line.DrawLine(nodes[index].position, down.position, Color.red, 1);
            }
        } else if (direction == 2) {
            MapNode right = nodes[index].right;
            if (right != null) {
                cost = nodes[index].rightCost + nodes[index].rightCost * multiplier + 500000;
                traffic = line.DrawLine(nodes[index].position, right.position, Color.red, 1);
            }
        } else {
            MapNode left = nodes[index].left;
            if (left != null) {
                cost = nodes[index].leftCost + nodes[index].leftCost * multiplier + 500000;
                traffic = line.DrawLine(nodes[index].position, left.position, Color.red, 1);
            }
        }
        car = GameObject.FindWithTag(("Grid")).GetComponent<PathFollower>();
        if (car.path.Count - car.currentWayPoint > 1 && traffic != null) {
            grid.a_star.update_node_costs(nodes[index], cost, direction);
            List<MapNode> path = grid.a_star.find(nodes[car.currentWayPoint - 1]);
            nodes = path;
            path.Reverse();
            car.change_path(path, car.currentWayPoint);
        }
    }
}

