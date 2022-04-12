using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Astar {
    public MapNode start;

    public MapNode end;

    public Map map;

    PriorityQueue open_queue = new PriorityQueue();

    List<KeyValuePair<MapNode, MapNode>> closed = new List<KeyValuePair<MapNode, MapNode>>();

    public List<MapNode> path;

    double prev_cost = 0;

    public Astar(Map map) {
        path = new List<MapNode>();
        this.start = map.start;
        this.end = map.end;
        this.map = map;
        find();
        path.Reverse();
        cleanPath();
    }

    public void find() {
        if (check_neighbours()) {
            return;
        }
        foreach (MapNode node in map.nodes) {
            create_pair(node);
        }

        Nodes temp = open_queue.Dequeue(this.start);
        closed.Add(new KeyValuePair<MapNode, MapNode>(temp.to, temp.from));

        while (open_queue.size > 0 && closed[closed.Count - 1].Key != end) {
            update_adj_nodes();
            add_to_close();
        }
        get_optimal_path();
    }

    public void update_adj_nodes() {
        foreach (Nodes node in open_queue.get_Queue()) {
            if (!node.to.isVisited) {
                MapNode adj_node = closed[closed.Count - 1].Key;
                if (node.to == adj_node.up) {
                    if (prev_cost + adj_node.upCost < node.g_cost) {
                        double g_cost = prev_cost + adj_node.upCost;
                        create_pair(node.to, adj_node, g_cost, total_cost(adj_node.up, g_cost));
                        open_queue.Dequeue(node);
                    }
                }
                if (node.to == adj_node.down) {
                    if (prev_cost + adj_node.downCost < node.g_cost) {
                        double g_cost = prev_cost + adj_node.downCost;
                        create_pair(node.to, adj_node, g_cost, total_cost(adj_node.down, g_cost));
                        open_queue.Dequeue(node);
                    }
                }
                if (node.to == adj_node.right) {
                    if (prev_cost + adj_node.rightCost < node.g_cost) {
                        double g_cost = prev_cost + adj_node.rightCost;
                        create_pair(node.to, adj_node, g_cost, total_cost(adj_node.right, g_cost));
                        open_queue.Dequeue(node);
                    }
                }
                if (node.to == adj_node.left) {
                    if (prev_cost + adj_node.leftCost < node.g_cost) {
                        double g_cost = prev_cost + adj_node.leftCost;
                        create_pair(node.to, adj_node, g_cost, total_cost(adj_node.left, g_cost));
                        open_queue.Dequeue(node);
                    }
                }
            }
        }
        closed[closed.Count - 1].Key.isVisited = true;
    }

    public void add_to_close() {
        foreach (Nodes node in open_queue.get_Queue()) {
            foreach (KeyValuePair<MapNode, MapNode> closed_node in closed.ToList()) {
                if (closed_node.Key == node.from) {
                    closed.Add(new KeyValuePair<MapNode, MapNode>(node.to, closed_node.Key));
                    prev_cost = node.g_cost;
                    open_queue.Dequeue(node);
                    return;
                }
            }
        }
    }

    public void get_optimal_path() {
        path.Add(closed[closed.Count - 1].Key);
        MapNode prev = closed[closed.Count - 1].Value;
        closed.Remove(closed[closed.Count - 1]);
        while (prev != start) {
            foreach (KeyValuePair<MapNode, MapNode> node in closed.ToList()) {
                if (prev == node.Key) {
                    path.Add(node.Key);
                    prev = node.Value;
                    closed.Remove(node);
                }
            }
        }
        path.Add(prev);
    }

    public void create_pair(MapNode node, MapNode from = null, double g_cost = 1000000, double f_cost = 1000000) {
        open_queue.Enqueue(node, from, g_cost, f_cost);
    }

    public double total_cost(MapNode node, double cost) {
        double dx = Math.Abs(node.position.x - end.position.x);
        double dy = Math.Abs(node.position.y - end.position.y);
        // 4 is the max speed
        return cost + (dx + dy) / (map.speedFactor * 4);
    }

    private void cleanPath() {
        if (path.Count < 3) {
            return;
        }
        for (int i = 1; i < path.Count - 1; i++) {
            MapNode prev = path[i - 1];
            MapNode curr = path[i];
            MapNode next = path[i + 1];
            if (
                prev.position.x == next.position.x ||
                prev.position.y == next.position.y
            ) {
                path.RemoveAt(i);
                i--;
            }
        }
    }

    public List<MapNode> get_path() {
        return path;
    }

    public bool check_neighbours() {
        if (start.up == null && start.down == null && start.right == null && start.left == null) {
            return true;
        }
        return false;
    }
}
