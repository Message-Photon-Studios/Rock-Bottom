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
        var nodePos = new Vector2(
            (position.x - (position.x % LevelGenManager.ROOMSIZE)) / LevelGenManager.ROOMSIZE, 
            (position.y - (position.y % LevelGenManager.ROOMSIZE)) / LevelGenManager.ROOMSIZE);

        if (nodes.ContainsKey(nodePos) && !nodes[nodePos].explored)
        {
            nodes[nodePos].explored = true;
            // Find and delete child with name nodePos + " black"
            var child = minimapHolder.transform.Find("fogOfWar").Find(nodePos + " black");
            UnityEngine.Object.DestroyImmediate(child.gameObject);
        }
    }

    private void generateElements()
    {
        var basicRoom = Resources.Load("Minimap/RoomBasic");
        var outOfBounds = Resources.Load("Minimap/OutOfBounds");
        var wall = Resources.Load("Minimap/WallBasic");

        // Create a GameObject called Minimap
        minimapHolder = GameObject.Find("minimap");
        if (minimapHolder != null)
            UnityEngine.Object.DestroyImmediate(minimapHolder);
        minimapHolder = new GameObject("minimap");

        // Set minimapHolder layer to be minimap
        minimapHolder.layer = LayerMask.NameToLayer("Minimap");
        
        var minimapRooms = new GameObject("minimapRooms");
        minimapRooms.transform.parent = minimapHolder.transform;
        foreach (var node in nodes)
        {
            //Instantiate a basic room in the position of the node
            var obj = (GameObject) UnityEngine.Object.Instantiate(basicRoom, node.Key * 2 * LevelGenManager.ROOMSIZE, Quaternion.identity);
            obj.name = node.Key + " room";
            obj.transform.parent = minimapRooms.transform;
        }

        var minPos = new Vector2(nodes.Select(elem => elem.Key.x).Min() - 5, nodes.Select(elem => elem.Key.y).Min() - 5);
        var maxPos = new Vector2(nodes.Select(elem => elem.Key.x).Max() + 5, nodes.Select(elem => elem.Key.y).Max() + 5);
        
        
        var fogOfWar = new GameObject("fogOfWar");
        fogOfWar.transform.parent = minimapHolder.transform;
        for (var x = minPos.x; x < maxPos.x; x++)
        {
            for (var y = minPos.y; y < maxPos.y; y++)
            {
                var pos = new Vector2(x, y);
                var obj = (GameObject) UnityEngine.Object.Instantiate(outOfBounds, pos * 2 * LevelGenManager.ROOMSIZE, Quaternion.identity);
                obj.name = pos + " black";
                obj.transform.parent = fogOfWar.transform;
            }
        }
    }
}
