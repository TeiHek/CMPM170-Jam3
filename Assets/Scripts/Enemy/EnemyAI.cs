using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using static UnityEngine.UI.CanvasScaler;

public class EnemyAI : MonoBehaviour
{
    [System.Serializable]
    public struct Units{
        public GameObject gameObject;
        [HideInInspector]public BaseUnit baseunit;
    }


     public List<Units> EnemyList = new List<Units>();
     public List<Units> AllyList = new List<Units>();

    //helper variables for storing list data
    private GameObject EnemyUnits;
    private GameObject AllyUnits;


    //private Vector3Int Destination;
    private List<Vector3Int> tempPath;


    private void Awake()
    {
        //init Lists of allys and enemys
        buildList();


    }

    //------------------------------------------------------------------------------
    //------------------------------ Functions -------------------------------------
    // in case u don't know my rules, but only functions will be called in Start/Awake/Update
    // helper functions only called in function

    public BaseUnit attacker;
    public BaseUnit targeter;

    private void Start()
    {
        MoveAttack(attacker, targeter);
    }


    // attacker = the Unit that is attacking
    // pos = Where the unit should move to
    // target = The unit to attack
    public void MoveAttack(BaseUnit attacker, BaseUnit target)
    {
        int attackRange = attacker.maxAttackRange;
        int moveRange = attacker.moveRange;
        List<Vector3Int> travelPath = GameManager.Instance.PathFinder.FindPath(attacker.GetTile(), target.GetTile(), attacker);

        Debug.Log(attacker.GetTile());
        Debug.Log(target.GetTile());
        Debug.Log(travelPath.Count);

        if (travelPath.Count <= attackRange)
        {
            //attack if it's on range
            Debug.Log("on range");
        }
        else
        {
            //move first then attack if it's out of range before attack
            Debug.Log("out of range");
        }


        //if (travelPath.Count - 1 > target.GetMoveRange())
        //{
        //    return;
        //}
        //StartCoroutine(attacker.MoveAttack(travelPath, target));
    }






    //this bool will determine if the all ai movement is complete
    [HideInInspector]public bool isAIComplete = false;



    // this functions will apply movement and attack on all Enemy units 
    //  rules: enemy will find closest player's units and get close to it, once it appoach the attack arange, it attacks
    //  wait second will be used for pause
    IEnumerator runAI()
    {

        isAIComplete = false;

        //rebuild the list to remove all inactive cases
        buildList();

        for (int i = 0; i < EnemyList.Count; i++)
        {
            UnitHeadToUnitAI(EnemyList[i].baseunit);
            yield return new WaitForSeconds(0.25f);
            yield return new WaitUntil(() => GameManager.Instance.state == GameState.EnemyTurn);
        }

        isAIComplete = true;
        GameManager.Instance.state = GameState.PlayerTurn;
    }

    public void AIProcess()
    {
        StartCoroutine(runAI());
    }


   




    //AI move Unit1 to most proper tile 
    public void UnitHeadToUnitAI(BaseUnit Unit)
    {
        TargetList.Clear();
        BuildTarget();

        List<Vector3Int> availablePath = new List<Vector3Int>();

        //float targetDistance = 0;
        //float targetDistanceMin = 999;
        //float targetIndex = -1;

        float distance = 0;
        float distanceMin = 999;
        Vector3Int resultTile = Unit.GetTile();   //avoid it jump to the black hole if invalid case happened
        int MinIndex = -1;
        int targetIndex = -1;

        GameObject resultTarget;


        for(int i = 0; i < TargetList.Count; i++)
        {
            for(int p = 0; p < TargetList[i].allAdjs.Count; p++)
            {
                distance = Vector3Int.Distance(Unit.GetTile(), TargetList[i].allAdjs[p]);

                //check 3 things, if it's the closest ally unit
                //if the tile has something on it
                //if the path is valid
                if (distance < distanceMin 
                    && isTileHavingUnit(TargetList[i].allAdjs[p]) == false
                    && GameManager.Instance.PathFinder.FindPath(Unit.GetTile(), TargetList[i].allAdjs[p], Unit).Count > 1)
                {

                    distanceMin = distance;
                    MinIndex = p;
                    targetIndex =i;
                    resultTile = TargetList[i].allAdjs[p];
                    resultTarget = TargetList[i].allyUnit.gameObject;
                }// distance check end

                if(GameManager.Instance.PathFinder.FindPath(Unit.GetTile(), TargetList[i].allAdjs[p], Unit).Count == 1)
                {
                    distanceMin = distance;
                    MinIndex = p;
                    targetIndex = i;
                    resultTile = TargetList[i].allAdjs[p];
                    resultTarget = TargetList[i].allyUnit.gameObject;
                }

                //Debug.Log(GameManager.Instance.PathFinder.FindPath(EnemyList[0].baseunit.GetTile(), AllyList[0].baseunit.GetTile(), EnemyList[0].baseunit).Count);
            }
        }
        //Debug.Log("the result is " + "Unit " + targetIndex + " tile " + resultTile);
       // Debug.Log("")
       UnitHeadToTile(Unit, resultTile);
    }







    //AI move Unit1 TO Unit2's possible Adj area
    public void UnitHeadToUnit(BaseUnit Unit1, BaseUnit ToUnit2)
    {
        Vector3Int destination;
        List<Vector3Int> availablePath = new List<Vector3Int>();
        float distance = 0;
        float distanceMin = 999;
        Vector3Int resultTile = Unit1.GetTile() ;   //avoid it jump to the black hole if invalid case happened
        int MinIndex = -1;
      

        //test all adj's valiablity. note i = 0 is self tile, so starts from 1
        for (int i = 1; i < 5; i++)
        {
            destination = getUnitAdjacentTiles(ToUnit2)[i];

            //test if tile is used before the path check
            if(isTileHavingUnit(destination) == false)
            {
                //check if it's valid path
                if (GameManager.Instance.PathFinder.FindPath(ToUnit2.GetTile(), destination, ToUnit2).Count > 0)
                {
                    //if it's valid it will be stored in the list
                    availablePath.Add(destination);
                    distance = Vector3Int.Distance(Unit1.GetTile(), destination);
                    //check closest tile as target tile
                    if(distance < distanceMin)
                    {
                        distanceMin = distance;
                        MinIndex = i;
                        resultTile = destination;
                    }// distance check end
                }
            }

        }

        UnitHeadToTile(Unit1, resultTile);
    }





    //------------------------------------------------------------------------------
    //------------------------------ Helper Functions ------------------------------
    //only be called in function

    void buildList()
    {
        EnemyList.Clear();
        AllyList.Clear();

        EnemyUnits = GameObject.Find("*Enemy/*Units");

        for (int i = 0; i < EnemyUnits.transform.childCount; i++)
        {
            if (EnemyUnits.transform.GetChild(i).gameObject.activeSelf)
            {
                //active will be stored
                Units tempUnit = new Units();
                tempUnit.gameObject = EnemyUnits.transform.GetChild(i).gameObject;
                tempUnit.baseunit = tempUnit.gameObject.GetComponent<EnemyUnit>();
                EnemyList.Add(tempUnit);
            }

        }

        AllyUnits = GameObject.Find("*Ally/*Units");

        for (int i = 0; i < AllyUnits.transform.childCount; i++)
        {
            if (AllyUnits.transform.GetChild(i).gameObject.activeSelf)
            {
                Units tempUnit = new Units();
                tempUnit.gameObject = AllyUnits.transform.GetChild(i).gameObject;
                tempUnit.baseunit = tempUnit.gameObject.GetComponent<ControllableUnit>();
                AllyList.Add(tempUnit);
            }

        }
    }
    




    [HideInInspector]
    public struct target
    {
        [HideInInspector] public Units allyUnit;
        [HideInInspector] public List<Vector3Int> allAdjs;
    }

    List<target> TargetList = new List<target>();

    //build all possible targets for AI
    void BuildTarget()
    {
        target tempTarget;
        List<Vector3Int> adjTiles;

        foreach (Units ally in AllyList)
        {
            tempTarget = new target();
            tempTarget.allyUnit = ally;
            adjTiles = new List<Vector3Int>();
            adjTiles = getUnitAdjacentTiles(ally.baseunit);
            adjTiles.RemoveAt(0);
            tempTarget.allAdjs = adjTiles;
            TargetList.Add(tempTarget);
        }
    }





    //function that test if certain tile is occupied by any unit
    bool isTileHavingUnit(Vector3Int tile)
    {
        //test if Enemy on the tile
        foreach (Units unit in EnemyList)
        {
            if (unit.baseunit.GetTile() == tile)
            {
                return true;
            }
        }
        //test if Ally on the tile
        foreach (Units unit in AllyList)
        {
            if (unit.baseunit.GetTile() == tile)
            {
                return true;
            }
        }

        return false;
    }


    //get adjacent tiles of the unit
    //  at index 0 will be the unit position,
    //  index 1 is top tile, 2 is down tile, 3 is left, 4 is right
    List<Vector3Int> getUnitAdjacentTiles(BaseUnit unit)
    {
        List<Vector3Int> adjacentTiles = new List<Vector3Int>();
        adjacentTiles.Add(unit.GetTile());
        adjacentTiles.Add(new Vector3Int(unit.GetTile().x, unit.GetTile().y + 1, 0));
        adjacentTiles.Add(new Vector3Int(unit.GetTile().x, unit.GetTile().y - 1, 0));
        adjacentTiles.Add(new Vector3Int(unit.GetTile().x - 1, unit.GetTile().y, 0));
        adjacentTiles.Add(new Vector3Int(unit.GetTile().x + 1, unit.GetTile().y, 0));
        return adjacentTiles;
    }


    //make unit head to the destination tile due to the movment range
    void UnitHeadToTile(BaseUnit unit, Vector3Int destination)
    {
        // assignPath to temp variable
        tempPath = new List<Vector3Int>(GameManager.Instance.PathFinder.FindPath(unit.GetTile(), destination, unit));

        if (unit.moveRange > tempPath.Count)
        {
            //des is bigger than range
            if (tempPath.Count <= 0)
            {
                Debug.Log("Error: Unable to find path");
            }
            else
            {
                destination = tempPath[tempPath.Count - 1];
                MoveUnit(unit, unit.GetTile(), destination);
            }

        }
        else if (unit.moveRange <= tempPath.Count && unit.moveRange > 0)
        {
            //des is smaller than range

            if(unit.moveRange == tempPath.Count)
            {
                destination = tempPath[unit.moveRange - 1];
            }
            else
            {
                destination = tempPath[unit.moveRange];
            }
            

            if (isTileHavingUnit(destination) == false)
            {
                MoveUnit(unit, unit.GetTile(), destination);
            }
            else
            {
                destination = tempPath[unit.moveRange - 1];
                MoveUnit(unit, unit.GetTile(), destination);
            }


        }
        else if (unit.moveRange <= 0)
        {
            Debug.Log("Error: No moverange has been set for this unit");
        }
    }

    //return unit the EnemyUnit is goint to attack
    Units findClosestTarget(BaseUnit EnemyUnit)
    {
        float distance;
        float closestDistance = 999;
        int targetedAllyIndex = -1;

        for (int i = 0; i < AllyList.Count; i++)
        {
            distance = Vector3Int.Distance(EnemyUnit.GetTile(), AllyList[i].baseunit.GetTile());
            //compare and contrast
            if (distance < closestDistance)
            {
                closestDistance = distance;
                targetedAllyIndex = i;
            }
        }//for end

        return AllyList[targetedAllyIndex];
    }


    //move unit from start to end
    void MoveUnit(BaseUnit unit, Vector3Int start, Vector3Int end)
    {
        List<Vector3Int> travelPath = GameManager.Instance.PathFinder.FindPath(start, end, unit);
        StartCoroutine(unit.MovePosition(travelPath));
    }



    //------------------------------------------------------------------------------
}
