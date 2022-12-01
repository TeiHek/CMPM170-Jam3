using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Oneclickfunction : MonoBehaviour
{
    public UnityEvent DoubleClick;
    // Start is called before the first frame update

    public void OnMouseOver()
    {
        if(Input.GetMouseButtonDown(1)){
            Click2();
        }
    }

    private void Click2(){
        SoundManager.PlaySound("sfx_MouseButton", 1);
        DoubleClick.Invoke();
    }    
}
