using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    [Header("Map Data")]
    [SerializeField] private Tilemap worldMap;
    [SerializeField] private Tilemap interactiveMap;
    [SerializeField] private Tile hoverTile;

    [Header("Tile Data")]
    [SerializeField] private List<TileData> tileDataList;
    private Dictionary<TileBase, TileData> tileData;

    private Vector3Int prevMousePos = new Vector3Int();

    // Start is called before the first frame update
    void Start()
    {

    }

    private void Awake()
    {
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
        if(!mouseGridPos.Equals(prevMousePos))
        {
            interactiveMap.SetTile(prevMousePos, null);
            interactiveMap.SetTile(mouseGridPos, hoverTile);
            prevMousePos = mouseGridPos;
        }

        // Test block
        if (Input.GetMouseButtonDown(0))
        {
            TileBase clickedTile = worldMap.GetTile(mouseGridPos);
            print("Position" + mouseGridPos + ", Move Cost:" + tileData[clickedTile].moveCost);
        }

    }

    Vector3Int GetMousePosition()
    {
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return worldMap.WorldToCell(mouseWorldPosition);
    }
}
