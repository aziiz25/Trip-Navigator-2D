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
    private float Acceleration = 2f;
    private float Decelaration = 4f;

    TestScript script;

    public GameObject player;
    GridArea grid;

    Vector3 start, end;


    // Use this for initialization
    void Start() {
        script = GameObject.FindWithTag(("Grid")).GetComponent<TestScript>();
        player = GameObject.FindWithTag(("Player"));
    }

    // Update is called once per frame
    void Update() {
        if (grid == null) {
            grid = script.grid;
        }
        if (grid != null) {
            // i want the car to appear after the path is found
            if (grid.path != null) {
                path = grid.path;
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
        }
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
        return speed;
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
        Decelarate();
    }

    void Drive() {
        // move towards the next waypoint
        player.transform.position = Vector3.MoveTowards(player.transform.position, targetWayPoint, speed * Time.deltaTime);

        // rotate towards the next waypoint !!! not working me dont know how  Heeeeeeeeeeelp!!!!! :(

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
    }

    void Accelerate() {
        if (DistanceToNextWaypoint() > 1f && speed < maxSpeed) {
            speed += Acceleration * Time.deltaTime;
        }
    }

    void Decelarate() {
        if (DistanceToNextWaypoint() <= 5f && speed > minSpeed) {
            speed -= Decelaration * Time.deltaTime;
        }
    }


    float DistanceToNextWaypoint() {
        return Vector3.Distance(player.transform.position, targetWayPoint);
    }
}