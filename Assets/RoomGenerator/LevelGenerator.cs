using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEditor.PlayerSettings;
using Gizmos = UnityEngine.Gizmos;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class DungeonGraph
{

    public List<DungeonNode> nodes;
    public List<DungeonBigNode> bigNodes;
    public List<DungeonNode> drawnNodes;
    public class DungeonNode
    {
        public int depth;
        public DungeonGraph parent;
        public DungeonNode[] connections;
        public bool inCompoundRoom;
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
                return _layoutCode;;
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
                Gizmos.DrawLine(pos * 20 + Vector2.one * 10, (node.pos + pos) * 10 + Vector2.one * 10);
                if (parent.drawnNodes.Exists(elem => node.pos == elem.pos))
                    continue;
                node.draw();
            }
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
            Gizmos.color = layoutCode == 255 ? Color.red : Color.green;
            Gizmos.DrawSphere(members[0].pos * 20 + Vector2.one * 20, 4f);
            Gizmos.color = Color.white;
        }
    }

    public DungeonNode root;

    public DungeonGraph(Vector2 pos)
    {
        nodes = new List<DungeonNode>();
        bigNodes = new List<DungeonBigNode>();
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
    }

    //TODO
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
            if ((neighbors[1] != null && neighbors[2] != null && neighbors[3] != null) && 
                (neighbors[0].connections[2] != null) && 
                (neighbors[1].connections[3] != null) &&
                (neighbors[3].connections[0] != null) &&
                (neighbors[2].connections[1] != null))
                bigNodes.Add(new DungeonBigNode(neighbors[0], neighbors[1], neighbors[2], neighbors[3]));
        }
    }

    //TODO
    public void filterBigRooms(float p, int minDistance)
    {
        var prefabs = Resources.LoadAll<BigRoomData>("RoomRoster/big").ToList();
        bigNodes = bigNodes.FindAll(bigNode =>
        {
            return Random.Range(0, 100) < p * 100 && prefabs.Exists(prefab => prefab.layoutCode == bigNode.layoutCode || prefab.mirroredLayoutCode == bigNode.layoutCode);
        });
        for (var i = 0; i < bigNodes.Count; i++)
        {
            var bigNode = bigNodes[i];
            var tooClose = bigNodes
                .FindAll(otherBigNode => Mathf.Abs(bigNode.pos.x - otherBigNode.pos.x) < minDistance + 1 && 
                                         Mathf.Abs(bigNode.pos.y - otherBigNode.pos.y) < minDistance + 1)
                .Where(otherBigNode => bigNode != otherBigNode);

            foreach (var otherBigNode in tooClose)
            {
                bigNodes.Remove(otherBigNode);
                if (bigNodes.IndexOf(otherBigNode) < i)
                    i--;
            }
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
        defineBigRooms(bigRoomP, bigRoomDistance);
        createScene();
    }

    public void createScene()
    {
        var roomHolder = GameObject.Find("RoomHolder");
        if (roomHolder != null)
            Object.DestroyImmediate(roomHolder);
        roomHolder = new GameObject("RoomHolder");
        var prefabs = Resources.LoadAll<RoomData>("RoomRoster/normal");
        foreach (var node in graph.nodes.FindAll(elem => !elem.inCompoundRoom))
        {
            var filteredPrefabs = prefabs.Where(prefab => prefab.layoutCode == node.layoutCode).ToArray();
            var idx = Random.Range(0, filteredPrefabs.Length);
            var prefab = filteredPrefabs[idx];
            var room = PrefabUtility.InstantiatePrefab(prefab.gameObject) as GameObject;
            if (room == null) continue;
            room.gameObject.SetActive(true);
            room.transform.position = node.pos * 20;
            room.name = prefab.name + " | " + node.pos;
            room.transform.parent = roomHolder.transform;
        }
        SceneManager.MoveGameObjectToScene(roomHolder, SceneManager.GetActiveScene());

        var bigRoomHolder = GameObject.Find("BigRoomHolder");
        if (bigRoomHolder != null)
            Object.DestroyImmediate(bigRoomHolder);
        bigRoomHolder = new GameObject("BigRoomHolder");
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
    }
    
    public void defineBigRooms(float p = 1f, int minDistance = 1)
    {
        graph.findBigRooms();
        graph.filterBigRooms(p, minDistance);
        foreach (var member in graph.bigNodes.SelectMany(node => node.members))
        {
            member.inCompoundRoom = true;
        }
    }
}
