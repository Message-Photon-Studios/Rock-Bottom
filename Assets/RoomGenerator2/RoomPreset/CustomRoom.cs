using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

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

public enum DoorColor
{
    None = 0,
    Green = 1,
    Red = 2,
    Blue = 3,
    Yellow = 4,
    Pink = 5,
    Cyan = 6
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
}

[Serializable]
public class RoomNode
{
    public DoorColor[] doors = { DoorColor.None, DoorColor.None, DoorColor.None, DoorColor.None };
}

public class CustomRoom : MonoBehaviour
{
    public static readonly Vector2[] dirVectors = { Vector2.left, Vector2.down, Vector2.right, Vector2.up };
    public static readonly Vector2[] sideToDirVectors = { Vector2.down, Vector2.right, Vector2.up, Vector2.left };
    public static readonly int[] mirrorDir = { 2, 3, 0, 1 };

    public bool repeatable;
    public DoorColor roomRegionColor = DoorColor.Green;
    public bool allowGreenClosingRooms = true;
    public RoomNodeHolder roomNodes;

    public DisplayMode displayMode;
    public int maxSpawns = -1;

    [HideInInspector] 
    public int spawnCount = 0;
    
    private Vector2? _size;
    private Vector2? _minNode;

    [HideInInspector]
    public Vector2 size => _size ?? (_size = getSize()).Value;
    public Vector2 minNode => _minNode ?? (_minNode = getDownLeftCorner()).Value;

    [HideInInspector]
    public Vector2 selectedNode;
    public bool isClosingRoom = false;
    
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
                        if(neighbors.Length <= i)
                        {
                            Debug.LogWarning("Neighbor doors insufficient!");
                            Gizmos.color = Color.black;
                            break;
                        } 
                        else if (neighbors[i] == null)
                        {
                            if(node.Value.doors.Length <= i)
                            {
                                Debug.LogWarning("Node doors insufficient!");
                                Gizmos.color = Color.black;
                            } else
                            {
                                switch ((DoorColor)node.Value.doors[i])
                                {
                                    case DoorColor.None:
                                        Gizmos.color = Color.white;
                                        break;
                                    case DoorColor.Green:
                                        Gizmos.color = Color.green;
                                        break;
                                    case DoorColor.Red:
                                        Gizmos.color = Color.red;
                                        break;
                                    case DoorColor.Blue:
                                        Gizmos.color = Color.blue;
                                        break;
                                    case DoorColor.Yellow:
                                        Gizmos.color = Color.yellow;
                                        break;
                                    case DoorColor.Pink:
                                        Gizmos.color = Color.magenta;
                                        break;
                                    case DoorColor.Cyan:
                                        Gizmos.color = Color.cyan;
                                        break;
                                    default:
                                        Gizmos.color = Color.black;
                                        break;
                                }
                            }
                            //Gizmos.color = !node.Value.doors[i] ? Color.white : Color.green;
                        }
                           
                        else
                        {
                            if(node.Value.doors.Length <= i)
                            {
                                Debug.LogWarning("Node doors insufficient!");
                                Gizmos.color = Color.black;
                            }
                            else if (node.Value.doors[i] == DoorColor.None)
                                Gizmos.color = Color.white;
                            else
                                continue;
                        }
                        var firstCorner = (shift + node.Key) * 2*LevelGenManager.ROOMSIZE + (dirVectors[i] + sideToDirVectors[i]) * LevelGenManager.ROOMSIZE;
                        var secondCorner = (shift + node.Key) * 2*LevelGenManager.ROOMSIZE + (dirVectors[i] - sideToDirVectors[i]) * LevelGenManager.ROOMSIZE;
                        Gizmos.DrawLine(firstCorner, secondCorner);
                        break;
                    case DisplayMode.Connections when neighbors[i] != null:
                        if (node.Value.doors[i] == DoorColor.None)
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
                node.doors[mirrorDir[i]] = DoorColor.None;
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
            neighbors[i].doors[mirrorDir[i]] = DoorColor.Green;
            node.doors[i] = DoorColor.Green;
        }
        selectedNode = newCoords;
        roomNodes.Add(newCoords, node);
        EditorUtility.SetDirty(this);
    }

    public void toggleDoor(Direction doorDir, DoorColor doorColor)
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
            {
                if(node.doors[(int)doorDir] == doorColor)
                    neighbor.doors[mirrorDir[(int)doorDir]] = DoorColor.None;
                else
                    neighbor.doors[mirrorDir[(int)doorDir]] = doorColor;
                //neighbor.doors[mirrorDir[(int)doorDir]] = !neighbor.doors[mirrorDir[(int)doorDir]];
            }
        }
        
        //node.doors[(int)doorDir] = !node.doors[(int)doorDir];
        if(node.doors[(int)doorDir] == doorColor) node.doors[(int)doorDir] = DoorColor.None;
        else node.doors[(int)doorDir] = doorColor;

        EditorUtility.SetDirty(this);
    }

    public void changeAllDoors(DoorColor doorColor)
    {
        if(doorColor == DoorColor.None) return;
        foreach (var node in roomNodes)
        {
            var neighbors = roomNodes.getNeighbors(node.Key);
            for (var i = 0; i < 4; i++)
            {
                if (node.Value.doors[i] != DoorColor.None && neighbors[i] == null)
                    node.Value.doors[i] = doorColor;
            }
        }

        EditorUtility.SetDirty(this);
    }

#endif

    public List<Door> getDoors()
    {
        var count = 0;
        foreach (var node in roomNodes)
        {
            var neighbors = roomNodes.getNeighbors(node.Key);
            for (var i = 0; i < 4; i++)
            {
                if (node.Value.doors[i] != DoorColor.None && neighbors[i] == null)
                    count++;
            }
        }
        var doors = new List<Door>();
        foreach (var node in roomNodes)
        {
            var neighbors = roomNodes.getNeighbors(node.Key);
            for (var i = 0; i < 4; i++)
            {
                if (node.Value.doors[i] != DoorColor.None && neighbors[i] == null)
                    doors.Add(new Door(node.Key, (Direction)i, this, count, node.Value.doors[i], allowGreenClosingRooms));
            }
        }

        return doors;
    }

    public Vector2 getSize()
    {
        var min = new Vector2(int.MaxValue, int.MaxValue);
        var max = new Vector2(int.MinValue, int.MinValue);
        foreach (var node in roomNodes)
        {
            if (node.Key.x < min.x)
                min.x = node.Key.x;
            if (node.Key.y < min.y)
                min.y = node.Key.y;
            if (node.Key.x > max.x)
                max.x = node.Key.x;
            if (node.Key.y > max.y)
                max.y = node.Key.y;
        }
        return max - min + Vector2.one;
    }

    public Vector2 getDownLeftCorner()
    {
        // Get the node with the lowest x and y value
        var min = new Vector2(int.MaxValue, int.MaxValue);
        foreach (var node in roomNodes)
        {
            if (node.Key.x < min.x)
                min.x = node.Key.x;
            if (node.Key.y < min.y)
                min.y = node.Key.y;
        }
        return min;
    }
}
