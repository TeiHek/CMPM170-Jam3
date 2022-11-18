using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseUnit : MonoBehaviour
{
    public int maxHp;
    public int hp;
    public int moveRange;
    public int minAttackRange;
    public int maxAttackRange;

    private Vector3Int gridPos;

    // Start is called before the first frame update
    public virtual void Start()
    {
        
    }

    // Update is called once per frame
    public virtual void Update()
    {
        
    }

    protected void GetNearestTile()
    {
        // Snap the unit to the closest tile and update the tiledata
        gridPos = GameManager.Instance.MapManager.grid.WorldToCell(transform.position);
        transform.position = GameManager.Instance.MapManager.grid.CellToWorld(gridPos);
        // Move unit to center of tile rather than corner of tile
        transform.position += GameManager.Instance.MapManager.grid.cellSize / 2;
        GameManager.Instance.MapManager.AddUnit(this, gridPos);
    }

}
