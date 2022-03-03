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
        nodes_with_cost = nodes_with_cost.OrderBy(o => o.Value).ToList();
        get_optimal_path();
        foreach (var node in path){
            Debug.Log("path x: " + node.position.x + " path y: " + node.position.y);
        }
    }



    public double total_cost(MapNode node){
        double dx1 = node.position.x -  end.position.x;
        double dy1 = node.position.y -  end.position.y;
        double dx2 =  start.position.x -  end.position.x;
        double dy2 =  start.position.y -  end.position.y;
        double cross = Math.Abs(dx1*dx2 - dy2*dy1);
        return cross * 0.001;
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
        if(node != start){
        KeyValuePair<MapNode,double> pair = new KeyValuePair<MapNode,double>(node ,total_cost(node));
        nodes_with_cost.Add(pair);
        }else{
        KeyValuePair<MapNode,double> pair = new KeyValuePair<MapNode,double>(node ,total_cost(node));
        nodes_with_cost.Add(pair);          
        }
        find_path(node.up);
        find_path(node.left);
        find_path(node.down);
        find_path(node.right);
    }

    public List<MapNode> get_path(){
        return path;
    }


    public void get_optimal_path(){
        map.markUnvisited();
        get_optimal_path(this.start);
    }

    public void get_optimal_path(MapNode node){ 
        if(node.position.x == this.end.position.x && node.position.y == this.end.position.y){
            node.isVisited = true;
            path.Add(node);
        }else if (node == null){
            path = null;
        }else{
            foreach(KeyValuePair<MapNode,double> next_node in nodes_with_cost){
                if(!next_node.Key.isVisited && !end.isVisited){
                    if(next_node.Key == node.up){
                        path.Add(node);
                        node.isVisited = true;
                        get_optimal_path(node.up);
                    }else if(next_node.Key == node.down){
                        path.Add(node);
                        node.isVisited = true;
                        get_optimal_path(node.down);
                    }else if(next_node.Key == node.right){
                        node.isVisited = true;
                        path.Add(node);
                        get_optimal_path(node.right);
                    } else if(next_node.Key == node.left){
                        node.isVisited = true;
                        path.Add(node);
                        get_optimal_path(node.left);
                    }
                }
            }
        }
    }
}
