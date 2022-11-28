using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TurnControlButton : MonoBehaviour
{
    public Button button1;
    public Button button2;

    private EnemyAI Script_EnemyAI;


    private void Awake()
    {
        Script_EnemyAI = GameObject.Find("*AI").GetComponent<EnemyAI>();

    }


    void Start()
    {
        button1.enabled = true;
        button2.enabled = true;
    }

    public void Button_TurnControl()
    {
        button1.enabled = false;
        //do something
        //  Script_EnemyAI.UnitHeadTo(Script_EnemyAI.Unit1, Script_EnemyAI.endPoint);

        Script_EnemyAI.AIProcess();
        //Script_EnemyAI.UnitHeadToUnit(Script_EnemyAI.EnemyList[0].baseunit, Script_EnemyAI.AllyList[0].baseunit);

        button1.enabled = true;
    }


    public void Button_Quit()
    {

        button2.enabled = false;
        //dosomething

        button2.enabled = true;


    }



}
