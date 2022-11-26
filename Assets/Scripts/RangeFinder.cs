using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RangeFinder
{
    public List<Vector3Int> FindRange(Vector3Int start, int maxRange, BaseUnit unit)
    {
        // Create List for path to draw
        List<Vector3Int> path = new List<Vector3Int>();

        List<Vector3Int> frontier = new List<Vector3Int>();
        Dictionary<Vector3Int, int> explored = new Dictionary<Vector3Int, int>();

        frontier.Add(start);
        explored.Add(start, 0);

        while (frontier.Count > 0)
        {
            // Get next item in the list
            Vector3Int currentTile = frontier[0];
            frontier.RemoveAt(0);

            List<Vector3Int> neighboringTiles = GetNeighboringTiles(currentTile, unit);
            foreach (Vector3Int neighbor in neighboringTiles)
            {
                int newCost = explored[currentTile] + GameManager.Instance.MapManager.GetMoveCost(neighbor);
                if (!explored.ContainsKey(neighbor) && newCost < maxRange+1)
                {
                    frontier.Add(neighbor);
                    explored.Add(neighbor, newCost);
                }
            }
        }
        return GetFinalRange(explored, start);
    }

    private List<Vector3Int> GetNeighboringTiles(Vector3Int current, BaseUnit unit)
    {
        List<Vector3Int> neighbors = new List<Vector3Int>();
        // Check Up
        if (GameManager.Instance.MapManager.IsInBounds(current + Vector3Int.up) && GameManager.Instance.MapManager.IsNavigable(current + Vector3Int.up) && (
            GameManager.Instance.MapManager.GetUnitAt(current + Vector3Int.up) == null || GameManager.Instance.MapManager.GetUnitAt(current + Vector3Int.up).GetType() != unit.GetType()))
        {
            neighbors.Add(current + Vector3Int.up);
        }
        // Check Down
        if (GameManager.Instance.MapManager.IsInBounds(current + Vector3Int.down) && GameManager.Instance.MapManager.IsNavigable(current + Vector3Int.down) && (
            GameManager.Instance.MapManager.GetUnitAt(current + Vector3Int.down) == null || GameManager.Instance.MapManager.GetUnitAt(current + Vector3Int.down).GetType() != unit.GetType()))
        {
            neighbors.Add(current + Vector3Int.down);
        }
        // Check Left
        if (GameManager.Instance.MapManager.IsInBounds(current + Vector3Int.left) && GameManager.Instance.MapManager.IsNavigable(current + Vector3Int.left) && (
            GameManager.Instance.MapManager.GetUnitAt(current + Vector3Int.left) == null || GameManager.Instance.MapManager.GetUnitAt(current + Vector3Int.left).GetType() != unit.GetType()))
        {
            neighbors.Add(current + Vector3Int.left);
        }
        // Check Right
        if (GameManager.Instance.MapManager.IsInBounds(current + Vector3Int.right) && GameManager.Instance.MapManager.IsNavigable(current + Vector3Int.right) && (
            GameManager.Instance.MapManager.GetUnitAt(current + Vector3Int.right) == null || GameManager.Instance.MapManager.GetUnitAt(current + Vector3Int.right).GetType() != unit.GetType()))
        {
            neighbors.Add(current + Vector3Int.right);
        }
        return neighbors;
    }

    private List<Vector3Int> GetFinalRange(Dictionary<Vector3Int, int> explored, Vector3Int start)
    {
        List<Vector3Int> finalRange = new List<Vector3Int>();
        foreach(Vector3Int tile in explored.Keys)
        {
            // Skip over tiles with allied units
            if(GameManager.Instance.MapManager.GetUnitAt(tile) != null && tile != start)
            {
                continue;
            }
            finalRange.Add(tile);
        }
        
        return finalRange;
    }
}
