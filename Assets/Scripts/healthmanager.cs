using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class healthmanager : MonoBehaviour
{
    public Text TextHealthUI;

    private float current_HP;
    private float MAX_HP ;

    private int _scr;
    // Start is called before the first frame update
    GameObject ParentObj;
    BaseUnit ParentUnit;

    private void Awake()
    {
        ParentObj = this.gameObject.transform.parent.gameObject;
        ParentUnit = ParentObj.GetComponent<BaseUnit>();

        MAX_HP = ParentUnit.maxHp;
        current_HP = ParentUnit.hp;
        TextHealthUI.text = current_HP.ToString();
    }

    void Update()
    {
        current_HP = ParentUnit.hp;
        TextHealthUI.text = current_HP.ToString() + "/" + MAX_HP.ToString();
    }
}
