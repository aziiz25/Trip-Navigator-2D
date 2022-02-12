using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour{
    // Start is called before the first frame update
    GridArea grid;
    void Start(){
        grid = new GridArea(4,2, 10f, new Vector3(20,0));
    }

    // Update is called once per frame
    void Update(){
        if(Input.GetMouseButtonDown(0)){
            grid.SetValue(GetMousePositionWorld(), 1);
        }
    }
    public Vector3 GetMousePositionWorld() {
        Vector3 vec = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        vec.z = 0f;
        return vec;
    }
}
