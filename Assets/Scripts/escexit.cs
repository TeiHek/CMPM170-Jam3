using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class escexit : MonoBehaviour
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
            escclick2();
        }
    }

    void escclick2(){
        escclick.Invoke();
    }
}
