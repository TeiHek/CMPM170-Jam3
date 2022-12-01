using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{

    public static AudioClip sfx_playerHit;
    static AudioSource audioSrc;



    // Start is called before the first frame update

    void Start()
    {
        sfx_playerHit = Resources.Load<AudioClip>("/SFX/InUse/Hit_Metal");

        audioSrc = GetComponent<AudioSource>();

        Debug.Log(sfx_playerHit);
       // PlaySound("sfx_playerHit");

    }

    public static void PlaySound (string clip)
    {
        switch (clip)
        {
            case "sfx_playerHit":
                audioSrc.PlayOneShot(sfx_playerHit);
                break;

        }
    }

}
