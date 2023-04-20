using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class MapNode
{
    public DungeonNode node;
    public bool explored;
}

public class Minimap
{
    private Dictionary<Vector2, MapNode> nodes;
    private GameObject minimapHolder;

    public Minimap(DungeonGraph graph)
    {
        // Create a map of the graph
        nodes = new Dictionary<Vector2, MapNode>();
        foreach (var node in graph.nodes)
        {
            nodes.Add(node.Key, new MapNode() { node = node.Value, explored = false });
        }
        generateElements();
    }

    public void testPosition(Vector2 position)
    {
        var size = LevelGenManager.ROOMSIZE * 2;
        var nodePos = new Vector2(
            (float)Math.Round(position.x / size), 
            (float)Math.Round(position.y / size));

        if (!nodes.ContainsKey(nodePos) || nodes[nodePos].explored) 
            return;

        var basicRoom = Resources.Load("Minimap/RoomBasic");
        nodes[nodePos].explored = true;
        var obj = (GameObject) UnityEngine.Object.Instantiate(basicRoom, nodePos * 2 * LevelGenManager.ROOMSIZE, Quaternion.identity);
        obj.name = nodePos + " room";
        obj.transform.parent = minimapHolder.transform.Find("minimapRooms");
    }

    private void generateElements()
    {
        // Create a GameObject called Minimap
        minimapHolder = GameObject.Find("minimap");
        if (minimapHolder != null)
            UnityEngine.Object.DestroyImmediate(minimapHolder);

        minimapHolder = new GameObject("minimap") { layer = LayerMask.NameToLayer("Minimap") };
        var minimapRooms = new GameObject("minimapRooms") { transform = { parent = minimapHolder.transform } };
    }
}
