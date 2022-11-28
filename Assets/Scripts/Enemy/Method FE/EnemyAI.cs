using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    // Ideally you would get the reference by getting it from MapManager.enemyUnits
    /* example:
     * foreach(BaseUnit enemy in GetEnemyUnits().Values)
     * {
     *   // Do something...
     * }
     */

    // There is no point where I convert Row/Column (Vector2 or Vector2Int) into Vector3Int, but for reference
    /*
     * to convert from Vector2Int to Vector3Int, create a new Vector3Int and use the x and y components of the Vector2:
     * Vector2Int test = new Vector2Int(2, 5);
     *\// To convert,
     * Vector3Int testButVec3 = new Vector3Int(test.x, test.y, 0);
     */

    // Using a test unit to have a reference to it for now.
    public BaseUnit testUnit;

    // Start is called before the first frame update
    void Start()
    {
       MoveUnit(testUnit, testUnit.GetTile(), new Vector3Int(-1, -2, 0) );
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveUnit(BaseUnit unit, Vector3Int start, Vector3Int end)
    {
        List<Vector3Int> travelPath = GameManager.Instance.PathFinder.FindPath(start, end, unit);
        StartCoroutine(unit.MovePosition(travelPath));
    }
}
