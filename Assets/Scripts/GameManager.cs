using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
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

    public IEnumerator EndTurn(GameState nextState)
    {
        yield return StartCoroutine(UIMenuController.TurnFade(nextState));
        state = nextState;
        if (nextState == GameState.EnemyTurn)
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