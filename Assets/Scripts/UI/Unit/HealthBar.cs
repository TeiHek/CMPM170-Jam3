using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HealthBar : MonoBehaviour
{

    private float current_HP;
    private float MAX_HP ;
    private float percentage_HP;

    GameObject ParentObj;
    BaseUnit ParentUnit;
    GameObject HPBar;
    Vector3 localHPBar_Scale;

    private void Awake()
    {
        ParentObj = this.gameObject.transform.parent.gameObject;
        ParentUnit = ParentObj.GetComponent<BaseUnit>();

        MAX_HP = ParentUnit.maxHp;
        current_HP = ParentUnit.hp;
        percentage_HP = 1;
        HPBar = this.gameObject.transform.GetChild(0).GameObject();
        localHPBar_Scale = HPBar.transform.localScale;
    }
    // Update is called once per frame
    void Update()
    {
        current_HP = ParentUnit.hp;
        percentage_HP = current_HP / MAX_HP;
        HPBar.transform.localScale = new Vector3(localHPBar_Scale.x * percentage_HP, 1, 1);
    }
}
