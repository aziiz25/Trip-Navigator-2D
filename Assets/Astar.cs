using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class Astar{

    MapNode start;
    MapNode end;

    public Map map;

    List<KeyValuePair<MapNode,double>> nodes_with_cost;

    List<MapNode> path;
    public Astar(Map map){
        path = new List<MapNode>();
        this.start = map.start;
        this.end = map.end;
        this.map = map;
        nodes_with_cost = new List<KeyValuePair<MapNode,double>>();
        find_path();
        foreach(KeyValuePair<MapNode,double> node in nodes_with_cost){
            Debug.Log(node.Key+" -------------- "+ node.Value);
        }
        nodes_with_cost = nodes_with_cost.OrderBy(o => o.Value).ToList();

        map.markUnvisited();
        get_optimal_path(this.start);
        foreach(MapNode node in path){
            Debug.Log(node.position.x + " : " + node.position.y);
        }

    }


    public Double claculate_distence(MapNode node,MapNode end){
        double x_postion = Math.Pow((node.position.x - end.position.x),2);
        double y_postion = Math.Pow((node.position.y - end.position.y),2);
        return Math.Sqrt(x_postion + y_postion);
    }

    public double total_cost(MapNode node){
        return get_cost(node) + claculate_distence(node ,this.end);
    }

    public float get_cost(MapNode node){
        return get_cost(this.start, node);
    }

    // cost * steps; 
    public float get_cost(MapNode node, MapNode value, float steps = 0){
        if(node == null || node.isVisited){
            return 0;
        }
        if(node.up != null && node.up.position.x == value.position.x && node.up.position.y == value.position.y){
            value.isVisited = true;
            return (steps + 1) * node.upCost;
        }
        if(node.down != null && node.down.position.x == value.position.x && node.down.position.y == value.position.y){
            value.isVisited = true;
            return (steps + 1) * node.downCost;
        }
        if(node.left != null && node.left.position.x == value.position.x && node.left.position.y == value.position.y){
            value.isVisited = true;
            return (steps + 1) * node.leftCost;
        }
        if(node.right != null && node.right.position.x == value.position.x && node.right.position.y == value.position.y){
            value.isVisited = true;
            return (steps + 1) * node.rightCost;
        }
        node.isVisited = true;
        return get_cost(node.up,value, steps + 1)+
        get_cost(node.down,value, steps + 1)+
        get_cost(node.left,value, steps + 1)+
        get_cost(node.right,value, steps + 1);
    }

    public void find_path(){
        map.markUnvisited();
        find_path(this.start);
    }
    public void find_path(MapNode node){
        if(node == null || node.isVisited){
            return;
        }
        node.isVisited = true;
        KeyValuePair<MapNode,double> pair = new KeyValuePair<MapNode,double>(node,total_cost(node));
        nodes_with_cost.Add(pair);
        find_path(node.up);
        find_path(node.left);
        find_path(node.down);
        find_path(node.right);
    }

    public List<MapNode> get_path(){
        return path;
    }

    public void get_optimal_path(MapNode node){ 
        if(node == this.end){
            path.Add(node); 
        }else if (node == null){
            path = null;
        }else{
            foreach(KeyValuePair<MapNode,double> next_node in nodes_with_cost){
                //Debug.Log(node.Key+" -------------- "+ node.Value);
                if(!next_node.Key.isVisited){
                    if(next_node.Key == node.up){
                        path.Add(next_node.Key);
                        next_node.Key.isVisited = true;
                        get_optimal_path(node.up);
                    }else if(next_node.Key == node.down){
                        path.Add(next_node.Key);
                        next_node.Key.isVisited = true;
                        get_optimal_path(node.down);
                    }else if(next_node.Key == node.right){
                        next_node.Key.isVisited = true;
                        path.Add(next_node.Key);
                        get_optimal_path(node.right);
                    } else if(next_node.Key == node.left){
                        next_node.Key.isVisited = true;
                        path.Add(next_node.Key);
                        get_optimal_path(node.left);
                    }
                }
            }
        }
    }
}
