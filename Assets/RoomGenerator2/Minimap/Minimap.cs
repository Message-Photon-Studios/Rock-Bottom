using System;
using System.Collections.Generic;
using System.Drawing;
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
        
        disoverTile(nodePos + new Vector2(-1,  1));
        disoverTile(nodePos + new Vector2( 1, -1));
        disoverTile(nodePos + new Vector2(-1, -1));
        disoverTile(nodePos + new Vector2(-1,  0));
        disoverTile(nodePos + new Vector2( 0, -1));
        disoverTile(nodePos + new Vector2( 1,  1));
        disoverTile(nodePos + new Vector2( 1,  0));
        disoverTile(nodePos + new Vector2( 0,  1));
        disoverTile(nodePos + new Vector2( 0,  0));
    }

    private void disoverTile(Vector2 nodePos)
    {
        var size = LevelGenManager.ROOMSIZE * 2;   
        if (!nodes.ContainsKey(nodePos) || nodes[nodePos].explored) 
            return;

        var basicRoom = Resources.Load("Minimap/RoomBasic");
        var basicWall = Resources.Load("Minimap/WallBasic");
        nodes[nodePos].explored = true;
        var obj = (GameObject) UnityEngine.Object.Instantiate(basicRoom, nodePos * 2 * LevelGenManager.ROOMSIZE, Quaternion.identity);
        obj.name = nodePos + " room";
        obj.transform.parent = minimapHolder.transform.Find("minimapRooms");

        var node = nodes[nodePos].node;
        for (var i = 0; i < 4; i++)
        {
            if (node.doors[i])
                continue;

            var wall = (GameObject) UnityEngine.Object.Instantiate(
            basicWall, 
                nodePos * size + CustomRoom.dirVectors[i] * LevelGenManager.ROOMSIZE, 
                Quaternion.identity);
            wall.transform.parent = obj.transform;
            if (i is (int)Direction.Down or (int)Direction.Up)
                wall.transform.Rotate(new Vector3(0, 0, 1), 90);
        }
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
