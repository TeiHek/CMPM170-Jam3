using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class TileData : ScriptableObject
{
    public List<TileBase> tiles;
    [SerializeField] private int moveCost;

    public int GetMoveCost()
    {
        return moveCost;
    }
}
