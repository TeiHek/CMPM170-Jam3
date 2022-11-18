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
    [SerializeField] private Tile hoverTile;

    [Header("Tile Data")]
    [SerializeField] private List<TileData> tileDataList;
    private Dictionary<TileBase, TileData> tileData;

    // Interactive Layer
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
        Vector3Int mouseGridPos = GetMousePosition();
        if (worldMap.HasTile(mouseGridPos))
        {
            if (!mouseGridPos.Equals(prevMousePos))
            {
                interactiveMap.SetTile(prevMousePos, null);
                interactiveMap.SetTile(mouseGridPos, hoverTile);
                prevMousePos = mouseGridPos;
                lastMousePosInBounds = mouseGridPos;
            }
        }
        else
        {
            interactiveMap.SetTile(lastMousePosInBounds, null);
        }

        // Test block
        if (Input.GetMouseButtonDown(0))
        {
            if (worldMap.HasTile(mouseGridPos))
            {
                TileBase clickedTile = worldMap.GetTile(mouseGridPos);
                print("Position" + mouseGridPos + ", Move Cost:" + tileData[clickedTile].GetMoveCost() + ", Has unit: " + units.ContainsValue(mouseGridPos));
            }
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
}
