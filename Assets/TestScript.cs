
using System.IO;
using UnityEngine.UI;
using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class TestScript : MonoBehaviour {
    // Start is called before the first frame update
    public GridArea grid;
    public PathFollower car;
    public GameObject start;
    public GameObject run;
    public GameObject summary;
    public GameObject changeStart;
    public GameObject changeEnd;

    bool select_point;

    List<List<MapNode>> paths;
    private void Awake() {
        start = GameObject.Find("Start");
        run = GameObject.Find("Run");
        summary = GameObject.Find("Summary");
        changeStart = GameObject.Find("Change Start");
        changeEnd = GameObject.Find("Change End");
    }


    void Start() {
        start.SetActive(true);
        run.SetActive(false);
        summary.SetActive(false);
        changeStart.SetActive(false);
        changeEnd.SetActive(false);
        car = GameObject.FindWithTag(("Grid")).GetComponent<PathFollower>();
        string currentPath = Directory.GetCurrentDirectory();
        string pathToFile = "/Assets/Maps/map_" + (GameManager.instance.CharIndex + 1) + ".csv";
        string[][] mapValues = readMapData(currentPath + pathToFile);
        grid = new GridArea(88, 40, 1.2f, mapValues);
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetMouseButtonDown(0) && IsStartActive()) {
            Vector3 mousePosition = GetMousePositionWorld();
            if (mousePosition.y < -18) {
                return;
            }
            if (paths == null) {
                grid.SetValue(mousePosition, 1);
            } else {
                choose_path(mousePosition, paths);
            }
        }
        if (grid.path == null && paths != null) {
            grid.draw_paths(paths);
        }
        if (grid.path != null) {
            grid.draw_path();
            MoveToRun();
        }
        if (IsRunActive()) {
            run_info();
        }
        if (PathFollower.arrive) {
            MoveToSummary();
        }
        if (IsSummaryActive()) {
            summary_info();
        }
    }


    public Vector3 GetMousePositionWorld() {
        Vector3 vec = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        vec.z = 0f;
        return vec;
    }

    public void Confirm() {
        if (!grid.isFirstSelected && grid.start != null) {
            grid.confirmStart();
            GameObject.Find("StartText").GetComponentInChildren<Text>().text = grid.start.x + ", " + grid.start.y;
            GameObject.Find("EndText").GetComponentInChildren<Text>().text = "Select Your End Point";
            GameObject.Find("EndText").GetComponentInChildren<Text>().fontSize = 48;
            GameObject.Find("EndText").GetComponentInChildren<Text>().color = Color.red;
        } else {
            paths = grid.confirmEnd();
            GameObject.Find("EndText").GetComponentInChildren<Text>().text = grid.end.x + ", " + grid.end.y;
        }
    }

    public void choose_path(Vector3 mousePosition, List<List<MapNode>> paths) {
        grid.choose_path(mousePosition, paths);
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


    public void run_info() {
        if (car.path != null) {
            double time = car.CalculateExpectedArriveTime();
            double dis = car.CalculateRemainingDistance();
            double speed = car.speed;
            GameObject.Find("ArrivalTime").GetComponentInChildren<Text>().text = (int)time + " min";
            GameObject.Find("Distance").GetComponentInChildren<Text>().text = (int)dis + " km";
            GameObject.Find("CurrentSpeed").GetComponentInChildren<Text>().text = (int)speed + " km";
            GameObject.Find("ArrivalTime").GetComponentInChildren<Text>().fontSize = 48;
            GameObject.Find("ArrivalTime").GetComponentInChildren<Text>().color = Color.red;
        }
    }


    public void summary_info() {
        if (car.path != null) {
            double time = car.timer;
            double dis = car.CalculateTotalDistance();
            double speed = car.CalculateAVGSpeed();
            GameObject.Find("Duration").GetComponentInChildren<Text>().text = (int)time + " min";
            GameObject.Find("TotatlDistance").GetComponentInChildren<Text>().text = (int)dis + " km";
            GameObject.Find("AVGSpeed").GetComponentInChildren<Text>().text = (int)speed + " km";
        }
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

    public void MainMenu() {
        SceneManager.LoadScene("MainMenu");
    }

    public void Quit() {
        Application.Quit();
    }

    public bool IsStartActive() {
        return start.activeSelf;
    }

    public bool IsRunActive() {
        return run.activeSelf;
    }

    public bool IsSummaryActive() {
        return summary.activeSelf;
    }


    public void MoveToRun() {
        start.SetActive(false);
        run.SetActive(true);
        summary.SetActive(false);
    }

    public void MoveToSummary() {
        start.SetActive(false);
        run.SetActive(false);
        summary.SetActive(true);
    }
}
