using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    
    public BaseUnit testUnit;
    private Vector3 startPoint;
    private Vector3 endPoint;
    private GameObject desPoint;



    private void Awake()
    {
        if( !(desPoint = GameObject.Find("Test Point")))
        {
            Debug.Log("Destination Point is not found");
        }

        endPoint = new Vector3(desPoint.transform.position.x, desPoint.transform.position.y, 0);
    }


    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(Vector3Int.FloorToInt(endPoint));

        MoveUnit(testUnit, testUnit.GetTile(), Vector3Int.FloorToInt(endPoint));
       Debug.Log(GameManager.Instance.PathFinder.FindPath(testUnit.GetTile(), Vector3Int.FloorToInt(endPoint), testUnit).Count);


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
