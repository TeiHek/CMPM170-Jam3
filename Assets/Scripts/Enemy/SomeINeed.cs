using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool movingEnd(BaseUnit movingUnit)
    {
        return true;
        // return true once the moving is finished after calling MoveUnit(BaseUnit unit, Vector3Int start, Vector3Int end)
    }

    public void enablePlayerControll(bool input)
    {
        //if input is true; let player to control their units
        //if input is false; let player NOT control their units (when Enemy starts)
    }

    public void combat_CheckRange(BaseUnit attacker, BaseUnit defender)
    {
        //I will implement this (Joker)
        //Just make sure the range things works
    }



}
