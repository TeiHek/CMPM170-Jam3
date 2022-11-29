using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Flamethrower : MonoBehaviour
{
    public VisualEffect ft;
    public VisualEffect health;
    public VisualEffect thunder;
    public VisualEffect magic;
    public VisualEffect impact;
    public VisualEffect slash;
    // Start is called before the first frame update
    void Start()
    {
        ft.Stop();
        health.Stop();
        thunder.Stop();
        magic.Stop();
        impact.Stop();
        slash.Stop();
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

        // magic
        if (Input.GetKeyDown("4"))
        {
            magic.Play();
        }
        if (Input.GetKeyUp("4"))
        {
            magic.Stop();
        }

        // impact
        if (Input.GetKeyDown("5"))
        {
            impact.Play();
        }
        if (Input.GetKeyUp("5"))
        {
            impact.Stop();
        }

        // slash
        if (Input.GetKeyDown("6"))
        {
            slash.Play();
        }
        if (Input.GetKeyUp("6"))
        {
            slash.Stop();
        }
    }
}
