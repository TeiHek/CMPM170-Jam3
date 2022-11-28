using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnControlButton : MonoBehaviour
{
    public Button button1;
    public Button button2;

    void Start()
    {
        button1.enabled = true;
        button2.enabled = true;
    }

    public void buttonFunction1()
    {
        //do something
        button1.enabled = false;
    }


    public void buttonFunction2()
    {
        //dosomething
        button2.enabled = false;
    }



}
