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
    [SerializeField] private Tile allyOverlayAttackTile;
    [SerializeField] private Tile enemyOverlayMoveTile;
    [SerializeField] private Tile enemyOverlayAttackTile;


    [Header("Tile Data")]
    [SerializeField] private List<TileData> tileDataList;
    private Dictionary<TileBase, TileData> tileData;

    // Interactive Layer
    private Vector3Int mouseGridPos = new Vector3Int();
    private Vector3Int prevMousePos = new Vector3Int();
    private Vector3Int lastMousePosInBounds = new Vector3Int();

    // Unit tracking
    private Dictionary<Vector3Int, BaseUnit> allyUnits;
    private Dictionary<Vector3Int, BaseUnit> enemyUnits;
    private BaseUnit selectedUnit;
    private Vector3Int selectedUnitPos;
    private Vector3Int targetTile;

    // Start is called before the first frame update
    void Start()
    {
        selectedUnit = null;
    }

    private void Awake()
    {
        allyUnits = new Dictionary<Vector3Int, BaseUnit>();
        enemyUnits = new Dictionary<Vector3Int, BaseUnit>();
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
            if (!mouseGridPos.Equals(prevMousePos) && !GameManager.Instance.UIOpen && !GameManager.Instance.listeningForTarget)
            {
                interactiveMap.SetTile(prevMousePos, null);
                interactiveMap.SetTile(mouseGridPos, hoverTile);
                prevMousePos = mouseGridPos;
                lastMousePosInBounds = mouseGridPos;
                pathMap.ClearAllTiles();
                if ( (overlayMap.ContainsTile(allyOverlayMoveTile) || overlayMap.ContainsTile(enemyOverlayMoveTile)) && selectedUnit == null)
                {
                    overlayMap.ClearAllTiles();
                    pathMap.ClearAllTiles();
                }
                if (!GameManager.Instance.UIOpen && selectedUnit == null)
                {
                    ShowMoveAttackRange(mouseGridPos);
                }
                if (selectedUnit != null)
                {
                    List<Vector3Int> path = GameManager.Instance.PathFinder.FindPath(selectedUnitPos, mouseGridPos, selectedUnit);
                    foreach (Vector3Int step in path)
                    {
                        pathMap.SetTile(step, pathTile);
                    }
                }
            }
        }
        else
        {
            interactiveMap.SetTile(lastMousePosInBounds, null);
        }
    }

    public Vector3Int GetMousePosition()
    {
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return worldMap.WorldToCell(mouseWorldPosition);
    }

    public bool IsInBounds(Vector3Int pos)
    {
        return worldMap.HasTile(pos);
    }

    // Get the move cost for a tile at a given position
    public int GetMoveCost(Vector3Int pos)
    {
        TileBase tile = worldMap.GetTile(pos);
        return tileData[tile].GetMoveCost();
    }

    public TileAffinity GetTileAffinity(Vector3Int pos)
    {
        TileBase tile = worldMap.GetTile(pos);
        return tileData[tile].GetTileAffinity();
    }

    // Get whether or not a tile is navigable at a given position
    public bool IsNavigable(Vector3Int pos)
    {
        TileBase tile = worldMap.GetTile(pos);
        return tileData[tile].IsNavigable();
    }

    public void SelectTargetTile(Vector3Int pos)
    {
        targetTile = pos;
    }

    public Vector3Int GetTargetTile()
    {
        return targetTile;
    }

    #region Adding/Removing/Updating Units
    public void AddAllyUnit(Vector3Int pos, BaseUnit unit)
    {
        allyUnits.Add(pos, unit);
    }

    public void AddEnemyUnit(Vector3Int pos, BaseUnit unit)
    {
        enemyUnits.Add(pos, unit);
    }

    public void removeUnit(Vector3Int pos)
    {
        if (allyUnits.ContainsKey(pos))
        {
            allyUnits.Remove(pos);
        }
        else if (enemyUnits.ContainsKey(pos))
        {
            enemyUnits.Remove(pos);
        }
        // Should not remove nothing, debug line
        //else print("No unit to remove");
    }

    public void UpdateUnitLocation(BaseUnit unit, Vector3Int prevPos, Vector3Int newPos)
    {
        if (allyUnits.ContainsKey(prevPos))
        {
            allyUnits.Remove(prevPos);
            allyUnits.Add(newPos, unit);
        }
        else if (enemyUnits.ContainsKey(prevPos))
        {
            enemyUnits.Remove(prevPos);
            enemyUnits.Add(newPos, unit);
        }
        else print("No unit to update");
    }
    #endregion

    #region Unit Checking
    public bool IsUnit(Vector3Int pos)
    {
        return allyUnits.ContainsKey(pos) || enemyUnits.ContainsKey(pos);
    }
    public bool IsAllyUnit(Vector3Int pos)
    {
        return allyUnits.ContainsKey(pos);
    }

    public bool IsEnemyUnit(Vector3Int pos)
    {
        return enemyUnits.ContainsKey(pos);
    }

    public bool IsOtherFaction(BaseUnit unit, BaseUnit otherUnit)
    {
        if (unit == null || otherUnit == null)
            return false;

        if (unit.GetType() == otherUnit.GetType())
            return false;
        else return true;
    }
    public BaseUnit GetSelectedUnit()
    {
        return selectedUnit;
    }

    public Dictionary<Vector3Int, BaseUnit> GetAllyUnits()
    {
        return allyUnits;
    }

    #endregion

    #region Unit Info
    public BaseUnit GetUnitAt(Vector3Int pos)
    {
        if(allyUnits.ContainsKey(pos))
        {
            return allyUnits[pos];
        }
        else if (enemyUnits.ContainsKey(pos))
        {
            return enemyUnits[pos];
        }
        // No unit found
        return null;
    }

    public Dictionary<Vector3Int, BaseUnit> GetEnemyUnits()
    {
        return enemyUnits;
    }

    // Checks if ANY enemy is in attack range
    public bool EnemyInAttackRange(Vector3Int pos, int minAttackRange, int maxAttackRange)
    {
        List<Vector3Int> AttackRange = GameManager.Instance.rangeFinder.FindRangeRadius(pos, minAttackRange, maxAttackRange, selectedUnit, false);
        foreach(Vector3Int tile in AttackRange)
        {
            if (enemyUnits.ContainsKey(tile))
                return true;
        }
        return false;
    }

    // Checks if target is in attack range of selected unit
    public bool EnemyInAttackRange(BaseUnit target)
    {
        if (target == null)
            return false;
        List<Vector3Int> attackRange = GameManager.Instance.rangeFinder.FindRangeRadius(targetTile, selectedUnit.minAttackRange, selectedUnit.maxAttackRange, selectedUnit, false);
        if (attackRange.Contains(target.GetTile()))
            return true;
        else return false;
    }

    public bool InSelectedUnitMoveRange(Vector3Int pos)
    {
        List<Vector3Int> moveRange = GameManager.Instance.rangeFinder.FindRange(selectedUnitPos, selectedUnit.GetMoveRange(), selectedUnit);
        if (moveRange.Contains(pos))
            return true;
        else return false;
    }
    #endregion

    #region Unit Controls
    public void SelectUnit(Vector3Int pos)
    {
        if (!IsUnit(pos))
        {
            return;
        }
        selectedUnit = GetUnitAt(pos);
        selectedUnitPos = pos;
    }

    public void DeselectUnit()
    {
        selectedUnit = null;
        ClearMoveAttackTiles();
    }

    public void MoveSelectedUnit(Vector3Int pos)
    {
        BaseUnit unit = selectedUnit;
        DeselectUnit();
        List<Vector3Int> travelPath = GameManager.Instance.PathFinder.FindPath(selectedUnitPos, pos, unit);
        List<Vector3Int> travelRange = GameManager.Instance.rangeFinder.FindRange(selectedUnitPos, unit.GetMoveRange(), unit);
        if(travelPath.Count-1 > unit.GetMoveRange() || !travelRange.Contains(pos))
        {
            return;
        }
        StartCoroutine(unit.MovePosition(travelPath));
        unit.ableToAct = false;
    }

    // Pos = Where the unit should move to
    // Target = The unit to attack
    public void MoveAttack(Vector3Int pos, BaseUnit target)
    {
        BaseUnit unit = selectedUnit;
        DeselectUnit();
        List<Vector3Int> travelPath = GameManager.Instance.PathFinder.FindPath(selectedUnitPos, pos, unit);
        List<Vector3Int> travelRange = GameManager.Instance.rangeFinder.FindRange(selectedUnitPos, unit.GetMoveRange(), unit);
        if (travelPath.Count - 1 > unit.GetMoveRange() || !travelRange.Contains(pos))
        {
            return;
        }
        StartCoroutine(unit.MoveAttack(travelPath, target));
        unit.ableToAct = false;
    }
    #endregion

    public void DebugClick()
    {
        // Test block
        if (worldMap.HasTile(mouseGridPos))
        {
            TileBase clickedTile = worldMap.GetTile(mouseGridPos);
           // print("Position" + mouseGridPos + ", Move Cost:" + tileData[clickedTile].GetMoveCost() + ", Has unit: " + IsUnit(mouseGridPos) + "Unit: " + GetUnitAt(mouseGridPos));
        }
    }

    public void ShowMoveAttackRange(Vector3Int pos)
    {
        BaseUnit unit = GetUnitAt(pos);
        if (unit == null)
        {
            return;
        }

        int unitMoveRange = unit.GetMoveRange();
        List<Vector3Int> moveRange = GameManager.Instance.rangeFinder.FindRange(pos, unitMoveRange, unit);
        List<Vector3Int> attackRange = GameManager.Instance.rangeFinder.FindRangeRadius(pos, unitMoveRange, unitMoveRange + unit.GetMaxAttackRange(), unit, true);
        if (IsAllyUnit(pos))
        {
            foreach (Vector3Int tile in moveRange)
            {
                overlayMap.SetTile(tile, allyOverlayMoveTile);
            }
            foreach (Vector3Int tile in attackRange)
            {
                overlayMap.SetTile(tile, allyOverlayAttackTile);
            }
        }
        else
        {
            foreach (Vector3Int tile in moveRange)
            {
                overlayMap.SetTile(tile, enemyOverlayMoveTile);
            }
            foreach (Vector3Int tile in attackRange)
            {
                overlayMap.SetTile(tile, enemyOverlayAttackTile);
            }
        }
    }

    public void ShowAttackTiles(Vector3Int pos, BaseUnit unit)
    {
        if (unit == null)
        {
            return;
        }

        List<Vector3Int> attackRange = GameManager.Instance.rangeFinder.FindRangeRadius(pos, unit.GetMinAttackRange(), unit.GetMaxAttackRange(), unit, false);
        foreach (Vector3Int tile in attackRange)
        {
            overlayMap.SetTile(tile, allyOverlayAttackTile);
        }
    }

    public void ClearMoveAttackTiles()
    {
        overlayMap.ClearAllTiles();
        pathMap.ClearAllTiles();
    }
}
