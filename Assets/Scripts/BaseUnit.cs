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
    public bool ableToAct;

    // Start is called before the first frame update
    public virtual void Start()
    {
        
    }

    // Update is called once per frame
    public virtual void Update()
    {
        
    }

    public int GetMoveRange()
    {
        return moveRange;
    }
    public IEnumerator MovePosition(List<Vector3Int> path)
    {
        float step = 4 * Time.deltaTime;
        GameState prevState = GameManager.Instance.state;
        GameManager.Instance.state = GameState.UnitInTransit;
        while (path.Count > 0)
        {
            Vector3 midTile = GameManager.Instance.MapManager.grid.CellToWorld(path[0]) + GameManager.Instance.MapManager.grid.cellSize / 2;
            transform.position = Vector2.MoveTowards(transform.position, midTile, step);

            if(Vector2.Distance(transform.position, midTile) < 0.0001f)
            {
                path.RemoveAt(0);
            }
            yield return null;
        }
        AddLocation();
        yield return new WaitForSeconds(0.05f);
        GameManager.Instance.state = prevState;
    }

    private Vector3Int GetNearestTile()
    {
        // Snap the unit to the closest tile and update the tiledata
        return GameManager.Instance.MapManager.grid.WorldToCell(transform.position);
    }

    protected void SnapToGrid()
    {
        transform.position = GameManager.Instance.MapManager.grid.CellToWorld(GetNearestTile()) + GameManager.Instance.MapManager.grid.cellSize / 2;
    }

    protected void AddLocation()
    {
        GameManager.Instance.MapManager.AddUnit(GetNearestTile(), this);
    }

}
