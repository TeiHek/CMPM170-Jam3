using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Range(0f, 1f), SerializeField] private float maxOnceAgainChance;
    private bool recentOnceAgain;
    public static GameManager Instance;
    public GameState state;
    [HideInInspector] public bool UIOpen;
    [HideInInspector] public bool listeningForTarget;

    public EnemyAI enemyAI;
    [Header("Assigned at runtime")]
    public MapManager MapManager;
    public InputController Controller;
    public PathFinder PathFinder;
    public RangeFinder rangeFinder;
    public UIMenuController UIMenuController;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        MapManager = GetComponentInChildren<MapManager>();
        Controller = GetComponentInChildren<InputController>();
        UIMenuController = GetComponentInChildren<UIMenuController>();
        PathFinder = new PathFinder();
        rangeFinder = new RangeFinder();
        state = GameState.PlayerTurn;
        UIOpen = false;
        listeningForTarget = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ProcessEndTurn()
    {
        int unitsWithAffinity = 0;
        int totalUnitsWithAffinity = 0;
        GameState newState = GameState.Setup;

        if (recentOnceAgain)
        {
            if (state == GameState.PlayerTurn)
            {
                newState = GameState.EnemyTurn;
            }
            else
            {
                newState = GameState.PlayerTurn;
            }
            recentOnceAgain = false;
        }
        else
        {
            //print("trying once again check");
            if (state == GameState.PlayerTurn)
            {
                foreach (KeyValuePair<Vector3Int, BaseUnit> tileUnitPair in MapManager.GetAllyUnits())
                {
                    if (tileUnitPair.Value.affinity != TileAffinity.None)
                    {
                        if (MapManager.GetTileAffinity(tileUnitPair.Key) == tileUnitPair.Value.affinity)
                        {
                            unitsWithAffinity++;
                            totalUnitsWithAffinity++;
                        }
                        else
                        {
                            totalUnitsWithAffinity++;
                        }
                    }
                   // print("Chance:" + unitsWithAffinity + "," + totalUnitsWithAffinity);
                }
               // print(((float)unitsWithAffinity / (float)totalUnitsWithAffinity) * maxOnceAgainChance);
                if (Random.Range(0f, 1f) < ((float)unitsWithAffinity / (float)totalUnitsWithAffinity) * maxOnceAgainChance)
                {
                    newState = GameState.PlayerTurn;
                    recentOnceAgain = true;
                }
                else
                {
                    newState = GameState.EnemyTurn;
                    recentOnceAgain = false;
                }
            }
            else
            {
                foreach (KeyValuePair<Vector3Int, BaseUnit> tileUnitPair in MapManager.GetEnemyUnits())
                {
                    if (tileUnitPair.Value.affinity != TileAffinity.None)
                    {
                        if (MapManager.GetTileAffinity(tileUnitPair.Key) == tileUnitPair.Value.affinity)
                        {
                            unitsWithAffinity++;
                            totalUnitsWithAffinity++;
                        }
                        else
                        {
                            totalUnitsWithAffinity++;
                        }
                    }
                    //print("Chance:" + unitsWithAffinity + "," + totalUnitsWithAffinity);
                }
                if (Random.Range(0f, 1f) < ((float)unitsWithAffinity / (float)totalUnitsWithAffinity) * maxOnceAgainChance)
                {
                    print("enemy again");
                    newState = GameState.EnemyTurn;
                    recentOnceAgain = true;
                }
                else
                {
                    newState = GameState.PlayerTurn;
                    recentOnceAgain = false;
                }
            }
        }
        StartCoroutine(EndTurn(newState));
    }

    public IEnumerator EndTurn(GameState nextState)
    {
        yield return new WaitForEndOfFrame();
        yield return StartCoroutine(UIMenuController.TurnFade(nextState, recentOnceAgain));
        state = nextState;
        if(nextState == GameState.PlayerTurn)
        {
            foreach (BaseUnit unit in MapManager.GetAllyUnits().Values)
            {
                unit.ableToAct = true;
            }
        }
        else if (nextState == GameState.EnemyTurn)
            enemyAI.AIProcess();
    }
}

public enum GameState
{
    Setup,
    PlayerTurn,
    EnemyTurn,
    UnitInTransit
}

public enum TileAffinity
{
    None,
    Grass,
    Forest,
    River,
    Mountain
}