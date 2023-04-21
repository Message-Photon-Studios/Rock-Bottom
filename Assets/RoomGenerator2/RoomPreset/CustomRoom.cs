using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public enum DisplayMode
{
    Walls,
    Connections
}

public enum Direction
{
    Left = 0,
    Down = 1,
    Right = 2,
    Up = 3
}

[System.Serializable]
public class RoomNodeHolder : SerializableDictionary<Vector2, RoomNode>
{
    public RoomNode[] getNeighbors(Vector2 pos)
    {
        // Get all adjacent nodes in the four cardinal directions
        var neighbors = new RoomNode[4];
        for (var i = 0; i < 4; i++)
        {
            var neighborPos = pos + CustomRoom.dirVectors[i];
            if (ContainsKey(neighborPos))
                neighbors[i] = this[neighborPos];
        }
        return neighbors;
    }

    public List<(Vector2, Direction)> getDoors()
    {
        var doors = new List<(Vector2, Direction)>();
        foreach (var node in this)
        {
            var neighbors = getNeighbors(node.Key);
            for (var i = 0; i < 4; i++)
            {
                if (node.Value.doors[i] && neighbors[i] == null)
                    doors.Add((node.Key, (Direction)i));
            }
        }

        return doors;
    }
}

[Serializable]
public class RoomNode
{
    public bool[] doors = { false, false, false, false };
}

public class CustomRoom : MonoBehaviour
{
    public static readonly Vector2[] dirVectors = { Vector2.left, Vector2.down, Vector2.right, Vector2.up };
    public static readonly Vector2[] sideToDirVectors = { Vector2.down, Vector2.right, Vector2.up, Vector2.left };
    public static readonly int[] mirrorDir = { 2, 3, 0, 1 };
    
    public RoomNodeHolder roomNodes;

    public DisplayMode displayMode;

    [HideInInspector]
    public Vector2 selectedNode;
    
#if UNITY_EDITOR
    public void draw(Vector2 shift)
    {
        foreach (var node in roomNodes)
        {
            Gizmos.color = node.Key == selectedNode ? Color.yellow : Color.white;
            Gizmos.DrawSphere((shift + node.Key) * 2*LevelGenManager.ROOMSIZE, 1f);
            var neighbors = roomNodes.getNeighbors(node.Key);
            for (var i = 0; i < neighbors.Length; i++)
            {
                switch (displayMode)
                {
                    // Displays the room's walls. White for normal walls, green for open walls
                    case DisplayMode.Walls:
                        if (neighbors[i] == null)
                            Gizmos.color = !node.Value.doors[i] ? Color.white : Color.green;
                        else
                        {
                            if (!node.Value.doors[i])
                                Gizmos.color = Color.white;
                            else
                                continue;
                        }
                        var firstCorner = (shift + node.Key) * 2*LevelGenManager.ROOMSIZE + (dirVectors[i] + sideToDirVectors[i]) * LevelGenManager.ROOMSIZE;
                        var secondCorner = (shift + node.Key) * 2*LevelGenManager.ROOMSIZE + (dirVectors[i] - sideToDirVectors[i]) * LevelGenManager.ROOMSIZE;
                        Gizmos.DrawLine(firstCorner, secondCorner);
                        break;
                    case DisplayMode.Connections when neighbors[i] != null:
                        if (!node.Value.doors[i])
                            continue;
                        Gizmos.color = neighbors[i] == null ? Color.green : Color.white;
                        Gizmos.DrawLine((shift + node.Key) * 2*LevelGenManager.ROOMSIZE, (shift + node.Key) * 2*LevelGenManager.ROOMSIZE + dirVectors[i] * LevelGenManager.ROOMSIZE);
                        break;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        draw(Vector2.zero);
        UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
    }
    
    public void deleteNode()
    {
        if (!roomNodes.ContainsKey(selectedNode))
            return;
        // Remove all references from neighbors
        var neighbors = roomNodes.getNeighbors(selectedNode);
        for (var i = 0; i < neighbors.Length; i++)
        {
            var node = neighbors[i];
            // set the door in the right direction to false
            if (node != null)
                node.doors[mirrorDir[i]] = false;
        }
        roomNodes.Remove(selectedNode);
        EditorUtility.SetDirty(this);
    }

    public void addNode(Direction dir)
    {
        var node = new RoomNode();
        var newCoords = new Vector2(0, 0);
        // If this is the first node, we simply add it without any other checks
        if (roomNodes.Count == 0)
        {
            roomNodes.Add(newCoords, node);
            return;
        }

        // Check if the selected node exists in the dictionary
        if (roomNodes.ContainsKey(selectedNode))
        {
            newCoords = selectedNode + dirVectors[(int)dir];
            if (roomNodes.ContainsKey(newCoords))
                return;
        }
        else
        {
            return;
        }

        // Check if the node will be connected to other nodes, if not, exit
        var neighbors = roomNodes.getNeighbors(newCoords);
        if (neighbors.All(elem => elem == null))
            return;

        // Correct all door values in the neighbors
        for (var i = 0; i < neighbors.Length; i++)
        {

            if (neighbors[i] == null)
                continue;
            neighbors[i].doors[mirrorDir[i]] = true;
            node.doors[i] = true;
        }
        selectedNode = newCoords;
        roomNodes.Add(newCoords, node);
        EditorUtility.SetDirty(this);
    }

    public void toggleDoor(Direction doorDir)
    {
        if (!roomNodes.ContainsKey(selectedNode))
            return;
        var node = roomNodes[selectedNode];
        var newPos = selectedNode + dirVectors[(int)doorDir];
        //Check if there is a node in the selected direction
        if (roomNodes.ContainsKey(newPos))
        {
            var neighbor = roomNodes[newPos];
        
            //If there is a node, toggle its mirrored door
            if (neighbor != null)
                neighbor.doors[mirrorDir[(int)doorDir]] = !neighbor.doors[mirrorDir[(int)doorDir]];
        }
        
        node.doors[(int)doorDir] = !node.doors[(int)doorDir];
        EditorUtility.SetDirty(this);
    }
#endif
}
