
using System.IO;
using UnityEngine.UI;
using System;
using UnityEngine;

public class TestScript : MonoBehaviour {
    // Start is called before the first frame update
    public GridArea grid;

    void Start() {
        string currentPath = Directory.GetCurrentDirectory();
        string[][] mapValues = readMapData(currentPath + "/Assets/Maps/map_1.csv");
        grid = new GridArea(88, 40, 1.2f, mapValues);
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Vector3 mousePosition = GetMousePositionWorld();
            if (mousePosition.y < -18) {
                return;
            }
            grid.SetValue(mousePosition, 1);
            GameObject.Find("Confirm").GetComponentInChildren<Button>().interactable = true;
        }
    }

    public Vector3 GetMousePositionWorld() {
        Vector3 vec = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        vec.z = 0f;
        return vec;
    }

    public void confirmStart() {
        GameObject.Find("Confirm").GetComponentInChildren<Text>().text = "Confirm End";
        GameObject.Find("Confirm").GetComponentInChildren<Button>().onClick.RemoveListener(confirmStart);
        GameObject.Find("Confirm").GetComponentInChildren<Button>().onClick.AddListener(confirmEnd);
        GameObject.Find("Confirm").GetComponentInChildren<Button>().interactable = false;

        grid.confirmStart();
    }

    public void confirmEnd() {
        grid.confirmEnd();
    }

    public string[][] readMapData(string filePath) {
        string[][] values = new string[88][];
        string[] text = System.IO.File.ReadAllLines(@filePath);
        string[][] stringValues = new string[40][];
        for (int i = 0; i < 40; i++) {
            stringValues[i] = text[i].Split(',');
        }
        for (int i = 0; i < 88; i++) {
            values[i] = new string[40];
            for (int j = 0; j < 40; j++) {
                values[i][39 - j] = stringValues[j][i];
            }
        }
        return values;
    }
}
