using System;
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


    public List<List<MapNode>> paths;


    public Astar(Map map) {
        path = new List<MapNode>();
        this.start = map.start;
        this.end = map.end;
        this.map = map;
        path = find(start);
        if (path != null) {
            path.Reverse();
            paths = yenKSP();
            cleanPath();
        } else {
            throw new InvalidOperationException();
        }
    }

    public List<MapNode> find(MapNode start) {
        if (start.position == end.position) {
            List<MapNode> t = new List<MapNode>();
            t.Add(end);
            t.Add(start);
            return t;
        }

        prev_cost = 0;
        this.map.markUnvisited();
        // start from draw traffic has diff ref ---: :: so to get the currect ref i did this
        start = map.nodes.Find(node => node.position == start.position);

        if (check_neighbours(start)) {
            return null;
        }
        open_queue = new PriorityQueue();

        closed = new List<KeyValuePair<MapNode, MapNode>>();
        foreach (MapNode node in map.nodes) {
            create_pair(node);
        }
        Nodes temp = open_queue.Dequeue(start);
        closed.Add(new KeyValuePair<MapNode, MapNode>(temp.to, temp.from));
        while (open_queue.size > 0 && closed[closed.Count - 1].Key.position != end.position) {
            int size = open_queue.size;
            update_adj_nodes();
            add_to_close();
            if (size == open_queue.size || check_neighbours(closed[closed.Count - 1].Key)) {
                return null;
            }
        }
        return get_optimal_path(start);
    }

    public void update_adj_nodes() {
        MapNode adj_node = closed[closed.Count - 1].Key;
        foreach (Nodes node in open_queue.get_Queue()) {
            if (!node.to.isVisited) {
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
    public List<MapNode> get_optimal_path(MapNode start) {
        List<MapNode> path = new List<MapNode>();
        path.Add(closed[closed.Count - 1].Key);
        MapNode prev = closed[closed.Count - 1].Value;
        closed.Remove(closed[closed.Count - 1]);
        while (prev != null) {
            foreach (KeyValuePair<MapNode, MapNode> node in closed.ToList()) {
                if (prev == node.Key) {
                    path.Add(node.Key);
                    prev = node.Value;
                    closed.Remove(node);
                }
            }
        }
        //path.Add(prev);
        return path;
    }


    public void update_node_costs(MapNode node, float cost, double location) {
        MapNode node_to_update = map.nodes.Find(n => n.position == node.position);
        if (location == 0) {
            node.upCost = cost;
            node.up.downCost = cost;
            node.upSpeed -= node_to_update.upSpeed * 0.5f;
            node.up.downSpeed -= node_to_update.downSpeed * 0.5f;

            node_to_update.upCost = cost;
            node_to_update.up.downCost = cost;
            node_to_update.upSpeed -= node_to_update.upSpeed * 0.5f;
            node_to_update.up.downSpeed -= node_to_update.downSpeed * 0.5f;
        } else if (location == 1) {
            node.downCost = cost;
            node.down.upCost = cost;
            node.downSpeed -= node_to_update.downSpeed * 0.5f;
            node.down.upSpeed -= node_to_update.down.upSpeed * 0.5f;

            node_to_update.downCost = cost;
            node_to_update.down.upCost = cost;
            node_to_update.downSpeed -= node_to_update.downSpeed * 0.5f;
            node_to_update.down.upSpeed -= node_to_update.down.upSpeed * 0.5f;
        } else if (location == 2) {
            node.rightCost = cost;
            node.right.leftCost = cost;
            node.rightSpeed -= node_to_update.rightSpeed * 0.5f;
            node.right.leftSpeed -= node_to_update.right.leftSpeed * 0.5f;

            node_to_update.rightCost = cost;
            node_to_update.right.leftCost = cost;
            node_to_update.rightSpeed -= node_to_update.rightSpeed * 0.5f;
            node_to_update.right.leftSpeed -= node_to_update.right.leftSpeed * 0.5f;
        } else {
            node.leftCost = cost;
            node.left.rightCost = cost;
            node.leftSpeed -= node_to_update.leftSpeed * 0.5f;
            node.left.rightSpeed -= node_to_update.left.rightSpeed * 0.5f;

            node_to_update.leftCost = cost;
            node_to_update.left.rightCost = cost;
            node_to_update.leftSpeed -= node_to_update.leftSpeed * 0.5f;
            node_to_update.left.rightSpeed -= node_to_update.left.rightSpeed * 0.5f;
        }
    }

    public void create_pair(MapNode node, MapNode from = null, double g_cost = 1000000, double f_cost = 1000000) {
        open_queue.Enqueue(node, from, g_cost, f_cost);
    }

    public double total_cost(MapNode node, double cost) {
        double dx = Math.Abs(node.position.x - end.position.x);
        double dy = Math.Abs(node.position.y - end.position.y);
        // 4 is the max speed
        return cost + ((dx + dy) / (map.speedFactor * 4));
    }

    public List<MapNode> cleanPath(List<MapNode> path) {
        if (path.Count < 3) {
            return path;
        }
        for (int i = 1; i < path.Count - 1; i++) {
            MapNode prev = path[i - 1];
            MapNode curr = path[i];
            MapNode next = path[i + 1];
            /*if (prev.position.x == next.position.x || prev.position.y == next.position.y) {
                path.RemoveAt(i);
                i--;
            }*/
            if ((prev.position.x == next.position.x) && (prev.position.y > next.position.y)){
                prev.downCost = prev.downCost + curr.downCost;
                path.RemoveAt(i);
                i--;
            } else if ((prev.position.x == next.position.x) && (prev.position.y <= next.position.y)){
                prev.upCost = prev.upCost + curr.upCost;
                path.RemoveAt(i);
                i--;
            } else if ((prev.position.x > next.position.x) && (prev.position.y == next.position.y)){
                prev.leftCost = prev.leftCost + curr.leftCost;
                path.RemoveAt(i);
                i--;
            } else if ((prev.position.x <= next.position.x) && (prev.position.y == next.position.y)){
                prev.rightCost = prev.rightCost + curr.rightCost;
                path.RemoveAt(i);
                i--;
            }
        }
        return path;
    }


    private void cleanPath() {
        paths.ForEach(path => cleanPath(path));
    }

    public List<MapNode> get_path() {
        return path;
    }

    public List<List<MapNode>> get_paths() {
        return paths;
    }
    

    public bool check_neighbours(MapNode start) {
        if (start.up == null && start.down == null && start.right == null && start.left == null) {
            return true;
        }
        return false;
    }

    public List<List<MapNode>> yenKSP() {
        List<List<MapNode>> paths = new List<List<MapNode>>();
        paths.Add(path);
        List<KeyValuePair<List<MapNode>, double>> k_pot_paths = new List<KeyValuePair<List<MapNode>, double>>();
        int number_of_paths = 2;
        for (int k = 1; k < number_of_paths; k++) {
            for (int i = 0; i < paths[k - 1].Count() - 2; i++) {
                MapNode spurNode = get_sput_new_ref(paths[k - 1][i]);
                List<MapNode> root_path = get_node_from(paths[k - 1], 0, i);
                foreach (List<MapNode> path in paths) {
                    if (equal_path(root_path, get_node_from(path, 0, i))) {
                        remove_edges(path[i], path[i + 1]);
                    }
                }
                remove_from_graph(root_path, spurNode);
                List<MapNode> spur_path = find(spurNode);
                if (spur_path == null) {
                    this.map = new Map(map.gridValues, this.start.position, this.end.position);
                    continue;
                }
                spur_path.Reverse();
                List<MapNode> total_path = get_total_path(root_path, spur_path);
                if (!if_path_exist(k_pot_paths, total_path)) {
                    k_pot_paths.Add(new KeyValuePair<List<MapNode>, double>(total_path, prev_cost));
                    k_pot_paths = k_pot_paths.OrderBy(path => path.Value).ToList();
                }
                this.map = new Map(map.gridValues, this.start.position, this.end.position);

            }
            if (k_pot_paths.Count() == 0) {
                break;
            }
            paths.Add(k_pot_paths[0].Key);
            k_pot_paths.Remove(k_pot_paths[0]);
        }
        return paths;
    }

    public List<MapNode> get_total_path(List<MapNode> root_path, List<MapNode> spur_path) {
        root_path.AddRange(spur_path);
        return root_path;
    }
    public List<MapNode> get_node_from(List<MapNode> path, int i, int j) {
        if (j >= path.Count()) {
            return null;
        }
        List<MapNode> nodes = new List<MapNode>();
        for (; i <= j; i++) {
            nodes.Add(path[i]);
        }
        return nodes;
    }


    public void remove_edges(MapNode node, MapNode next_node) {
        MapNode temp_node = map.nodes.Find(n => node.position == n.position);
        MapNode temp_next_node = map.nodes.Find(n => next_node.position == n.position);
        if (temp_node != null) {
            node = temp_node;
        }
        if (temp_next_node != null) {
            next_node = temp_next_node;
        }
        if (node.up != null && node.up.position == next_node.position) {
            node.up = null;
            next_node.down = null;
        }
        if (node.down != null && node.down.position == next_node.position) {
            node.down = null;
            next_node.up = null;
        }
        if (node.right != null && node.right.position == next_node.position) {
            node.right = null;
            next_node.left = null;
        }
        if (node.left != null && node.left.position == next_node.position) {
            node.left = null;
            next_node.right = null;
        }
    }



    public bool equal_path(List<MapNode> path1, List<MapNode> path2) {
        if (path1 == null || path2 == null || path1.Count != path2.Count) {
            return false;
        }
        for (int i = 0; i < path1.Count; i++) {
            if (path1[i].position != path2[i].position) {
                return false;
            }
        }
        return true;
    }

    public void remove_from_graph(List<MapNode> root_path, MapNode spur_node) {
        foreach (MapNode node in root_path) {
            if (node.position == spur_node.position) {
                continue;
            }
            map.nodes.Remove(map.nodes.Find(n => node.position == n.position));
        }
    }

    public bool if_path_exist(List<KeyValuePair<List<MapNode>, double>> k_pot_paths, List<MapNode> path) {
        bool check = false;
        foreach (KeyValuePair<List<MapNode>, double> pot_path in k_pot_paths) {
            if (pot_path.Key.Count != path.Count) {
                continue;
            } else {
                for (int i = 0; i < path.Count; i++) {
                    if (pot_path.Key[i].position == path[i].position) {
                        check = true;
                    } else {
                        check = false;
                        break;
                    }
                }
                if (check) {
                    return check;
                }
            }
        }
        return check;
    }

    public MapNode get_sput_new_ref(MapNode spur_node) {
        foreach (MapNode node in this.map.nodes) {
            if (node.position == spur_node.position) {
                return node;
            }
        }
        return null;
    }
}

