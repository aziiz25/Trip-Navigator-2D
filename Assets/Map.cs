using System;
using System.Collections.Generic;

public class Map
{
    public MapNode start { get; set; }
    public MapNode end { get; set; }
    public List<MapNode> nodes { get; set; }
}