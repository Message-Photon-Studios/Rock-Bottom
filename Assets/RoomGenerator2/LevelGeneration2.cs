using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

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

public class DungeonNode2
{
    public bool[] doors = { false, false, false, false };
}

public class DungeonGraph2
{
    public Dictionary<Vector2, DungeonNode2> nodes;
    public List<(Vector2, CustomRoom, bool)> rooms;
    public int size => nodes.Count;

    public DungeonGraph2(CustomRoom initialRoom)
    {
        nodes = new Dictionary<Vector2, DungeonNode2>();
        rooms = new List<(Vector2, CustomRoom, bool)>();
        placeRoom(new Vector2(0, 1), new Vector2(0, 0), Direction.Down, initialRoom, false);
    }

    public bool areDoorsBlocked(List<(Vector2, Direction)> doors, Vector2 shift, RoomNodeHolder room)
    {
        var blocked = false;
        foreach (var door in doors)
        {
            var pos = door.Item1 + shift;

            // Test in every direction with a door that there is either not a node or that the node has a door
            var dir = (int)door.Item2;
            var neighborPos = pos + CustomRoom.dirVectors[dir];
            if (!nodes.ContainsKey(neighborPos))
                continue;
            // Either both are false or both are true, in any other case there is a blockage
            if (!nodes[neighborPos].doors[CustomRoom.mirrorDir[dir]] && !room[door.Item1].doors[dir])
                continue;
            if (nodes[neighborPos].doors[CustomRoom.mirrorDir[dir]] && room[door.Item1].doors[dir])
                continue;
            blocked = true;
            break;
        }
        return blocked;
    }

    public (int, Vector2) testRoom(Vector2 doorPos, Direction doorDir, RoomNodeHolder room)
    {
        // Get all entrances of the room that have a mirrored direction to the one provided
        var doors = room.getDoors();
        var mirroredEntrances = doors
            .Where(door => (int)door.Item2 == CustomRoom.mirrorDir[(int)doorDir])
            .Select(door => door.Item1)
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
            if (room
                .Select(roomNode => roomNode.Key + shift)
                .Any(nodePos => nodes.ContainsKey(nodePos)))
            {
                failedTries++;
                continue;
            }

            // Check that no door is being blocked
            if (areDoorsBlocked(doors.ToList(), shift, room))
            {
                failedTries++;
                continue;
            }

            found = true;
            door = entrance;
            break;
        }

        return !found ? (-1, new Vector2()) : (failedTries, door);
    }

    public List<(Vector2, Direction)> placeRoom(Vector2 graphPos, Vector2 doorPos, Direction doorDir, CustomRoom prefab, bool isFlipped)
    {
        // Calculate the shift needed to move the room to the graph door
        var shift = graphPos - doorPos + CustomRoom.dirVectors[(int)doorDir];

        //If flipped, get the flipped nodeHolder
        var room = isFlipped ? prefab.roomNodes.getFlipped() : prefab.roomNodes;

        // Place all nodes of the room
        foreach (var node in room)
        {
            var pos = node.Key + shift;
            nodes[pos] = new DungeonNode2()
            {
                doors = node.Value.doors
            };
        }
        rooms.Add((shift, prefab, isFlipped));

        var remainingDoors = new List<(Vector2, Direction)>();
        // Return the list of doors that can be used to connect to other rooms
        foreach (var node in room)
        {
            for (var i = 0; i < node.Value.doors.Length; i++)
            {
                // get the adjacent node to the door
                var neighborPos = node.Key + CustomRoom.dirVectors[i] + shift;
                // If the door is open and there is no node there, add it to the list of remaining doors
                if (node.Value.doors[i] && !nodes.ContainsKey(neighborPos))
                    remainingDoors.Add((node.Key + shift, (Direction)i));
            }
        }
        return remainingDoors;
    }

    public void draw()
    {
        foreach (var (position, room, flipped) in rooms)
        {
            room.draw(position, flipped);
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
                    var firstCorner = node.Key * 12f + (CustomRoom.dirVectors[i] + CustomRoom.sideToDirVectors[i]) * 6f;
                    var secondCorner = node.Key * 12f + (CustomRoom.dirVectors[i] - CustomRoom.sideToDirVectors[i]) * 6f;
                    Gizmos.DrawLine(firstCorner, secondCorner);
                }
            }
        }
    }
}

public class LevelGeneration2
{
    public DungeonGraph2 graph;
    private List<CustomRoom> normalRooms;
    private List<CustomRoom> closingRooms;

    private List<CustomRoom> usedRooms;

    private List<(Vector2, Direction)> remainingDoors;

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
                break;
            initGeneration();
        } 
        while (!tryGenerate(size));
        if (tries <= 20) 
            return;

        Debug.Log("Failed to generate level");
    }

    public void initGeneration()
    {
        var initRoom = Resources.Load<CustomRoom>("Rooms/InitRoom");
        remainingDoors = new List<(Vector2, Direction)>{(new Vector2(0, 0), Direction.Up)};
        normalRooms = new List<CustomRoom>();
        closingRooms = Resources.LoadAll<CustomRoom>("Rooms/ClosingRooms").ToList();
        usedRooms = new List<CustomRoom>();
        graph = new DungeonGraph2(initRoom);
    }

    private (bool, bool) nextRoom(int size)
    {
        // If we are out of rooms but still have to continue, we refill the normal room list
        if (normalRooms.Count == 0)
        {
            normalRooms = Resources.LoadAll<CustomRoom>("Rooms/NormalRooms").ToList();
        }

        // Get a random element from the list and remove it
        var nextPos = remainingDoors[Random.Range(0, remainingDoors.Count)];
        remainingDoors.Remove(nextPos);
        var (room, entrance, flipped) = getRoom(size - graph.size, nextPos.Item1, nextPos.Item2);
        if (room == null)
            return (false, false);
        var exits = graph.placeRoom(nextPos.Item1, entrance, nextPos.Item2, room, flipped);
        remainingDoors = remainingDoors.Concat(exits).ToList();
        // If it's a normal room, we remove so it does not repeat
        if (normalRooms.Contains(room))
        {
            normalRooms.Remove(room);
            usedRooms.Add(room);
        }

        // If we have no more doors, we are done
        return (remainingDoors.Count == 0, true);
    }

    private bool tryGenerate(int size)
    {
        while (true)
        {
            var (finished, success) = nextRoom(size);
            if (finished) 
                return true;
            if (!success) 
                return false;
        }
    }

    private (CustomRoom, Vector2, bool) getRoom(int remainingSize, Vector2 pos, Direction doorDir)
    {
        (CustomRoom, Vector2, bool) retRoom;

        if (remainingSize > 0)
        {
            retRoom = getNormalRoom(pos, doorDir, false);
            if (retRoom.Item1 == null)
                retRoom = getClosingRoom(pos, doorDir);
        }
        else
        {
            retRoom = getClosingRoom(pos, doorDir);
        }

        return retRoom;
    }

    private (CustomRoom, Vector2, bool) getClosingRoom(Vector2 pos, Direction doorDir)
    {
        var rooms = closingRooms.ToList();

        // Get the posisition of the other node of the door
        var otherNodePos = pos + CustomRoom.dirVectors[(int)doorDir];

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
        if (rooms.Count > 0) 
            return (rooms[Random.Range(0, rooms.Count)], new Vector2(0, 0), false);

        // If we have no rooms, we try again but flipping them
        rooms = closingRooms.ToList();
        rooms = rooms
            .Where(room => room.roomNodes.getFlipped()[new Vector2(0, 0)].doors.SequenceEqual(doorsToOpen))
            .ToList();

        // If we don't find any, we are missing a crucial closing room layout
        if (rooms.Count == 0)
        {
            Debug.LogError("Missing closing room layout!! Do we have all posible rooms?");
            return (null, new Vector2(0, 0), false);
        }

        return (rooms[Random.Range(0, rooms.Count)], new Vector2(0, 0), true);
    }

    private (CustomRoom, Vector2, bool) getNormalRoom(Vector2 pos, Direction doorDir, bool secondTry)
    {
        var rooms = normalRooms.ToList();
        if (secondTry)
        {
            rooms = normalRooms.Concat(usedRooms).ToList();
        }
        // Shuffle filtered rooms
        rooms.Shuffle();
        // Find a room that can be placed, order by the lowest door
        var results = rooms
            // Get all result of testing the liability of the room
            .Select(room =>
            {
                var test = graph.testRoom(pos, doorDir, room.roomNodes);
                if (test.Item1 != -1) 
                    return (room, test.Item2, test.Item1, false);
                test = graph.testRoom(pos, doorDir, room.roomNodes.getFlipped());
                return (room, test.Item2, test.Item1, true);
            })
            //Filter all invalid ones
            .Where(result => result.Item3 != -1)
            //Order by the lowest door
            .OrderBy(result => result.Item3)
            .Select(result => (result.Item1, result.Item2, result.Item4))
            .ToList();
        if (results.Count == 0)
        {
            return secondTry ? (null, new Vector2(), false) : getNormalRoom(pos, doorDir, true);
        }
        // We return the best result and if the room is flipped
        return (results[0].Item1, results[0].Item2, results[0].Item3);
    }


}