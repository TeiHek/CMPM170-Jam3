using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // This will eventually become the script to track game state. Don't have that yet though
    [Header("Assigned at runtime")]
    public MapManager MapManager;
    public InputController Controller;
    public PathFinder PathFinder;

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
        PathFinder = new PathFinder();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
