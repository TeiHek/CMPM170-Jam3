using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnit : BaseUnit
{
    // Start is called before the first frame update
    public override void Start()
    {
        SnapToGrid();
        AddUnit();
        hp = maxHp;
    }

    // Update is called once per frame
    public override void Update()
    {

    }

    public override void AddUnit()
    {
        GameManager.Instance.MapManager.AddEnemyUnit(GetTile(), this);
    }
}
