using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Gizmos = UnityEngine.Gizmos;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public enum RoomType
{
    Big,
    Tall,
    Long,
    Normal,
    Start,
    End
}

public class DungeonGraph
{

    public List<DungeonNode> nodes;
    public List<DungeonBigNode> bigNodes;
    public List<DungeonTallNode> tallNodes;
    public List<DungeonLongNode> longNodes;
    public List<DungeonNode> drawnNodes;
    public class DungeonNode
    {
        public int depth;
        public DungeonGraph parent;
        public DungeonNode[] connections;
        public RoomType roomType = RoomType.Normal;
        public Vector2 pos;

        public int layoutCode
        {
            get
            {
                var _layoutCode = 0;
                if (connections[0] != null) _layoutCode += 8;
                if (connections[1] != null) _layoutCode += 4;
                if (connections[2] != null) _layoutCode += 2;
                if (connections[3] != null) _layoutCode += 1;
                return _layoutCode;
            }
        }
        public bool explored;

        public DungeonNode(Vector2 pos, DungeonGraph parent)
        {
            connections = new DungeonNode[] { null, null, null, null };
            this.pos = pos;
            this.parent = parent;
        }

        public void generateRandomly(float p, float pFall, int currentDepth, int maxDepth, int maxBranches, DungeonNode origin = null)
        {
            explored = true;
            List<Vector2> offsets = new()
            {
                Vector2.left,
                Vector2.down,
                Vector2.right,
                Vector2.up,
            };
            if (origin != null)
                depth = origin.depth + 1;
            else
                depth = currentDepth;
            
            var i = 0;
            var branchCount = 0;
            foreach (var offset in offsets)
            {
                if (connections[i] != null)
                {
                    i++;
                    continue;
                }
                if (currentDepth >= maxDepth || branchCount >= maxBranches) break;
                var res = Random.Range(0, 100);
                if (res < p * 100)
                {
                    var existing = parent.nodes.Find(node => node.pos == offset + pos);
                    if (existing != null)
                    {
                        connections[i] = existing;
                        var mirror = offsets.IndexOf(offset * -1);
                        existing.connections[mirror] = this;
                    }
                    else
                    {
                        var newNode = new DungeonNode(offset + pos, parent);
                        parent.nodes.Add(newNode);
                        connections[i] = newNode;
                        var mirror = offsets.IndexOf(offset * -1);
                        newNode.connections[mirror] = this;
                        
                    }
                    branchCount++;
                }
                i++;
            }

            foreach (var node in connections)
            {
                if (node is { explored: false })
                    node.generateRandomly(p - pFall, pFall, currentDepth + 1, maxDepth, maxBranches, this);
            }
        }

        public void draw()
        {
            parent.drawnNodes.Add(this);
            foreach (var node in connections)
            {
                if (node == null)
                    continue;
                Gizmos.color = roomType switch
                {
                    RoomType.Big => Color.cyan,
                    RoomType.Tall => Color.blue,
                    RoomType.Long => Color.magenta,
                    RoomType.Normal => Color.white,
                    RoomType.Start => Color.green,
                    RoomType.End => Color.red,
                    _ => throw new ArgumentOutOfRangeException()
                };
                Gizmos.DrawLine(pos * 20 + Vector2.one * 10, (node.pos + pos) * 10 + Vector2.one * 10);
                if (parent.drawnNodes.Exists(elem => node.pos == elem.pos))
                    continue;
                node.draw();
            }
            Gizmos.color = roomType switch
            {
                RoomType.Big => Color.cyan,
                RoomType.Tall => Color.blue,
                RoomType.Long => Color.magenta,
                RoomType.Normal => Color.white,
                RoomType.Start => Color.green,
                RoomType.End => Color.red,
                _ => throw new ArgumentOutOfRangeException()
            };
            Gizmos.DrawSphere(pos * 20 + Vector2.one * 10, 2f);
        }
    }

    public class DungeonBigNode
    {
        public DungeonNode[] members;
        public bool mirrored;

        public Vector2 pos => members[0].pos;

        public int layoutCode
        {
            get
            {
                var _layoutCode = 0;
                if (members[2].connections[0] != null) _layoutCode += 128;
                if (members[2].connections[3] != null) _layoutCode += 64;
                if (members[3].connections[2] != null) _layoutCode += 32;
                if (members[3].connections[3] != null) _layoutCode += 16;
                if (members[0].connections[0] != null) _layoutCode += 8;
                if (members[0].connections[1] != null) _layoutCode += 4;
                if (members[1].connections[2] != null) _layoutCode += 2;
                if (members[1].connections[1] != null) _layoutCode += 1;
                return _layoutCode;
            }
        }

        public DungeonBigNode(
            DungeonNode bottomLeft,
            DungeonNode topLeft,
            DungeonNode topRight,
            DungeonNode bottomRight)
        {
            members = new[] { bottomLeft, topLeft, topRight, bottomRight };
        }

        public void draw()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(members[0].pos * 20 + Vector2.one * 20, 4f);
            Gizmos.color = Color.white;
        }
    }

    public class DungeonTallNode
    {
        public DungeonNode[] members;
        public bool mirrored;

        public Vector2 pos => members[0].pos;

        public int layoutCode
        {
            get
            {
                var _layoutCode = 0;
                if (members[1].connections[3] != null) _layoutCode += 32; //topDoor
                if (members[0].connections[1] != null) _layoutCode += 16; //bottomDoor
                if (members[1].connections[0] != null) _layoutCode += 8;  //leftTopDoor
                if (members[0].connections[0] != null) _layoutCode += 4;  //leftBottomDoor 
                if (members[1].connections[2] != null) _layoutCode += 2;  //rightTopDoor
                if (members[0].connections[2] != null) _layoutCode += 1;  //rightBottomDoor
                return _layoutCode;
            }
        }

        public DungeonTallNode(
            DungeonNode bottom, 
            DungeonNode top)
        {
            members = new[] { bottom, top };
        }

        public void draw()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(members[0].pos * 20 + new Vector2(10f, 20f), 4f);
            Gizmos.color = Color.white;
        }
    }

    public class DungeonLongNode
    {
        public DungeonNode[] members;
        public bool mirrored;

        public Vector2 pos => members[0].pos;

        public int layoutCode
        {
            get
            {
                var _layoutCode = 0;
                if (members[0].connections[0] != null) _layoutCode += 32;
                if (members[1].connections[2] != null) _layoutCode += 16;
                if (members[0].connections[3] != null) _layoutCode += 8;
                if (members[1].connections[3] != null) _layoutCode += 4;
                if (members[0].connections[1] != null) _layoutCode += 2;
                if (members[1].connections[1] != null) _layoutCode += 1;
                return _layoutCode;
            }
        }

        public DungeonLongNode(
            DungeonNode left,
            DungeonNode right)
        {
            members = new[] { left, right };
        }

        public void draw()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(members[0].pos * 20 + new Vector2(20f, 10f), 4f);
            Gizmos.color = Color.white;
        }
    }

    public DungeonNode root;

    public DungeonGraph(Vector2 pos)
    {
        nodes = new List<DungeonNode>();
        bigNodes = new List<DungeonBigNode>();
        longNodes = new List<DungeonLongNode>();
        tallNodes = new List<DungeonTallNode>();
        drawnNodes = new List<DungeonNode>();
        root = new DungeonNode(pos, this);
        nodes.Add(root);
    }

    public void draw()
    {
        drawnNodes.Clear();
        root.draw();
        foreach (var bigNode in bigNodes)
        {
            bigNode.draw();
        }

        foreach (var longNode in longNodes)
        {
            longNode.draw();
        }

        foreach (var tallNode in tallNodes)
        {
            tallNode.draw();
        }
    }
    
    public void findBigRooms()
    {
        foreach (var node in nodes)
        {
            var neighbors = new DungeonNode[4];
            neighbors[0] = node;
            neighbors[1] = nodes.Find(elem => elem.pos == node.pos + Vector2.right);
            neighbors[2] = nodes.Find(elem => elem.pos == node.pos + Vector2.up);
            neighbors[3] = nodes.Find(elem => elem.pos == node.pos + Vector2.right + Vector2.up);
            
            //Left, down, right, up
            if ((neighbors[1] == null || neighbors[2] == null || neighbors[3] == null) ||
                (neighbors[0].connections[2] == null) ||
                (neighbors[1].connections[3] == null) ||
                (neighbors[3].connections[0] == null) ||
                (neighbors[2].connections[1] == null)) 
                continue;
            bigNodes.Add(new DungeonBigNode(neighbors[0], neighbors[1], neighbors[2], neighbors[3]));
        }

        foreach (var node in nodes)
        {
            var neighbors = new DungeonNode[2];
            neighbors[0] = node;
            neighbors[1] = nodes.Find(elem => elem.pos == node.pos + Vector2.right);
            if (neighbors[1] == null || neighbors[1].roomType != RoomType.Normal || neighbors[1].connections[0] == null) 
                continue;
            longNodes.Add(new DungeonLongNode(neighbors[0], neighbors[1]));
        }

        foreach (var node in nodes)
        {
            var neighbors = new DungeonNode[2];
            neighbors[0] = node;
            neighbors[1] = nodes.Find(elem => elem.pos == node.pos + Vector2.up);
            if (neighbors[1] == null || neighbors[1].roomType != RoomType.Normal || neighbors[1].connections[1] == null) 
                continue;
            tallNodes.Add(new DungeonTallNode(neighbors[0], neighbors[1]));
        }
    }
    
    public void filterBigRooms(float p, int minDistance)
    {
        var prefabs = Resources.LoadAll<BigRoomData>("RoomRoster/big").ToList();
        bigNodes = bigNodes.FindAll(bigNode =>
            bigNode.members[0].roomType == RoomType.Normal &&
            bigNode.members[1].roomType == RoomType.Normal &&
            bigNode.members[2].roomType == RoomType.Normal &&
            bigNode.members[3].roomType == RoomType.Normal);
        bigNodes = bigNodes.FindAll(bigNode =>
        {
            return Random.Range(0, 100) < p * 100 && prefabs.Exists(prefab => prefab.layoutCode == bigNode.layoutCode || prefab.mirroredLayoutCode == bigNode.layoutCode);
        });
        for (var i = 0; i < bigNodes.Count; i++)
        {
            var bigNode = bigNodes[i];
            var tooClose = bigNodes
                .FindAll(otherBigNode => Mathf.Abs(bigNode.pos.x - otherBigNode.pos.x) < minDistance + 1 && 
                                         Mathf.Abs(bigNode.pos.y - otherBigNode.pos.y) < minDistance + 1 &&
                                         bigNode != otherBigNode);

            foreach (var otherBigNode in tooClose)
            {
                bigNodes.Remove(otherBigNode);
                if (bigNodes.IndexOf(otherBigNode) < i)
                    i--;
            }
        }

        foreach (var bigNode in bigNodes)
        {
            bigNode.members[0].roomType = RoomType.Big;
            bigNode.members[1].roomType = RoomType.Big;
            bigNode.members[2].roomType = RoomType.Big;
            bigNode.members[3].roomType = RoomType.Big;
        }
    }

    public void filterLongRooms(float p, int minDistance)
    {
        var prefabs = Resources.LoadAll<LongRoomData>("RoomRoster/long").ToList();
        longNodes = longNodes.FindAll(elem => elem.members[0].roomType == RoomType.Normal && elem.members[1].roomType == RoomType.Normal);
        longNodes = longNodes.FindAll(longNode =>
        {
            return Random.Range(0, 100) < p * 100 && prefabs.Exists(prefab => prefab.layoutCode == longNode.layoutCode || prefab.mirroredLayoutCode == longNode.layoutCode);
        });
        for (var i = 0; i < longNodes.Count; i++)
        {
            var longNode = longNodes[i];
            var tooClose = longNodes
                .FindAll(otherLongNode => Mathf.Abs(longNode.pos.x - otherLongNode.pos.x) < minDistance + 1 &&
                                          Mathf.Abs(longNode.pos.y - otherLongNode.pos.y) < minDistance &&
                                          longNode != otherLongNode);
            foreach (var otherLongNode in tooClose)
            {
                longNodes.Remove(otherLongNode);
                if (longNodes.IndexOf(otherLongNode) < i)
                    i--;
            }
        }

        foreach (var longNode in longNodes)
        {
            longNode.members[0].roomType = RoomType.Long;
            longNode.members[1].roomType = RoomType.Long;
        }
    }

    public void filterTallRooms(float p, int minDistance)
    {
        var prefabs = Resources.LoadAll<TallRoomData>("RoomRoster/tall").ToList();
        tallNodes = tallNodes.FindAll(elem => elem.members[0].roomType == RoomType.Normal && elem.members[1].roomType == RoomType.Normal);
        tallNodes = tallNodes.FindAll(tallNode =>
        {
            return Random.Range(0, 100) < p * 100 && prefabs.Exists(prefab => prefab.layoutCode == tallNode.layoutCode || prefab.mirroredLayoutCode == tallNode.layoutCode);
        });
        for (var i = 0; i < tallNodes.Count; i++)
        {
            var tallNode = tallNodes[i];
            var tooClose = tallNodes
                .FindAll(otherTallNode => Mathf.Abs(tallNode.pos.x - otherTallNode.pos.x) < minDistance &&
                                          Mathf.Abs(tallNode.pos.y - otherTallNode.pos.y) < minDistance + 1 &&
                                          tallNode != otherTallNode);
            foreach (var otherTallNode in tooClose)
            {
                tallNodes.Remove(otherTallNode);
                if (tallNodes.IndexOf(otherTallNode) < i)
                    i--;
            }
        }

        foreach (var tallNode in tallNodes)
        {
            tallNode.members[0].roomType = RoomType.Tall;
            tallNode.members[1].roomType = RoomType.Tall;
        }
    }
}

public class LevelGenerator
{
    public DungeonGraph graph;

    public LevelGenerator()
    {
        graph = new DungeonGraph(Vector2.zero);
    }

    public void generateMainStructure(
        float p = 1f, 
        float pFall = 0.1f, 
        int maxDepth = 4, 
        int minRooms = 0, 
        int maxBranches = 4, 
        float branchingBias = 1f,
        float bigRoomP = 1f,
        int bigRoomDistance = 1)
    {
        graph.root.generateRandomly(p, pFall, 0, maxDepth, maxBranches);
        while (graph.nodes.Count <= minRooms)
        {
            var dungeonNodes = graph.nodes
                .FindAll(node => node.connections.Count(elem => elem != null) < 4)
                .OrderBy(node => node.depth)
                .ToArray();
            var result = Math.Log(branchingBias * (20 - 1) + 1, 20);
            var idx = (int)Math.Floor(result * (dungeonNodes.Length - 1));
            var dungeonNode = dungeonNodes.ElementAt(idx);
            var origin = Array.Find(dungeonNode.connections, conn => conn != null);
            dungeonNode.generateRandomly(p, pFall, dungeonNode.depth, dungeonNode.depth + maxDepth, maxBranches, origin);
        }
        createStartEndPoints();
        defineBigRooms(bigRoomP, bigRoomDistance);
        createScene();
        addEmptyBlackness();
    }

    private void addEmptyBlackness()
    {
        var minX = graph.nodes.Min(node => node.pos.x) - 2;
        var maxX = graph.nodes.Max(node => node.pos.x) + 2;
        var minY = graph.nodes.Min(node => node.pos.y) - 2;
        var maxY = graph.nodes.Max(node => node.pos.y) + 2;
        var blackSpaceHolder = GameObject.Find("BlackSpaceHolder");
        if (blackSpaceHolder != null)
            Object.DestroyImmediate(blackSpaceHolder);
        blackSpaceHolder = new GameObject("BlackSpaceHolder");
        var backgrounds = Resources.LoadAll("RoomRoster/special/background/outside");
        for (var x = minX; x <= maxX; x++)
        {
            for (var y = minY; y <= maxY; y++)
            {
                if (graph.nodes.Exists(node => node.pos.x == x && node.pos.y == y))
                    continue;
                var emptyBlackness = PrefabUtility.InstantiatePrefab(backgrounds[Random.Range(0, backgrounds.Length)]) as GameObject;
                emptyBlackness.transform.position = new Vector3(x * 20, y * 20, 0);
                emptyBlackness.transform.parent = blackSpaceHolder.transform;
            }
        }
        SceneManager.MoveGameObjectToScene(blackSpaceHolder, SceneManager.GetActiveScene());
    }

    private void createStartEndPoints()
    {
        var startNodes = graph.nodes.FindAll(node => node.pos.y == graph.nodes.Min(elem => elem.pos.y));
        var startNode = startNodes[Random.Range(0, startNodes.Count)];
        var newStartNode = new DungeonGraph.DungeonNode(startNode.pos + Vector2.down, graph){ roomType = RoomType.Start };
        startNode.connections[1] = newStartNode;
        newStartNode.connections[3] = startNode;
        graph.nodes.Add(newStartNode);
        var endNodes = graph.nodes.FindAll(node => node.pos.y == graph.nodes.Max(elem => elem.pos.y));
        var endNode = endNodes[Random.Range(0, endNodes.Count)];
        var newEndNode = new DungeonGraph.DungeonNode(endNode.pos + Vector2.up, graph){ roomType = RoomType.End };
        endNode.connections[3] = newEndNode;
        newEndNode.connections[1] = endNode;
        graph.nodes.Add(newEndNode);
    }

    private GameObject createNormalRooms()
    {
        var normalRoomHolder = new GameObject("NormalRoomHolder");
        var prefabs = Resources.LoadAll<RoomData>("RoomRoster/normal");
        var startPrefabs = Resources.LoadAll<RoomData>("RoomRoster/special/start");
        var endPrefabs = Resources.LoadAll<RoomData>("RoomRoster/special/end");
        foreach (var node in graph.nodes.FindAll(elem => elem.roomType is RoomType.Normal or RoomType.Start or RoomType.End))
        {
            var filteredPrefabs = node.roomType switch
            {
                RoomType.Start => startPrefabs.Where(prefab => prefab.layoutCode == node.layoutCode).ToArray(),
                RoomType.End => endPrefabs.Where(prefab => prefab.layoutCode == node.layoutCode).ToArray(),
                _ => prefabs.Where(prefab => prefab.layoutCode == node.layoutCode).ToArray()
            };
            var idx = Random.Range(0, filteredPrefabs.Length);
            var prefab = filteredPrefabs[idx];
            var room = PrefabUtility.InstantiatePrefab(prefab.gameObject) as GameObject;
            if (room == null) continue;
            room.gameObject.SetActive(true);
            room.transform.position = node.pos * 20;
            room.name = prefab.name + " | " + node.pos;
            room.transform.parent = normalRoomHolder.transform;
        }

        return normalRoomHolder;
    }

    private GameObject createBigRoomHolder()
    {
        var bigRoomHolder = new GameObject("BigRoomHolder");
        var bigPrefabs = Resources.LoadAll<BigRoomData>("RoomRoster/big");
        foreach (var bigNode in graph.bigNodes)
        {
            var filteredPrefabs = bigPrefabs
                .Where(prefab => 
                    prefab.layoutCode == bigNode.layoutCode || 
                    prefab.mirroredLayoutCode == bigNode.layoutCode)
                .ToArray();
            var idx = Random.Range(0, filteredPrefabs.Length);
            var prefab = filteredPrefabs[idx];
            if (prefab.mirroredLayoutCode == bigNode.layoutCode)
            {
                if (prefab.layoutCode != bigNode.layoutCode)
                    bigNode.mirrored = true;
                else
                    bigNode.mirrored = Random.Range(0, 2) == 0;
            }
            var room = PrefabUtility.InstantiatePrefab(prefab.gameObject) as GameObject;
            if (room == null) continue;
            room.gameObject.SetActive(true);
            room.transform.position = bigNode.pos * 20;
            if (bigNode.mirrored)
            {
                room.transform.localScale = new Vector3(-1, 1, 1);
                room.transform.position += new Vector3(40, 0, 0);
            }
            room.name = prefab.name + " | " + bigNode.pos;
            room.transform.parent = bigRoomHolder.transform;
        }

        return bigRoomHolder;
    }

    private GameObject createLongRoomHolder()
    {
        var longRoomHolder = new GameObject("LongRoomHolder");
        var longPrefabs = Resources.LoadAll<LongRoomData>("RoomRoster/long");
        foreach (var longNode in graph.longNodes)
        {
            var filteredPrefabs = longPrefabs
                .Where(prefab => 
                    prefab.layoutCode == longNode.layoutCode || 
                    prefab.mirroredLayoutCode == longNode.layoutCode)
                .ToArray();
            var idx = Random.Range(0, filteredPrefabs.Length);
            var prefab = filteredPrefabs[idx];
            if (prefab.mirroredLayoutCode == longNode.layoutCode)
            {
                if (prefab.layoutCode != longNode.layoutCode)
                    longNode.mirrored = true;
                else
                    longNode.mirrored = Random.Range(0, 2) == 0;
            }
            var room = PrefabUtility.InstantiatePrefab(prefab.gameObject) as GameObject;
            if (room == null) continue;
            room.gameObject.SetActive(true);
            room.transform.position = longNode.pos * 20;
            if (longNode.mirrored)
            {
                room.transform.localScale = new Vector3(-1, 1, 1);
                room.transform.position += new Vector3(40, 0, 0);
            }
            room.name = prefab.name + " | " + longNode.pos;
            room.transform.parent = longRoomHolder.transform;
        }

        return longRoomHolder;
    }

    private GameObject createTallRoomHolder()
    {
        var tallRoomHolder = new GameObject("TallRoomHolder");
        var tallPrefabs = Resources.LoadAll<TallRoomData>("RoomRoster/tall");
        foreach (var tallNode in graph.tallNodes)
        {
            var filteredPrefabs = tallPrefabs
                .Where(prefab => 
                    prefab.layoutCode == tallNode.layoutCode || 
                    prefab.mirroredLayoutCode == tallNode.layoutCode)
                .ToArray();
            var idx = Random.Range(0, filteredPrefabs.Length);
            var prefab = filteredPrefabs[idx];
            if (prefab.mirroredLayoutCode == tallNode.layoutCode)
            {
                if (prefab.layoutCode != tallNode.layoutCode)
                    tallNode.mirrored = true;
                else
                    tallNode.mirrored = Random.Range(0, 2) == 0;
            }
            var room = PrefabUtility.InstantiatePrefab(prefab.gameObject) as GameObject;
            if (room == null) continue;
            room.gameObject.SetActive(true);
            room.transform.position = tallNode.pos * 20;
            if (tallNode.mirrored)
            {
                room.transform.localScale = new Vector3(-1, 1, 1);
                room.transform.position += new Vector3(20, 0, 0);
            }
            room.name = prefab.name + " | " + tallNode.pos;
            room.transform.parent = tallRoomHolder.transform;
        }

        return tallRoomHolder;
    }

    private void createScene()
    {
        var roomHolder = GameObject.Find("RoomHolder");
        if (roomHolder != null)
            Object.DestroyImmediate(roomHolder);
        roomHolder = new GameObject("RoomHolder");

        var normalRoomHolder = createNormalRooms();
        var bigRoomHolder = createBigRoomHolder();
        var longRoomHolder = createLongRoomHolder();
        var tallRoomHolder = createTallRoomHolder();

        normalRoomHolder.transform.parent = roomHolder.transform;
        bigRoomHolder.transform.parent = roomHolder.transform;
        longRoomHolder.transform.parent = roomHolder.transform;
        tallRoomHolder.transform.parent = roomHolder.transform;

        SceneManager.MoveGameObjectToScene(roomHolder, SceneManager.GetActiveScene());
    }
    
    private void defineBigRooms(float p = 1f, int minDistance = 1)
    {
        graph.findBigRooms();
        graph.filterBigRooms(p, minDistance);
        graph.filterTallRooms(p, minDistance);
        graph.filterLongRooms(p, minDistance);
    }
}
