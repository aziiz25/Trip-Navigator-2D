using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class DrawTraffic : Line {
    // Start is called before the first frame update



    public System.Random random;
    List<MapNode> nodes;


    GameObject traffic;

    float multiplier = 0.25f;


    new void Start() {
        base.Start();
        random = new System.Random();
    }

    // Update is called once per frame

    // new is like the annotation @override :)
    new void Update() {
        base.Update();
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
        if (traffic == null) {
            int index = random.Next(0, nodes.Count);
            int direction = random.Next(0, 4); // 0 == top, 1 == bottom, 2 == right, 3 == left
            if (direction == 0) {
                MapNode up = nodes[index].up;
                if (up != null) {
                    nodes[index].upCost += nodes[index].upCost * multiplier;
                    traffic = DrawLine(nodes[index].position, up.position, Color.red,1);
                }
            } else if (direction == 1) {
                MapNode down = nodes[index].down;
                if (down != null) {
                    nodes[index].downCost += nodes[index].downCost * multiplier;
                    traffic = DrawLine(nodes[index].position, down.position, Color.red,1);
                }
            } else if (direction == 2) {
                MapNode right = nodes[index].right;
                if (right != null) {
                    nodes[index].rightCost += nodes[index].rightCost * multiplier;
                    traffic = DrawLine(nodes[index].position, right.position, Color.red, 1);
                }
            } else {
                MapNode left = nodes[index].left;
                if (left != null) {
                    nodes[index].leftCost += nodes[index].leftCost * multiplier;
                    traffic = DrawLine(nodes[index].position, left.position, Color.red,1);
                }
            }
        }
        if (traffic != null) {
            traffic.layer = 1;
        }
    }
}

