using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class DrawTraffic : MonoBehaviour {
    // Start is called before the first frame update



    public System.Random random;
    public List<MapNode> path;


    GameObject traffic;

    float multiplier = 2f;


    public Line line;


    public GridArea grid;

    public TestScript script;

    PathFollower car;

    public bool isTraffic;
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
                path = grid.path;
                StartCoroutine(traffic_delay());
            }
        }
    }

    public IEnumerator traffic_delay() {
        yield return new WaitForSeconds(2);
        add_traffic();
    }

    public void add_traffic() {
        if (traffic != null || path.Count <= 2) {
            return;
        }
        int index = random.Next(3, path.Count);
        int direction = random.Next(0, 4); // 0 == top, 1 == bottom, 2 == right, 3 == left
        float cost = 0;
        MapNode node_dir;
        if (direction == 0) {
            MapNode up = path[index].up;
            if (up != null) {
                cost = path[index].upCost + path[index].upCost * multiplier + 500000;
                traffic = line.DrawLine(path[index].position, up.position, Color.red, 1);
                node_dir = up;
            }
        } else if (direction == 1) {
            MapNode down = path[index].down;
            if (down != null) {
                cost = path[index].downCost + path[index].downCost * multiplier + 500000;
                traffic = line.DrawLine(path[index].position, down.position, Color.red, 1);
                node_dir = down;
            }
        } else if (direction == 2) {
            MapNode right = path[index].right;
            if (right != null) {
                cost = path[index].rightCost + path[index].rightCost * multiplier + 500000;
                traffic = line.DrawLine(path[index].position, right.position, Color.red, 1);
                node_dir = right;
            }
        } else {
            MapNode left = path[index].left;
            if (left != null) {
                cost = path[index].leftCost + path[index].leftCost * multiplier + 500000;
                traffic = line.DrawLine(path[index].position, left.position, Color.red, 1);
                node_dir = left;
            }
        }
        car = GameObject.FindWithTag(("Grid")).GetComponent<PathFollower>();
        if (car.path.Count > 1 && traffic != null) {
            grid.a_star.update_node_costs(this.path[index], cost, direction);
            List<MapNode> path = grid.a_star.find(car.path[car.currentWayPoint + 1]);
            add_to_list(path);
            path = grid.a_star.cleanPath(path);
            this.path = path;
            this.path.Reverse();
            isTraffic = true;
        }
    }

    public void add_to_list(List<MapNode> new_path) {
        for (int i = car.currentWayPoint; i >= 0; i--) {
            new_path.Add(this.path[i]);
        }
    }
}

