using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class Astar {

    public MapNode start;
    public MapNode end;

    public Map map;

    List<KeyValuePair<MapNode, KeyValuePair<MapNode, double>>> open_queue = new List<KeyValuePair<MapNode, KeyValuePair<MapNode, double>>>();
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
        if(check_neighbours()){
            return;
        }
        foreach (MapNode node in map.nodes) {
            create_pair(node);
        }
        foreach (KeyValuePair<MapNode, KeyValuePair<MapNode, Double>> node in open_queue) {
            if (node.Key == this.start) {
                open_queue.Remove(node);
                closed.Add(new KeyValuePair<MapNode, MapNode>(node.Key, null));
                break;
            }
        }
        while (open_queue.Count > 0 && closed[closed.Count - 1].Key != end) {
            update_adj_nodes();
            open_queue = open_queue.OrderBy(o => o.Value.Value).ToList();
            add_to_close();
        }
        get_optimal_path();

    }

    public void update_adj_nodes() {
        foreach (KeyValuePair<MapNode, KeyValuePair<MapNode, Double>> node in open_queue.ToList()) {
            MapNode adj_node = closed[closed.Count - 1].Key;
            if (!node.Key.isVisited) {
                if (node.Key == closed[closed.Count - 1].Key.up) {
                    double cost = huristic(adj_node , prev_cost + adj_node.upCost);
                    if (prev_cost + closed[closed.Count - 1].Key.upCost < node.Value.Value) {
                        create_pair(node.Key, closed[closed.Count - 1].Key, prev_cost + closed[closed.Count - 1].Key.upCost);
                        open_queue.Remove(node);
                    }
                }
                if (node.Key == closed[closed.Count - 1].Key.down) {
                    double cost = huristic(adj_node , prev_cost + adj_node.downCost);
                    if (cost < node.Value.Value) {
                        create_pair(node.Key, closed[closed.Count - 1].Key, prev_cost + closed[closed.Count - 1].Key.downCost);
                        open_queue.Remove(node);
                    }
                }
                if (node.Key == closed[closed.Count - 1].Key.right) {
                    double cost = huristic(adj_node , prev_cost + adj_node.rightCost);
                    if (cost < node.Value.Value) {
                        create_pair(node.Key, closed[closed.Count - 1].Key, prev_cost + closed[closed.Count - 1].Key.rightCost);
                        open_queue.Remove(node);
                    }
                }
                if (node.Key == closed[closed.Count - 1].Key.left) {
                    double cost = huristic(adj_node , prev_cost + adj_node.leftCost);
                    if (cost < node.Value.Value) {
                        create_pair(node.Key, closed[closed.Count - 1].Key, prev_cost + closed[closed.Count - 1].Key.leftCost);
                        open_queue.Remove(node);
                    }
                }
            }
        }
        closed[closed.Count - 1].Key.isVisited = true;
    }

    public bool add_to_close() {
        foreach (KeyValuePair<MapNode, KeyValuePair<MapNode, Double>> node in open_queue.ToList()) {
            foreach (KeyValuePair<MapNode, MapNode> closed_node in closed.ToList()) {
                if (closed_node.Key == node.Value.Key) {
                    closed.Add(new KeyValuePair<MapNode, MapNode>(node.Key, closed_node.Key));
                    prev_cost = node.Value.Value;
                    open_queue.Remove(node);
                    return true;
                }
            }
        }
        return false;
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
    public void create_pair(MapNode node, MapNode from = null, double cost = 1000000) {
        KeyValuePair<MapNode, double> node_cost = new KeyValuePair<MapNode, double>(from, cost);
        KeyValuePair<MapNode, KeyValuePair<MapNode, Double>> q = new KeyValuePair<MapNode, KeyValuePair<MapNode, Double>>(node, node_cost);
        open_queue.Add(q);
    }
    public double huristic(MapNode node, double cost) {
        double dx = Math.Pow(node.position.x - end.position.x, 2);
        double dy = Math.Pow(node.position.y - end.position.y,2);
        // 4 for the fastest road 
        return cost + (Math.Sqrt(dx+dy))/4;
    }

    private void cleanPath() {
        if (path.Count < 3) {
            return;
        }
        for (int i = 1; i < path.Count - 1; i++) {
            MapNode prev = path[i - 1];
            MapNode curr = path[i];
            MapNode next = path[i + 1];
            if (prev.position.x == next.position.x ||
                prev.position.y == next.position.y) {
                path.RemoveAt(i);
                i--;
            }
        }
    }

    public List<MapNode> get_path() {
        return path;
    }

    public bool check_neighbours(){
        if(start.up == null && start.down == null && start.right == null && start.left == null){
            return true;
            }
            return false;
    }
}
