using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
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

        public DungeonBigNode(
            DungeonNode bottomLeft,
            DungeonNode topLeft,
            DungeonNode topRight,
            DungeonNode bottomRight)
        {
            members = new[] { bottomLeft, topLeft, topRight, bottomRight };
        }
    }

    public DungeonNode root;

    public DungeonGraph(Vector2 pos)
    {
        nodes = new List<DungeonNode>();
        drawnNodes = new List<DungeonNode>();
        root = new DungeonNode(pos, this);
        nodes.Add(root);
    }

    public void draw()
    {
        drawnNodes.Clear();
        root.draw();
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
            if (neighbors[1] != null && neighbors[2] != null && neighbors[3] != null)
                bigNodes.Add(new DungeonBigNode(neighbors[0], neighbors[1], neighbors[2], neighbors[3]));
        }
    }

    //TODO
    public void filterBigRooms(float p, bool contigious)
    {
        foreach (var bigNode in bigNodes)
        {
            var randThreshold = Random.Range(0, 100);
            if (randThreshold < p * 100)
            {
                if (contigious) continue;
                var neighbor = bigNodes.First(); //TODO
                if (neighbor != null)
                    bigNodes.Remove(bigNode);
            }
            else
            {
                bigNodes.Remove(bigNode);
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

    public void generateMainStructure(float p = 1f, float pFall = 0.1f, int maxDepth = 4, int minRooms = 0, int maxBranches = 4, float branchingBias = 1f)
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
        createScene();
    }

    public void createScene()
    {
        var roomHolder = GameObject.Find("RoomHolder");
        if (roomHolder != null)
            Object.DestroyImmediate(roomHolder);
        roomHolder = new GameObject("RoomHolder");
        var prefabs = Resources.LoadAll<RoomData>("RoomRoster");
        foreach (var node in graph.nodes)
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
    }

    //TODO
    public void defineBigRooms(float p = 1f, bool contigious = false)
    {
        graph.findBigRooms();
        graph.filterBigRooms(p, contigious);
    }
}
