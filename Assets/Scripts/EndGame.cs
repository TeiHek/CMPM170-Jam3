using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.InputSystem;
using UnityEngine;

public class EndGame : MonoBehaviour
{
    [SerializeField] bool cheat;
    [SerializeField] GameObject _AiObject;
    EnemyAI _AI;

    [SerializeField] List<GameObject> importantUnits;

    [SerializeField] GameObject fadingObject;
    [SerializeField] Animator anim;

    //[Header("Important Condition Settings")]
    //[Tooltip("When this is on, trigger end game scene when all ally died.")]
    //[SerializeField] bool ifAllAllyDied;
    //[Tooltip("When this is on, trigger end game scene when all Enemy died.")]
    //[SerializeField] bool ifAllEnemyDied;


    int outputTime = 0;



    private void Awake()
    {
        outputTime = 0;
        _AI = _AiObject.GetComponent<EnemyAI>();
    }

    // Update is called once per frame
    void Update()
    {
        

        if(CheckCondition_Bad() && outputTime == 0)
        {
            badEndingEvent();
            outputTime++;
        }
        else if (CheckCondition_Good() && outputTime == 0)
        {
            goodEndingEvent();
            outputTime++;
        }


    }



    void goodEndingEvent()
    {

        if (outputTime == 0)
        {
            fadingObject.SetActive(true);
            anim.Play("Fading");

            Debug.Log("bad ending");
        }
    }

    void badEndingEvent()
    {
        if (outputTime == 0)
        {
            fadingObject.SetActive(true);
            anim.Play("Fading");

            Debug.Log("bad ending");
        }
    }

    bool CheckCondition_Bad()
    {
        if(_AI.AllyList.Count == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    bool CheckCondition_Good()
    {
        if (cheat)
        {
            return true;
        }
        if (_AI.EnemyList.Count == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }



}
