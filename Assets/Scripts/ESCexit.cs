using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ESCexit : MonoBehaviour
{
    public UnityEvent escclick;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("escape")){
            escexit2();
        }
    }

    void escexit2()
    {
        escclick.Invoke();
    }
}
