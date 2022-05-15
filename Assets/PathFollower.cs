using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFollower : MonoBehaviour {

    public List<MapNode> path;
    public int currentWayPoint = 1;
    public Vector3 targetWayPoint;

    public float speed = 1f;
    private float maxSpeed = 6f;
    private float minSpeed = 2f;
    private float Acceleration = 5f;
    private float Decelaration = 10f;
    public float timer;
    Vector3 beforePassed, afterPassed;
    float totalPassedDistance;

    TestScript script;

    public GameObject player;
    GridArea grid;

    Vector3 start, end;

    public static bool arrive = false;





    // Use this for initialization
    void Start() {
        script = GameObject.FindWithTag(("Grid")).GetComponent<TestScript>();
        player = GameObject.FindWithTag(("Player"));
        arrive = false;
        timer = 0;
        beforePassed = new Vector3();
        afterPassed = new Vector3();
        totalPassedDistance = 0;
    }

    // Update is called once per frame
    void Update() {
        if (grid == null) {
            grid = script.grid;
            return;
        }
        if (grid.path != null && path == null) {
            path = grid.path;
            return;
        }
        if (path == null || path.Count == 0) {
            return;
        }
        // i want the car to appear after the path is found
        //defult is not null for vector its 0 0 0 :))))
        if (is_start_end_defult(start, end)) {
            start = get_position(grid.start);
            end = get_position(grid.end);
            player.transform.position = start;
        }
        if (!grid.isFirstSelected) {
            targetWayPoint = get_position(this.path[currentWayPoint].position);
            maxSpeed = getSpeed(currentWayPoint);
            move();
        } else {
            if (grid.isFirstSelected) {
                start = new Vector3();
                end = new Vector3();
                currentWayPoint = 1;
            }
        }
    }



    public void change_path(List<MapNode> new_path, int new_point) {
        int index = path.FindIndex(p => p.position == new_path[0].position);
        this.path = new_path;
        this.grid.path = new_path;
        this.currentWayPoint = new_point;
        targetWayPoint = get_position(path[this.currentWayPoint].position);
        grid.a_star.cleanPath(path);
    }


    public bool next_node_on_path(MapNode node) {
        if (targetWayPoint != node.position) {
            return false;
        }
        return true;
    }


    private float getSpeed(int targetIndex) {
        var current = this.path[targetIndex - 1];
        var next = this.path[targetIndex];
        float speed;
        if (next.position.x - current.position.x == 0) {
            if (next.position.y - current.position.y > 0) {
                speed = current.upSpeed;
            } else {
                speed = current.downSpeed;
            }
        } else {
            if (next.position.x - current.position.x > 0) {
                speed = current.rightSpeed;
            } else {
                speed = current.leftSpeed;
            }
        }
        return speed * 1.2f;
    }

    public bool is_start_end_defult(Vector3 start, Vector3 end) {
        if (start.x == 0 && start.y == 0 && end.x == 0 && end.y == 0) {
            return true;
        }
        return false;
    }
    public Vector3 get_position(Vector3 vector) {
        return get_position(vector.x, vector.y);
    }
    public Vector3 get_position(float x, float y) {
        return grid.GetWorldPosition(x, y) + new Vector3(grid.cellSize / 2, grid.cellSize / 2);
    }
    void move() {
        
        Drive();
        Accelerate();
        //Decelarate();
        CalculateTimePassed();
        
    }

    void Drive() {
        beforePassed = player.transform.position;
        // move towards the next waypoint
        player.transform.position = Vector3.MoveTowards(player.transform.position, targetWayPoint, (speed) * Time.deltaTime);
        Vector3 relativePos = targetWayPoint - player.transform.position;
        if (!relativePos.Equals(new Vector3())) {
            float angle = Mathf.Atan2(relativePos.y, relativePos.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            player.transform.rotation = rotation;
        }
        if (player.transform.position.Equals(targetWayPoint) && !this.end.Equals(player.transform.position)) {
            currentWayPoint++;
            targetWayPoint = get_position(path[currentWayPoint].position);
        }
        if (this.end.Equals(player.transform.position)) {
            arrive = true;
        }
        afterPassed = player.transform.position;
    }

    void Accelerate() {
        if (DistanceToNextWaypoint() > 1f && speed < maxSpeed) {
            speed += Acceleration * Time.deltaTime;
        }
    }

    void Decelarate() {
        // not auccrate you may try to fix it ---
        float amount = speed + DistanceToNextWaypoint() * 0.1f;
        if (DistanceToNextWaypoint() <= amount && speed > minSpeed) {
            speed -= Decelaration * Time.deltaTime;
        }
    }


    float DistanceToNextWaypoint() {
        return Vector3.Distance(player.transform.position, targetWayPoint);
    }


    public float CalculateTotalDistance() {
        float totalDistance = 0;
        if (path != null) {
            for (int i = 0; i < (this.path.Count - 1); i++) {
                totalDistance += Vector3.Distance(path[i].position, path[i + 1].position);
            }
        }
        return totalDistance;
    }

    public float CalculateRemainingDistance() {
        float totalDistance = 0;
        totalPassedDistance += Vector3.Distance(beforePassed, afterPassed);
        totalDistance = CalculateTotalDistance() - (totalPassedDistance / 1.2f);
        return totalDistance;
    }

    public Vector3[] MapNodeToVector(){
        Vector3[] p = new Vector3[path.Count];
        for (int i = 0; i < path.Count; i++){
            p[i] = path[i].position;
        }
        return p;
    }

    public void CalculatePassedDistance(){
        
    }

    public float CalculateAVGSpeed() {
        float speed = 0;
        for (int i = 1; i < (this.path.Count); i++) {
            speed += getSpeed(i);
        }
        return speed / path.Count;
    }

    public float CalculateExpectedArriveTime() {
        return (CalculateTotalDistance() / CalculateAVGSpeed());
    }

    public void CalculateTimePassed() {
        if (!arrive) {
            timer += Time.deltaTime;
        }
    }
}