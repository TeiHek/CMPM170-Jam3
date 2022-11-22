using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class TileData : ScriptableObject
{
    public List<TileBase> tiles;
    [Tooltip("Check this if a unit should be able to stand on this tile")]
    [SerializeField] private bool navigable;
    [SerializeField] private int moveCost;

    public int GetMoveCost()
    {
        return moveCost;
    }

    public bool IsNavigable()
    {
        return navigable;
    }
}
