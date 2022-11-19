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
    [SerializeField] private Tile hoverTile;
    [SerializeField] private RuleTile pathTile;

    [Header("Tile Data")]
    [SerializeField] private List<TileData> tileDataList;
    private Dictionary<TileBase, TileData> tileData;

    // Interactive Layer
    private Vector3Int mouseGridPos = new Vector3Int();
    private Vector3Int prevMousePos = new Vector3Int();
    private Vector3Int lastMousePosInBounds = new Vector3Int();

    // Unit tracking
    private Dictionary<BaseUnit, Vector3Int> units;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void Awake()
    {
        units = new Dictionary<BaseUnit, Vector3Int>();
        tileData = new Dictionary<TileBase, TileData>();
        foreach(TileData TD in tileDataList)
        {
            foreach(Tile tile in TD.tiles)
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
            List<Vector3Int> path = GameManager.Instance.PathFinder.FindPath(Vector3Int.zero, mouseGridPos);
            foreach (Vector3Int step in path)
            {
                pathMap.SetTile(step, pathTile);
            }
        }
        else
        {
            interactiveMap.SetTile(lastMousePosInBounds, null);
        }


    }

    private Vector3Int GetMousePosition()
    {
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return worldMap.WorldToCell(mouseWorldPosition);
    }

    public void AddUnit(BaseUnit unit, Vector3Int pos)
    {
        units.Add(unit, pos);
    }

    public bool InBounds(Vector3Int pos)
    {
        return worldMap.HasTile(pos);
    }

    public void DebugClick()
    {
        // Test block
        if (worldMap.HasTile(mouseGridPos))
        {
            TileBase clickedTile = worldMap.GetTile(mouseGridPos);
            print("Position" + mouseGridPos + ", Move Cost:" + tileData[clickedTile].GetMoveCost() + ", Has unit: " + units.ContainsValue(mouseGridPos));
        }
    }

    public int GetMoveCost(Vector3Int pos)
    {
        TileBase tile = worldMap.GetTile(pos);
        return tileData[tile].GetMoveCost();
    }
}
