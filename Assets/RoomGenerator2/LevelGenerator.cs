using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class Door
{
    public Vector2 pos;
    public Direction dir;
    public CustomRoom room;
    public int doorsInRoom;

    public Door(Vector2 pos, Direction dir, CustomRoom room, int doorsInRoom)
    {
        this.pos = pos;
        this.dir = dir;
        this.room = room;
        this.doorsInRoom = room.repeatable ? 0 : doorsInRoom;
    }
}

public static class ListExtensions
{
    // Takes an iterable and shuffles it around
    public static void Shuffle<T>(this IList<T> list)
    {
        var random = new System.Random();
        for (var i = list.Count - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}

public class DungeonNode
{
    public bool[] doors = { false, false, false, false };
}

public class DungeonGraph
{
    public LevelGenerator parent;
    public Dictionary<Vector2, DungeonNode> nodes;
    public List<(Vector2, CustomRoom)> rooms;
    public int size => nodes.Count;

    public DungeonGraph(CustomRoom initialRoom, LevelGenerator parent)
    {
        nodes = new Dictionary<Vector2, DungeonNode>();
        rooms = new List<(Vector2, CustomRoom)>();
        placeRoom(new Vector2(0, 1), new Vector2(0, 0), Direction.Down, initialRoom);
        this.parent = parent;
    }

    public bool isTopDoorSuitable(Vector2 door)
    {
        for (var i = 0; i < 4; i++)
        {
            if (i == (int)Direction.Down)
                continue;
            // Check if the graph contains a node adjacent to the door
            if (!nodes.ContainsKey(door + CustomRoom.dirVectors[i]))
                continue;
            if (!nodes[door + CustomRoom.dirVectors[i]].doors[CustomRoom.mirrorDir[i]]) 
                continue;
            return false;
        }
        return true;
    }

    public bool areDoorsBlocked(List<Door> doors, Vector2 shift, RoomNodeHolder room)
    {
        foreach (var door in doors)
        {
            var pos = door.pos + shift;

            // Test in every direction with a door that there is either not a node or that the node has a door
            var dir = (int)door.dir;
            var neighborPos = pos + CustomRoom.dirVectors[dir];
            if (!nodes.ContainsKey(neighborPos))
                continue;
            // Either both are false or both are true, in any other case there is a blockage
            if (!nodes[neighborPos].doors[CustomRoom.mirrorDir[dir]] && !room[door.pos].doors[dir])
                continue;
            if (nodes[neighborPos].doors[CustomRoom.mirrorDir[dir]] && room[door.pos].doors[dir])
                continue;
            return true;
        }
        
        // Check that no node is blocking any open door in the graph
        foreach (var door in parent.remainingDoors)
        {
            var doorNeighborLocal = door.pos + CustomRoom.dirVectors[(int)door.dir] - shift;
            if (room.ContainsKey(doorNeighborLocal) && !room[doorNeighborLocal].doors[CustomRoom.mirrorDir[(int)door.dir]])
                return true;
        }

        // Check that it does not obstruct the top door either
        {
            var doorNeighborLocal = parent.topDoor.pos + Vector2.up - shift;
            if (room.ContainsKey(doorNeighborLocal))
            {
                if (!room[doorNeighborLocal].doors[CustomRoom.mirrorDir[(int)Direction.Down]])
                    return true;

                // Check that there is a new door to use as future potential door
                foreach (var door in doors.Where(door => door.dir == Direction.Up))
                {
                    if (isTopDoorSuitable(door.pos))
                        return false;
                }
                return true;
            }
        }
        return false;
    }

    public (int, Vector2) testRoom(Vector2 doorPos, Direction doorDir, CustomRoom room)
    {
        // Get all entrances of the room that have a mirrored direction to the one provided
        var doors = room.getDoors();
        var mirroredEntrances = doors
            .Where(door => (int)door.dir == CustomRoom.mirrorDir[(int)doorDir])
            .Select(door => door.pos)
            .ToList();
        // If there are no mirrored entrances, the room cannot be placed
        if (mirroredEntrances.Count == 0)
            return (-1, new Vector2(0, 0));
        // Order entrances from lowest y to highest y
        var orderedEntrances = mirroredEntrances.OrderBy(pos => pos.y).ToArray();
        // Get the position in the graph of the entrance
        var entrancePos = doorPos + CustomRoom.dirVectors[(int)doorDir];

        // Test the room with each entrance
        var failedTries = 0;
        var door = new Vector2(0, 0);
        var found = false;
        foreach (var entrance in orderedEntrances)
        {
            // Get the vector shift needed to move the room to the graph door
            var shift = entrancePos - entrance;
            // Check if the room can be placed at the door
            if (room.roomNodes
                .Select(roomNode => roomNode.Key + shift)
                .Any(nodePos => nodes.ContainsKey(nodePos)))
            {
                failedTries++;
                continue;
            }

            // Check that no door is being blocked
            if (areDoorsBlocked(doors.ToList(), shift, room.roomNodes))
            {
                failedTries++;
                continue;
            }

            found = true;
            door = entrance;
            break;
        }

        return !found ? (-1, new Vector2()) : (failedTries + (room.repeatable ? 2 : 0), door);
    }

    public List<Door> placeRoom(Vector2 graphPos, Vector2 doorPos, Direction doorDir, CustomRoom prefab)
    {
        // Calculate the shift needed to move the room to the graph door
        var shift = graphPos - doorPos + CustomRoom.dirVectors[(int)doorDir];

        // Place all nodes of the room
        foreach (var node in prefab.roomNodes)
        {
            var pos = node.Key + shift;
            nodes[pos] = new DungeonNode()
            {
                doors = node.Value.doors
            };
        }
        rooms.Add((shift, prefab));

        var remainingDoors = new List<Door>();
        // Return the list of doors that can be used to connect to other rooms
        var doors = prefab.getDoors();
        foreach (var door in doors)
        {
            var node = prefab.roomNodes[door.pos];
            for (var i = 0; i < node.doors.Length; i++)
            {
                // get the adjacent node to the door
                var neighborPos = door.pos + CustomRoom.dirVectors[i] + shift;
                // If the door is open and there is no node there, add it to the list of remaining doors
                if (node.doors[i] && !nodes.ContainsKey(neighborPos))
                    remainingDoors.Add(new Door(door.pos + shift, (Direction)i, prefab, door.doorsInRoom));
            }
        }

        //foreach (var node in prefab.roomNodes)
        //{
        //    for (var i = 0; i < node.Value.doors.Length; i++)
        //    {
        //        // get the adjacent node to the door
        //        var neighborPos = node.Key + CustomRoom.dirVectors[i] + shift;
        //        // If the door is open and there is no node there, add it to the list of remaining doors
        //        if (node.Value.doors[i] && !nodes.ContainsKey(neighborPos))
        //            remainingDoors.Add(new Door(node.Key + shift, (Direction)i));
        //    }
        //}
        return remainingDoors;
    }
#if UNITY_EDITOR
    public void draw()
    {
        foreach (var (position, room) in rooms)
        {
            room.draw(position);
        }

        foreach (var node in nodes)
        {
            for (var i = 0; i < 4; i++)
            {
                var neighborPos = node.Key + CustomRoom.dirVectors[i];
                if (node.Value.doors[i] 
                    && nodes.ContainsKey(neighborPos)
                    && !nodes[neighborPos].doors[CustomRoom.mirrorDir[i]])
                {
                    Gizmos.color = Color.red;
                    var firstCorner = node.Key * 2*LevelGenManager.ROOMSIZE + (CustomRoom.dirVectors[i] + CustomRoom.sideToDirVectors[i]) * LevelGenManager.ROOMSIZE;
                    var secondCorner = node.Key * 2*LevelGenManager.ROOMSIZE + (CustomRoom.dirVectors[i] - CustomRoom.sideToDirVectors[i]) * LevelGenManager.ROOMSIZE;
                    Gizmos.DrawLine(firstCorner, secondCorner);
                }
            }
        }
    }
#endif

    public bool validate()
    {
        foreach (var node in nodes)
        {
            for (var i = 0; i < 4; i++)
            {
                var neighborPos = node.Key + CustomRoom.dirVectors[i];
                if (node.Value.doors[i] 
                    && nodes.ContainsKey(neighborPos)
                    && !nodes[neighborPos].doors[CustomRoom.mirrorDir[i]])
                {
                    return false;
                }
            }
        }

        return true;
    }
}

public class LevelGenerator
{
    public DungeonGraph graph;
    private List<CustomRoom> normalRooms;
    private List<CustomRoom> closingRooms;

    private List<CustomRoom> usedRooms;

    public List<Door> remainingDoors;
    public Door topDoor;

    public void stepGenerate(int size)
    {
        if (graph == null)
        {
            initGeneration();
        }
        
        var (finished, success) = nextRoom(size);
        if (finished) Debug.Log("Finished successfully");
        if (!success) Debug.Log("Failed to generate");
    }

    public void generate(int size)
    {
        var tries = 0;
        do
        {
            tries++;
            if (tries > 20)
            {
                Debug.LogError("Failed to generate level");
                return;
            }

            initGeneration();
            var res = tryGenerate(size);
            if (!res) 
                continue;

            endGeneration();
        } while (!graph.validate());
        insertPrefabs();
    }

    private void insertPrefabs()
    {
        // Try to find the object RoomHolder and if it exists, delete it
        var roomHolder = GameObject.Find("RoomHolder");
        if (roomHolder != null)
            Object.DestroyImmediate(roomHolder);

        // Create a roomHolder game object
        roomHolder = new GameObject("RoomHolder");

        foreach (var room in graph.rooms)
        {
            // Instantiate the room
            var roomObj = Object.Instantiate(room.Item2, room.Item1 * 2 * LevelGenManager.ROOMSIZE, Quaternion.identity);
            roomObj.name = room.Item2.name + " | " + room.Item1;
            roomObj.transform.parent = roomHolder.transform;
        }
        // Add the roomHolder to the current scene
        SceneManager.MoveGameObjectToScene(roomHolder, SceneManager.GetActiveScene());
    }

    public void initGeneration()
    {
        var initRoom = Resources.Load<CustomRoom>("Rooms/InitRoom");
        remainingDoors = new List<Door>{new Door(new Vector2(0, 0), Direction.Up, initRoom, 0)};
        normalRooms = new List<CustomRoom>();
        closingRooms = Resources.LoadAll<CustomRoom>("Rooms/ClosingRooms").ToList();
        usedRooms = new List<CustomRoom>();
        graph = new DungeonGraph(initRoom, this);
        topDoor = new Door(new Vector2(0, 0), Direction.Up, initRoom, 0);
    }

    private void endGeneration()
    {
        var endRoom = Resources.Load<CustomRoom>("Rooms/EndRoom");
        graph.placeRoom(topDoor.pos, new Vector2(0, 0), Direction.Up, endRoom);
    }

    private (bool, bool) nextRoom(int size)
    {
        // If we are out of rooms but still have to continue, we refill the normal room list
        if (normalRooms.All(room => room.repeatable))
        {
            normalRooms = Resources.LoadAll<CustomRoom>("Rooms/NormalRooms").ToList();
        }

        // Get a random element from the list and remove it
        var weightedDoors = new List<Door>();
        foreach (var door in remainingDoors)
        {
            switch (door.doorsInRoom)
            {
                case 2:
                    for (var i = 0; i < LevelGenManager.twoDoorRoomBias; i++)
                        weightedDoors.Add(door);
                    break;
                case 3:
                    for (var i = 0; i < LevelGenManager.threeDoorRoomBias; i++)
                        weightedDoors.Add(door);
                    break;
                default:
                    weightedDoors.Add(door);
                    break;
            }
        }

        var nextPos = weightedDoors[Random.Range(0, weightedDoors.Count)];
        remainingDoors.Remove(nextPos);
        var (room, entrance) = getRoom(size - graph.size, nextPos);
        if (room == null)
            return (false, false);
        var exits = graph.placeRoom(nextPos.pos, entrance, nextPos.dir, room);
        processNewDoors(exits);
        // For each remainining door, ensure that there is no adjacent node in the graph
        // If there is, remove it from the list
        remainingDoors = remainingDoors
            .Where(door => !graph.nodes.ContainsKey(door.pos + CustomRoom.dirVectors[(int)door.dir]))
            .ToList();

        // If it's a normal room, we remove so it does not repeat
        if (normalRooms.Contains(room) && !room.repeatable)
        {
            normalRooms.Remove(room);
            usedRooms.Add(room);
        }

        // If we have no more doors, we are done
        return (remainingDoors.Count == 0, true);
    }

    private void processNewDoors(ICollection<Door> newDoors)
    {
        // Get all doors that are looking upwards and get the one with the hightest y value
        var topDoors = newDoors
            .Where(door => door.dir == Direction.Up)
            .OrderBy(door => door.pos.y)
            .ToList();

        for (var i = 0; i < topDoors.Count; i++)
        {
            if (graph.isTopDoorSuitable(topDoors[i].pos + CustomRoom.dirVectors[(int)Direction.Up])) 
                continue;

            topDoors.RemoveAt(i);
            i--;
        }
        if (topDoors.Count > 0 && topDoors.Last().pos.y > topDoor.pos.y)
        {
            remainingDoors.Add(topDoor);
            topDoor = topDoors.Last();
            newDoors.Remove(topDoors.Last());
        }
        // Add all new doors to the remaining door list
        remainingDoors = remainingDoors.Concat(newDoors).ToList();
    }

    private bool tryGenerate(int size)
    {
        while (true)
        {
            var (finished, success) = nextRoom(size);
            if (finished)
            {
                return graph.nodes.Count >= size;
            }
            if (!success) 
                return false;
        }
    }

    private (CustomRoom, Vector2) getRoom(int remainingSize, Door door)
    {
        (CustomRoom, Vector2) retRoom;

        if (remainingSize > 0)
        {
            retRoom = getNormalRoom(door, false);
            if (retRoom.Item1 == null)
                retRoom = getClosingRoom(door);
        }
        else
        {
            retRoom = getClosingRoom(door);
        }

        return retRoom;
    }

    private (CustomRoom, Vector2) getClosingRoom(Door door)
    {
        var rooms = closingRooms.ToList();

        // Get the posisition of the other node of the door
        var otherNodePos = door.pos + CustomRoom.dirVectors[(int)door.dir];

        // Get how many nodes adjacent to it have an open door looking at it
        var doorsToOpen = new[] { false, false, false, false };
        for (var i = 0; i < CustomRoom.dirVectors.Length; i++)
        {
            // Get the node adjacent to the otherNode
            var neighborPos = otherNodePos + CustomRoom.dirVectors[i];
            // If the node exists and has an open door looking at the otherNode, we set the door to open as true
            if (graph.nodes.ContainsKey(neighborPos) && graph.nodes[neighborPos].doors[CustomRoom.mirrorDir[i]])
                doorsToOpen[i] = true;
        }

        // Find closing rooms that have the same doors to open
        rooms = rooms
            .Where(room => room.roomNodes[new Vector2(0, 0)].doors.SequenceEqual(doorsToOpen))
            .ToList();
        
        // Return random room from the options
        if (rooms.Count == 0)
        {
            Debug.LogError("Missing closing room layout!! Do we have all posible rooms?");
            return (null, new Vector2(0, 0));
        }
        return (rooms[Random.Range(0, rooms.Count)], new Vector2(0, 0));
    }

    private (CustomRoom, Vector2) getNormalRoom(Door door, bool secondTry)
    {
        var rooms = normalRooms.ToList();
        if (secondTry)
        {
            rooms = normalRooms
                .Concat(usedRooms)
                .ToList();
        }
        else if (door.room.repeatable)
        {
            rooms = rooms.Where(room => !room.repeatable).ToList();
        }
        rooms = rooms.Where(room => room.name != door.room.name).ToList();
        // Shuffle filtered rooms
        rooms.Shuffle();
        // Find a room that can be placed, order by the lowest door
        var results = rooms
            // Get all result of testing the liability of the room
            .Select(room =>
            {
                var test = graph.testRoom(door.pos, door.dir, room);
                return (room, test.Item2, test.Item1);
            })
            //Filter all invalid ones
            .Where(result => result.Item3 != -1)
            //Order by the lowest door
            .OrderBy(result => result.Item3)
            .Select(result => (result.Item1, result.Item2))
            .ToList();
        if (results.Count == 0)
        {
            return secondTry ? (null, new Vector2()) : getNormalRoom(door, true);
        }
        // We return the best result and if the room is flipped
        return (results[0].Item1, results[0].Item2);
    }


}