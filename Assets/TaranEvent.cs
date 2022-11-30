using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TaranEvent : MonoBehaviour
{

    public BaseUnit EventUnit_Taran;
    public Vector3Int TargetDestination;
    //private Vector3Int Destination;
    private List<Vector3Int> tempPath;

    public void TaranMovement()
    {
        if (EventUnit_Taran.GetTile() != TargetDestination)
        {
            UnitHeadToTile(EventUnit_Taran, TargetDestination);
        }

    }

    //move unit from start to end
    void MoveUnit(BaseUnit unit, Vector3Int start, Vector3Int end)
    {
        List<Vector3Int> travelPath = GameManager.Instance.PathFinder.FindPath(start, end, unit);
        StartCoroutine(unit.MovePosition(travelPath));
    }


    //make unit head to the destination tile due to the movment range
    void UnitHeadToTile(BaseUnit unit, Vector3Int destination)
    {

        // assignPath to temp variable
        tempPath = new List<Vector3Int>(GameManager.Instance.PathFinder.FindPath(unit.GetTile(), destination, unit));
        Vector3Int resultDestination = destination;
        if (unit.moveRange > tempPath.Count)
        {
            //des is bigger than range
            if (tempPath.Count <= 0)
            {
                Debug.Log("Error: Unable to find path");
            }
            else
            {
                //find the moveable tiles
                for (int i = 0; i < tempPath.Count; i++)
                {
                    if (tempPath.Count - 1 - i <= 0)
                    {
                        //don't move
                        MoveUnit(unit, unit.GetTile(), unit.GetTile());
                    }
                    else
                    {
                        //find closest possible tile to move
                        destination = tempPath[tempPath.Count - 1 - i];
                    }
                }
                MoveUnit(unit, unit.GetTile(), resultDestination);
            }

        }
        else if (unit.moveRange <= tempPath.Count && unit.moveRange > 0)
        {

            //des is bigger than range
            if (unit.moveRange == tempPath.Count)
            {
                destination = tempPath[unit.moveRange - 1];
            }
            else
            {
                destination = tempPath[unit.moveRange];
            }

        }


    }

}

