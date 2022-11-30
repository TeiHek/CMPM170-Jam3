using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick()
    {
        //print("attempting move to " + GameManager.Instance.MapManager.GetTargetTile());
        GameManager.Instance.MapManager.MoveSelectedUnit(GameManager.Instance.MapManager.GetTargetTile());
        GameManager.Instance.UIMenuController.HideActionButtons();
    }
}
