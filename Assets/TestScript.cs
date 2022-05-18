
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

    public GameObject no_path;

    public DrawTraffic traffic;


    bool check_user_path_action;
    


    /*
    For state initialization 
    */
    private void Awake() {
        start = GameObject.Find("Start");
        select_path = GameObject.Find("SelectPath");
        choose_traffic = GameObject.Find("Traffic");
        run = GameObject.Find("Run");
        summary = GameObject.Find("Summary");
        changeStart = GameObject.Find("Change Start");
        changeEnd = GameObject.Find("Change End");
        no_path = GameObject.Find("NoPath");
    }

    /*
    Load variable 
    */
    void Start() {
        start.SetActive(true);
        select_path.SetActive(false);
        run.SetActive(false);
        choose_traffic.SetActive(false);
        summary.SetActive(false);
        changeStart.SetActive(false);
        changeEnd.SetActive(false);
        no_path.SetActive(false);
        path2 = GameObject.Find(("Path2"));
        car = GameObject.FindWithTag(("Grid")).GetComponent<PathFollower>();
        traffic = GameObject.FindWithTag(("Grid")).GetComponent<DrawTraffic>();
        string currentPath = Directory.GetCurrentDirectory();
        string pathToFile = "/Assets/Maps/map_" + (GameManager.instance.CharIndex + 1) + ".csv";
        string[][] mapValues = readMapData(currentPath + pathToFile);
        grid = new GridArea(88, 40, 1.2f, mapValues);
    }

    // Update is called once per frame
    // main loop of the program
    void Update() {
        try {
            if (Input.GetMouseButtonDown(0) && IsStartActive() && (!grid.isFirstSelected || !grid.isSecondSelected)) {
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
                changeStart.SetActive(true);
                changeEnd.SetActive(true);
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
                MoveToChooseRoadWithOutTraffic();
            }
            if (choose_traffic.activeSelf) {
                StartCoroutine(user_actions());
            }

            if (PathFollower.arrive) {
                MoveToSummary();
            }
            if (IsSummaryActive()) {
                summary_info();
            }
        } catch (Exception e) {
            MoveToNoPath();
            print(e);
        }
    }

    /*
    Get where the user click on the Screen
    */
    public Vector3 GetMousePositionWorld() {
        Vector3 vec = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        vec.z = 0f;
        return vec;
    }


    /*
    Regaring the user chooseing points 
    */
    public void Confirm() {
        if (!grid.isFirstSelected && grid.start != null) {
            grid.confirmStart();
            if (!grid.isFirstSelected) {
                return;
            }
            GameObject.Find("StartText").GetComponentInChildren<Text>().text = grid.start.x + "x-axis, " + grid.start.y + "y-axis";
            if (!grid.isSecondSelected) {
                GameObject.Find("EndText").GetComponentInChildren<Text>().text = "Select Your End Point";
            }
            GameObject.Find("EndText").GetComponentInChildren<Text>().color = Color.red;
            GameObject.Find("EndText").GetComponentInChildren<Text>().fontSize = 48;
            changeStart.SetActive(true);
        } else {
            try {
                if (grid.isFirstSelected && grid.isSecondSelected){
                paths = grid.Start();
                }
                grid.confirmEnd();
                GameObject.Find("EndText").GetComponentInChildren<Text>().text = grid.end.x + "x-axis, " + grid.end.y + "y-axis";
                changeEnd.SetActive(true);
            } catch (Exception e) {
                MoveToNoPath();
                print(e);
            }
        }
    } 

    public void ChangeStart() {
        if (paths != null) {
            paths = null;
            MoveToStart();
            grid.delete_paths();

        }
        grid.isFirstSelected = false;
        changeStart.SetActive(false);
    }

    public void ChangeEnd() {
        if (paths != null) {
            paths = null;
            MoveToStart();
            grid.delete_paths();
        }
        grid.isSecondSelected = false;
        changeEnd.SetActive(false);
    }

    /*
    those two methods linked to two button on the ui based on what the user select
    the path will be chosen
    */
    public void ChoosePath1() {
        grid.choose_path(0, paths);
    }

    public void ChoosePath2() {
        grid.choose_path(1, paths);
    }

    /*
    This linked to a button on the screen that change the path if the user click on change path 
    the button will be shown for 3 sec if the user click then the car will go to the better path
    */
    public void ChangePath() {
        if (paths.Count < 2) {
            return;
        }
        choose_traffic.SetActive(false);
        car.change_path(paths[1], car.currentWayPoint);
        paths.RemoveAt(0);
        grid.draw_path();
        grid.a_star.cleanPath(paths[0]);
    }

    /*
    This method will wait for the user action for 3 sec if the user didnt do 
    anything the car will stay on path and the button will disappair
    */
    public IEnumerator user_actions() {
        yield return new WaitForSeconds(3);
        change_path_action();
    }

    //linked to above method
    public void change_path_action() {
        if (paths.Count < 2) {
            return;
        }
        choose_traffic.SetActive(false);
        paths.Remove(paths[1]);
        grid.draw_path();
        car.path = paths[0];
    }

    // showning cost to the user when he tries to choose a path
    public void CalculateCosts() {
        float costPath1 = car.CalculateExpectedArriveTime(paths[0]);
        GameObject.Find("Path1").GetComponentInChildren<Text>().text = "Green\nTime: " + Math.Floor(costPath1) + " mins";
        if (paths.Count == 2) {
            float costPath2 = car.CalculateExpectedArriveTime(paths[1]);
            GameObject.Find("Path2").GetComponentInChildren<Text>().text = "Blue\nTime: " + Math.Ceiling(costPath2) + " mins";
        }
    }

    // showing info during the trip
    public void run_info() {
        if (car.path != null) {
            double dis = car.CalculateRemainingDistance();
            double speed = car.speed;
            double avgSpeed = car.CalculateAVGSpeed();
            double time = dis / speed;
            if(speed < avgSpeed){
                time = dis/avgSpeed;
            }
           
            
            if ((int)time > 0) {
                GameObject.Find("ArrivalTime").GetComponentInChildren<Text>().text = (int)time + " min";
                GameObject.Find("ArrivalTime").GetComponentInChildren<Text>().fontSize = 48;
            } else {
                GameObject.Find("ArrivalTime").GetComponentInChildren<Text>().text = "Less then a min";
                GameObject.Find("ArrivalTime").GetComponentInChildren<Text>().fontSize = 40;
            }

            GameObject.Find("Distance").GetComponentInChildren<Text>().text = Math.Round(dis) + " km";
            GameObject.Find("CurrentSpeed").GetComponentInChildren<Text>().text = speed.ToString("0.0 km/min");
            GameObject.Find("ArrivalTime").GetComponentInChildren<Text>().color = Color.red;
        }
    }

    //show summary after the trip ends
    public void summary_info() {
        if (car.path != null) {
            double time = car.timer;
            double dis = car.CalculateTotalDistance();
            double speed = dis / time;
            GameObject.Find("Duration").GetComponentInChildren<Text>().text = (int)time + " min";
            GameObject.Find("TotatlDistance").GetComponentInChildren<Text>().text = dis.ToString("0.00 km");
            GameObject.Find("AVGSpeed").GetComponentInChildren<Text>().text = speed.ToString("0.00 km/min");
        }
    }
    // reading the map
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

    // check if path not equal
    public void check_not_equal_path() {
        // if node not found
        foreach (MapNode node in traffic.path) {
            int index = car.path.FindIndex(p => p.position == node.position);
            if (index == -1) {
                paths.Add(grid.a_star.cleanPath(traffic.path));
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
        CalculateCosts();
    }
    //for choose traffic starte
    public void MoveToChooseRoadWithOutTraffic() {
        if (paths.Count < 2 || check_user_path_action) {
            choose_traffic.SetActive(false);
        } else {
            choose_traffic.SetActive(true);
        }
        check_user_path_action = true;
    }

    public void MoveToStart() {
        start.SetActive(true);
        select_path.SetActive(false);
        run.SetActive(false);
        summary.SetActive(false);
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


    public void MoveToNoPath() {
        start.SetActive(false);
        select_path.SetActive(false);
        run.SetActive(false);
        summary.SetActive(false);
        no_path.SetActive(true);
    }
}
