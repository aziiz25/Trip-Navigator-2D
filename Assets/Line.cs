using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour {


    public GridArea grid;

    public TestScript script;



    public void Start() {
        if (GameObject.FindWithTag(("Grid")) != null) {
            script = GameObject.FindWithTag(("Grid")).GetComponent<TestScript>();
        }
    }

    public void Update() {
        if (this.grid == null) {
            this.grid = script.grid;
        }

    }


    public GameObject DrawLine(Vector3 start, Vector3 end, Color color, float width = 0.55f) {
        start = get_position(start.x, start.y);
        end = get_position(end.x, end.y);
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended"));
        lr.startColor = color;
        lr.endColor = color;
        lr.startWidth = width;
        lr.endWidth = width;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        return myLine;
    }


        public Vector3 get_position(float x, float y) {
        return grid.GetWorldPosition(x, y) + new Vector3(grid.cellSize / 2, grid.cellSize / 2);
    }

}
