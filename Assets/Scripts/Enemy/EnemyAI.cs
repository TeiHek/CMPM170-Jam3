using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using static UnityEngine.UI.CanvasScaler;

public class EnemyAI : MonoBehaviour
{

    public GameObject _EventObject;
    private TaranEvent _TaranEvent;


    [System.Serializable]
    public struct Units
    {
        public GameObject gameObject;
        [HideInInspector] public BaseUnit baseunit;
    }

    public float movementFrequency;

    public List<Units> EnemyList = new List<Units>();
    public List<Units> AllyList = new List<Units>();

    //helper variables for storing list data
    private GameObject EnemyUnits;
    private GameObject AllyUnits;


    //private Vector3Int Destination;
    private List<Vector3Int> tempPath;

    //public BaseUnit attacker;
    //public BaseUnit targeter;

    private void Awake()
    {
        _TaranEvent = _EventObject.GetComponent<TaranEvent>();
        //init Lists of allys and enemys
        buildList();


        //MoveAttack(attacker, targeter);

    }

    //------------------------------------------------------------------------------
    //------------------------------ Functions -------------------------------------
    // in case u don't know my rules, but only functions will be called in Start/Awake/Update
    // helper functions only called in function

    //this bool will determine if the all ai movement is complete
    [HideInInspector] public bool isAIComplete = false;



    // this functions will apply movement and attack on all Enemy units 
    //  rules: enemy will find closest player's units and get close to it, once it appoach the attack arange, it attacks
    //  wait second will be used for pause
    IEnumerator runAI()
    {

        isAIComplete = false;

        //rebuild the list to remove all inactive cases
        buildList_Enemy();
        for (int i = 0; i < EnemyList.Count; i++)
        {
            buildList_Ally();
            UnitHeadToUnitAI(EnemyList[i].baseunit);
            yield return new WaitUntil(() => GameManager.Instance.state == GameState.EnemyTurn);

        }
        //this uncontrollable character will move to the house on the left top for the event
        _TaranEvent.TaranMovement();

        yield return new WaitForSeconds(movementFrequency);
        isAIComplete = true;
        if(AllyList.Count != 0)
        {
            GameManager.Instance.ProcessEndTurn();
        }
 
    }

    public void AIProcess()
    {
        //run ai if it has Enemy
        if (EnemyList.Count >= 0)
        {
            StartCoroutine(runAI());
        }

    }







    //AI move Unit1 to most proper tile 
    public void UnitHeadToUnitAI(BaseUnit Unit)
    {
        TargetList.Clear();
        BuildTarget();
        if(TargetList.Count == 0)
        {
            return;
        }

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


        for (int i = 0; i < TargetList.Count; i++)
        {
            for (int p = 0; p < TargetList[i].allAdjs.Count; p++)
            {
                distance = Vector3Int.Distance(Unit.GetTile(), TargetList[i].allAdjs[p]);
                //get travel path, range, attack range details
                List<Vector3Int> travelPath = GameManager.Instance.PathFinder.FindPath(Unit.GetTile(), TargetList[i].allAdjs[p], Unit);
                //check 3 things, if it's the closest ally unit
                //if the tile has something on it
                //if the path is valid
                if (distance < distanceMin
                    && isTileHavingUnit(TargetList[i].allAdjs[p]) == false
                    && travelPath.Count > 1)
                {

                    distanceMin = distance;
                    MinIndex = p;
                    targetIndex = i;
                    resultTile = TargetList[i].allAdjs[p];
                    resultTarget = TargetList[i].allyUnit.gameObject;
                }// distance check end

                //it won't move if it's already close to target
                if (travelPath.Count == 1)
                {
                    distanceMin = distance;
                    MinIndex = p;
                    targetIndex = i;
                    resultTile = TargetList[i].allAdjs[p];
                    resultTarget = TargetList[i].allyUnit.gameObject;
                }


            }
        }
        MoveAttack(Unit, TargetList[targetIndex].allyUnit.baseunit, resultTile);
    }




    public void MoveAttack(BaseUnit attacker, BaseUnit target, Vector3Int resultTile)
    {
        int pathLength;
        pathLength = GameManager.Instance.PathFinder.FindPath(attacker.GetTile(), resultTile, attacker).Count + 1;

        int attackRange = attacker.maxAttackRange;
        int moveRange = attacker.moveRange;
        if (pathLength - 1 <= attackRange)
        {
            //attack if it's on range
            //this means it will not move, and just attack
            AttackBetweenTwo(attacker, target);
        }
        else if (attackRange + moveRange >= pathLength - 1)
        {
            //move and attack
            StartCoroutine(waitSecondsForAttack(1, attacker, target, resultTile));
        }
        else if (attackRange + moveRange < pathLength - 1)
        {
            //just move
            UnitHeadToTile(attacker, resultTile);
        }
    }





    //AI move Unit1 TO Unit2's possible Adj area
    public void UnitHeadToUnit(BaseUnit Unit1, BaseUnit ToUnit2)
    {
        Vector3Int destination;
        List<Vector3Int> availablePath = new List<Vector3Int>();
        float distance = 0;
        float distanceMin = 999;
        Vector3Int resultTile = Unit1.GetTile();   //avoid it jump to the black hole if invalid case happened
        int MinIndex = -1;


        //test all adj's valiablity. note i = 0 is self tile, so starts from 1
        for (int i = 1; i < 5; i++)
        {
            destination = getUnitAdjacentTiles(ToUnit2)[i];

            //test if tile is used before the path check
            if (isTileHavingUnit(destination) == false)
            {
                //check if it's valid path
                if (GameManager.Instance.PathFinder.FindPath(ToUnit2.GetTile(), destination, ToUnit2).Count > 0)
                {
                    //if it's valid it will be stored in the list
                    availablePath.Add(destination);
                    distance = Vector3Int.Distance(Unit1.GetTile(), destination);
                    //check closest tile as target tile
                    if (distance < distanceMin)
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
        buildList_Enemy();
        buildList_Ally();
    }



    //build eneny list before the battle starts
    void buildList_Enemy()
    {
        EnemyList.Clear();
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
    }

    //build ally list when the battle happened, so it will be dynamic if ally died
    void buildList_Ally()
    {
        AllyList.Clear();
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
                        if (isTileHavingUnit(destination) == false)
                        {
                            resultDestination = destination;
                            break;
                        }
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

            for (int i = 0; i < tempPath.Count; i++)
            {

                if (isTileHavingUnit(destination) == false)
                {
                    MoveUnit(unit, unit.GetTile(), destination);
                    break;
                }
                else
                {
                    if (unit.moveRange - 1 - i == 0)
                    {
                        //not move
                        break;
                    }
                    destination = tempPath[unit.moveRange - 1 - i];
                }
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



    //helper function for enmey attack
    IEnumerator waitSecondsForAttack(float waitTime, BaseUnit attacker, BaseUnit target, Vector3Int resultTile)
    {
        //move unit
        UnitHeadToTile(attacker, resultTile);
        yield return new WaitForSeconds(waitTime);
        //then attack
        AttackBetweenTwo(attacker, target);
    }

    //helper function for enmey attack
    void AttackBetweenTwo(BaseUnit attacker, BaseUnit target)
    {
        //bad use for function MoveAttack(travelPath, target), but this will make it not move, moving is handled by my functions
        List<Vector3Int> travelPath = GameManager.Instance.PathFinder.FindPath(attacker.GetTile(), attacker.GetTile(), attacker);
        StartCoroutine(attacker.MoveAttack(travelPath, target));
    }

    //move unit from start to end
    void MoveUnit(BaseUnit unit, Vector3Int start, Vector3Int end)
    {
        List<Vector3Int> travelPath = GameManager.Instance.PathFinder.FindPath(start, end, unit);
        StartCoroutine(unit.MovePosition(travelPath));
    }



    //------------------------------------------------------------------------------
}
