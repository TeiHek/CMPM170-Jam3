using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
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
        StartCoroutine(GameManager.Instance.EndTurn(GameState.EnemyTurn));
        Script_EnemyAI.AIProcess();


        button1.enabled = true;
    }


    public void Button_Quit()
    {

        button2.enabled = false;
        //dosomething

        button2.enabled = true;


    }



}
