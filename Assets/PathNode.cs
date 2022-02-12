using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode{

    private GridArea grid;
    private int x, y;
    public int gCost;
    public int hCost;
    public int fCost;
    public PathNode previous;
    public PathNode(GridArea grid, int x, int y){
        this.grid = grid;
        this.x = x;
        this.y = y;
         
    }

    public override string ToString(){
        return x+ " "+ y;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
