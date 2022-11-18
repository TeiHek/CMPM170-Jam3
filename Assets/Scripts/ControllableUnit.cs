using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllableUnit : BaseUnit
{
    // Start is called before the first frame update
    public override void Start()
    {
        GetNearestTile();
    }

    // Update is called once per frame
    public override void Update()
    {
        
    }
}
