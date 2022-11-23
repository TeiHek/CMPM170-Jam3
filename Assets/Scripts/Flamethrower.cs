using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Flamethrower : MonoBehaviour
{
    public VisualEffect ft;
    // Start is called before the first frame update
    void Start()
    {
        ft.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ft.Play();
        }
        if (Input.GetMouseButtonUp(0))
        {
            ft.Stop();
        }
    }
}
