using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFinder
{
    struct PriorityTile
    {
        public Vector3Int pos;
        public int priority;

        public PriorityTile(Vector3Int position, int prio)
        {
            pos = position;
            priority = prio;
        }
    }

    struct ExploredTile
    {
        public Vector3Int prevTile;
        public int totalCost;

        public ExploredTile(Vector3Int pos, int cost)
        {
            prevTile = pos;
            totalCost = cost;
        }
    }
    
    public List<Vector3Int> FindPath(Vector3Int start, Vector3Int end)
    {
        // Create List for path to draw
        List<Vector3Int> path = new List<Vector3Int>();
        // Return case: Mouse is out of bounds/ end is out of bounds
        if(!GameManager.Instance.MapManager.InBounds(end))
        {
            return path;
        }

        // For some reason Unity does not support Priority Queue and I'm not about to make one
        List<PriorityTile> frontier = new List<PriorityTile>();
        Dictionary<Vector3Int, ExploredTile> explored = new Dictionary<Vector3Int, ExploredTile>();
        
        frontier.Add(new PriorityTile(start, 0));
        explored.Add(start, new ExploredTile(start, 0));
        
        while(frontier.Count > 0)
        {
            // Using OrderBy from Linq rather than Sort as it is stable
            // Use pseudo- priority list
            Vector3Int currentTile = frontier.OrderBy(tile => tile.priority).First().pos;
            frontier.RemoveAt(0);

            if (currentTile == end)
            {
                // Return finalized list
                return GetFinalPath(start, end, explored);
            }

            List<Vector3Int> neighboringTiles = GetNeighboringTiles(currentTile);
            foreach(Vector3Int neighbor in neighboringTiles)
            {
                int newCost = explored[currentTile].totalCost + GameManager.Instance.MapManager.GetMoveCost(neighbor);
                if(!explored.ContainsKey(neighbor) || newCost < explored[neighbor].totalCost)
                {
                    int priority = newCost + getManHattanDistance(end, neighbor);
                    frontier.Add(new PriorityTile(neighbor, priority));
                    if (!explored.ContainsKey(neighbor))
                    {
                        explored.Add(neighbor, new ExploredTile(currentTile, newCost));
                    }
                    else
                    {
                        ExploredTile newData = explored[neighbor];
                        newData.totalCost = newCost;
                        explored[neighbor] = newData;
                    }
                }
            }
        }
        return path;
    }

    private List<Vector3Int> GetNeighboringTiles(Vector3Int current)
    {
        List<Vector3Int> neighbors = new List<Vector3Int>();
        // Check Up
        if (GameManager.Instance.MapManager.InBounds(current + Vector3Int.up))
        {
            neighbors.Add(current + Vector3Int.up);
        }
        // Check Down
        if(GameManager.Instance.MapManager.InBounds(current + Vector3Int.down))
        {
            neighbors.Add(current + Vector3Int.down);
        }
        // Check Left
        if(GameManager.Instance.MapManager.InBounds(current + Vector3Int.left))
        {
            neighbors.Add(current + Vector3Int.left);
        }
        // Check Right
        if(GameManager.Instance.MapManager.InBounds(current + Vector3Int.right))
        {
            neighbors.Add(current + Vector3Int.right);
        }
        return neighbors;
    }

    private int getManHattanDistance(Vector3Int a, Vector3Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    private List<Vector3Int> GetFinalPath(Vector3Int start, Vector3Int end, Dictionary<Vector3Int, ExploredTile> explored)
    {
        List<Vector3Int> finalPath = new List<Vector3Int>();
        Vector3Int currentTile = end;

        while(currentTile != start)
        {
            finalPath.Add(currentTile);
            currentTile = explored[currentTile].prevTile;
        }
        finalPath.Add(currentTile);
        finalPath.Reverse();
        return finalPath;
    }
}
