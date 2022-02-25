using UnityEngine;

public class MapNode
{

    public Vector3 position { get; set; }
    public MapNode up { get; set; }
    public MapNode down { get; set; }
    public MapNode left { get; set; }
    public MapNode right { get; set; }

    public float upCost { get; set; }
    public float downCost { get; set; }
    public float leftCost { get; set; }
    public float rightCost { get; set; }


}