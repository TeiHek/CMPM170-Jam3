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
        SoundManager.PlaySound("sfx_MouseButton", 1);
        GameManager.Instance.ProcessEndTurn();
        button1.enabled = true;
    }


    public void Button_Quit()
    {

        button2.enabled = false;
        SoundManager.PlaySound("sfx_MouseButton", 1);
        Application.Quit();
        button2.enabled = true;


    }



}
