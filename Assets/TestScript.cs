
using System.IO;
using UnityEngine.UI;
using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;

public class TestScript : MonoBehaviour {
    // Start is called before the first frame update
    public GridArea grid;
    public PathFollower car;
    public GameObject start;
    public GameObject run;
    public GameObject summary;
    public GameObject select_path;
    public GameObject changeStart;

    public GameObject changeEnd;

    bool select_point;

    List<List<MapNode>> paths;

    public GameObject path2;

    public GameObject choose_traffic;

    public DrawTraffic traffic;

    bool keep_path;

    bool check_user_path_action;

    private void Awake() {
        start = GameObject.Find("Start");
        select_path = GameObject.Find("SelectPath");
        choose_traffic = GameObject.Find("Traffic");
        run = GameObject.Find("Run");
        summary = GameObject.Find("Summary");
        changeStart = GameObject.Find("Change Start");
        changeEnd = GameObject.Find("Change End");
    }


    void Start() {
        start.SetActive(true);
        select_path.SetActive(false);
        run.SetActive(false);
        choose_traffic.SetActive(false);
        summary.SetActive(false);
        changeStart.SetActive(false);
        changeEnd.SetActive(false);
        path2 = GameObject.Find(("Path2"));
        car = GameObject.FindWithTag(("Grid")).GetComponent<PathFollower>();
        traffic = GameObject.FindWithTag(("Grid")).GetComponent<DrawTraffic>();
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
            }
        }

        if (grid.path == null && paths != null) {
            grid.draw_paths(paths);
            MoveToChoosePath();
            if (paths.Count < 2 && GameObject.Find("Path2") != null) {
                GameObject.Find("Path2").SetActive(false);
            }
        }

        if (grid.path != null && !IsRunActive()) {
            grid.draw_path();
            paths.Clear();
            paths.Add(grid.path);
            MoveToRun();
        }
        if (IsRunActive()) {
            run_info();
        }
        if (traffic.isTraffic && !check_user_path_action) {
            check_not_equal_path();
            grid.draw_paths(paths);
            MoveToChooseRoadWithTraffic();
        }
        if (choose_traffic.activeSelf) {
            StartCoroutine(user_actions());
        }
        
        if(keep_path){

        }

        if (PathFollower.arrive) {
            MoveToSummary();
        }
        if (IsSummaryActive()) {
            summary_info();
        }
    }

    public IEnumerator user_actions() {
        yield return new WaitForSeconds(3);
        change_path_action();
    }

    public void change_path_action() {
        if (paths.Count < 2) {
            return;
        }
        choose_traffic.SetActive(false);
        car.change_path(paths[1], car.currentWayPoint);
        paths.RemoveAt(0);
        grid.draw_path();
        grid.a_star.cleanPath(paths[0]);
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


    public void ChoosePath1() {
        grid.choose_path(0, paths);
    }

    public void ChoosePath2() {
        grid.choose_path(1, paths);
    }


    public void keep_current_path() {
        if (paths.Count < 2) {
            return;
        }
        choose_traffic.SetActive(false);
        paths.Remove(paths[1]);
        grid.draw_path();
        keep_path = true;
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


    public void check_not_equal_path() {
        foreach (MapNode node in traffic.path) {
            int index = car.path.FindIndex(p => p.position == node.position);
            if (index == -1) {
                paths.Add(traffic.path);
                return;
            }
        }
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


    public void MoveToChoosePath() {
        start.SetActive(false);
        select_path.SetActive(true);
        choose_traffic.SetActive(false);
        run.SetActive(false);
        summary.SetActive(false);
    }

    public void MoveToChooseRoadWithTraffic() {
        if (paths.Count < 2) {
            choose_traffic.SetActive(false);
        } else {
            choose_traffic.SetActive(true);
        }
        check_user_path_action = true;
    }


    public void MoveToRun() {
        start.SetActive(false);
        select_path.SetActive(false);
        run.SetActive(true);
        summary.SetActive(false);
    }

    public void MoveToSummary() {
        start.SetActive(false);
        select_path.SetActive(false);
        run.SetActive(false);
        summary.SetActive(true);
    }
}
