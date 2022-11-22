using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    [Header("Map Data")]
    public Grid grid;
    [SerializeField] private Tilemap worldMap;
    [SerializeField] private Tilemap interactiveMap;
    [SerializeField] private Tilemap pathMap;
    [SerializeField] private Tilemap overlayMap;

    [SerializeField] private Tile hoverTile;
    [SerializeField] private RuleTile pathTile;
    [SerializeField] private Tile allyOverlayMoveTile;

    [Header("Tile Data")]
    [SerializeField] private List<TileData> tileDataList;
    private Dictionary<TileBase, TileData> tileData;

    // Interactive Layer
    private Vector3Int mouseGridPos = new Vector3Int();
    private Vector3Int prevMousePos = new Vector3Int();
    private Vector3Int lastMousePosInBounds = new Vector3Int();

    // Unit tracking
    private Dictionary<Vector3Int, BaseUnit> units;
    private BaseUnit selectedUnit;
    private Vector3Int selectedUnitPos;

    // Start is called before the first frame update
    void Start()
    {
        selectedUnit = null;
    }

    private void Awake()
    {
        units = new Dictionary<Vector3Int, BaseUnit>();
        tileData = new Dictionary<TileBase, TileData>();
        foreach(TileData TD in tileDataList)
        {
            foreach(TileBase tile in TD.tiles)
            {
                tileData.Add(tile, TD);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        mouseGridPos = GetMousePosition();
        
        if (worldMap.HasTile(mouseGridPos))
        {
            if (!mouseGridPos.Equals(prevMousePos))
            {
                interactiveMap.SetTile(prevMousePos, null);
                interactiveMap.SetTile(mouseGridPos, hoverTile);
                prevMousePos = mouseGridPos;
                lastMousePosInBounds = mouseGridPos;
                pathMap.ClearAllTiles();
            }
        }
        else
        {
            interactiveMap.SetTile(lastMousePosInBounds, null);
        }

        if(selectedUnit)
        {
            List<Vector3Int> range = GameManager.Instance.rangeFinder.FindRange(selectedUnitPos, selectedUnit.GetMoveRange(), selectedUnit);
            foreach(Vector3Int tile in range)
            {
                overlayMap.SetTile(tile, allyOverlayMoveTile);
            }
            List<Vector3Int> path = GameManager.Instance.PathFinder.FindPath(selectedUnitPos, mouseGridPos, selectedUnit);
            foreach (Vector3Int step in path)
            {
                pathMap.SetTile(step, pathTile);
            }
        }
        else
        {
            overlayMap.ClearAllTiles();
            pathMap.ClearAllTiles();
        }
    }

    public Vector3Int GetMousePosition()
    {
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return worldMap.WorldToCell(mouseWorldPosition);
    }

    public void AddUnit(Vector3Int pos, BaseUnit unit)
    {
        units.Add(pos, unit);
    }

    public bool IsInBounds(Vector3Int pos)
    {
        return worldMap.HasTile(pos);
    }

    public void DebugClick()
    {
        // Test block
        if (worldMap.HasTile(mouseGridPos))
        {
            TileBase clickedTile = worldMap.GetTile(mouseGridPos);
            print("Position" + mouseGridPos + ", Move Cost:" + tileData[clickedTile].GetMoveCost() + ", Has unit: " + IsAlliedUnit(mouseGridPos));
        }
    }

    // Get the move cost for a tile at a given position
    public int GetMoveCost(Vector3Int pos)
    {
        TileBase tile = worldMap.GetTile(pos);
        return tileData[tile].GetMoveCost();
    }

    // Get whether or not a tile is navigable at a given position
    public bool IsNavigable(Vector3Int pos)
    {
        TileBase tile = worldMap.GetTile(pos);
        return tileData[tile].IsNavigable();
    }

    public bool IsAlliedUnit(Vector3Int pos)
    {
        if (!units.ContainsKey(pos)) return false;
        return units[pos].GetType() == typeof(ControllableUnit);
    }

    public BaseUnit GetSelectedUnit()
    {
        return selectedUnit;
    }

    // Overload for unit at position
    public BaseUnit GetUnitAt(Vector3Int pos)
    {
        if (!units.ContainsKey(pos))
        {
            return null;
        }
        return units[pos];
    }

    public void SelectUnit(Vector3Int pos)
    {
        if (!units.ContainsKey(pos))
        {
            return;
        }
        selectedUnit = units[pos];
        selectedUnitPos = pos;
    }

    public void DeselectUnit()
    {
        selectedUnit = null;
    }

    public void MoveSelectedUnit(Vector3Int pos)
    {
        BaseUnit unit = selectedUnit;
        DeselectUnit();
        List<Vector3Int> travelPath = GameManager.Instance.PathFinder.FindPath(selectedUnitPos, pos, unit);
        if(travelPath.Count > unit.GetMoveRange())
        {
            return;
        }
        units.Remove(selectedUnitPos);
        StartCoroutine(unit.MovePosition(travelPath));
    }
}
