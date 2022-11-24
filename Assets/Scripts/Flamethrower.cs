using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Flamethrower : MonoBehaviour
{
    public VisualEffect ft;
    public VisualEffect health;
    public VisualEffect thunder;
    // Start is called before the first frame update
    void Start()
    {
        ft.Stop();
        health.Stop();
        thunder.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        // flamethrower
        if (Input.GetKeyDown("1"))
        {
            ft.Play();
        }
        if (Input.GetKeyUp("1"))
        {
            ft.Stop();
        }

        // health
        if (Input.GetKeyDown("2"))
        {
            health.Play();
        }
        if (Input.GetKeyUp("2"))
        {
            health.Stop();
        }

        // thunder
        if (Input.GetKeyDown("3"))
        {
            thunder.Play();
        }
        if (Input.GetKeyUp("3"))
        {
            thunder.Stop();
        }
    }
}
