using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class PriorityQueue {

    List<Nodes> elements;

    public int size = 0;

    public PriorityQueue() {
        elements = new List<Nodes>();
    }

    public void Enqueue(MapNode to, MapNode from, double g_cost, double f_cost) {
        elements.Add(new Nodes(to, from, f_cost, g_cost));
        elements = elements.OrderBy(o => o.f_cost).ToList();
        size = elements.Count;
    }

    public Nodes Dequeue(MapNode node) {
        foreach (Nodes element in elements) {
            if (element.to.position == node.position) {
                elements.Remove(element);
                size = elements.Count;
                return element;
            }
        }
        return null;
    }

    public Nodes Dequeue(Nodes node) {
        elements.Remove(node);
        size = elements.Count;
        return node;
    }

    public List<Nodes> get_Queue() {
        return elements.ToList();
    }
}


public class Nodes {
    public MapNode to;

    public MapNode from;

    public double g_cost;

    public double f_cost;

    public Nodes(MapNode to, MapNode from, double g_cost, double f_cost) {
        this.to = to;
        this.from = from;
        this.g_cost = g_cost;
        this.f_cost = f_cost;
    }
}