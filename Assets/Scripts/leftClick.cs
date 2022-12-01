using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class leftClick : MonoBehaviour
{
    public UnityEvent Oneclick;
    // Start is called before the first frame update

    void OnMouseDown()
    {
        Click3();
    }

    private void Click3(){
        Oneclick.Invoke();
    }  
}
