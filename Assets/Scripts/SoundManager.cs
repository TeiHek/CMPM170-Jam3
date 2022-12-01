using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{

    public static AudioClip sfx_HitArrow, sfx_HitFire, sfx_HitLance, sfx_HitMetal, sfx_Thunder;

    public static AudioClip sfx_MouseClick, sfx_MouseClickGood, sfx_MouseButton;

    static AudioSource audioSrc;



    // Start is called before the first frame update

    void Start()
    {
        //loading
        sfx_HitArrow = Resources.Load<AudioClip>("SFX/InUse/Hit_Arrow");
        sfx_HitFire = Resources.Load<AudioClip>("SFX/InUse/Hit_Fire");
        sfx_HitLance = Resources.Load<AudioClip>("SFX/InUse/Hit_Lance");
        sfx_HitMetal = Resources.Load<AudioClip>("SFX/InUse/Hit_Metal");
        sfx_Thunder = Resources.Load<AudioClip>("SFX/InUse/Hit_Thunder");


        sfx_MouseButton = Resources.Load<AudioClip>("SFX/InUse/Mouse_Button");
        sfx_MouseClick = Resources.Load<AudioClip>("SFX/InUse/Mouse_Click");
        sfx_MouseClickGood = Resources.Load<AudioClip>("SFX/InUse/Mouse_ClickGood");

        audioSrc = GetComponent<AudioSource>();


       //PlaySound("sfx_MouseButton", 1);

    }

    //clip: which sound u want to play
    //volumn: how loud u want the sound want to be
    public static void PlaySound (string clip, float volumn)
    {
        switch (clip)
        {
            case "sfx_HitArrow":
                audioSrc.clip = sfx_HitArrow;
                audioSrc.volume = volumn;
                audioSrc.PlayOneShot(audioSrc.clip);
                break;

            case "sfx_HitFire":
                audioSrc.clip = sfx_HitFire;
                audioSrc.volume = volumn;
                audioSrc.PlayOneShot(audioSrc.clip);
                break;

            case "sfx_HitLance":
                audioSrc.clip = sfx_HitLance;
                audioSrc.volume = volumn;
                audioSrc.PlayOneShot(audioSrc.clip);
                break;

            case "sfx_HitMetal":
                audioSrc.clip = sfx_HitMetal;
                audioSrc.volume = volumn;
                audioSrc.PlayOneShot(audioSrc.clip);
                break;

            case "sfx_Thunder":
                audioSrc.clip = sfx_Thunder;
                audioSrc.volume = volumn;
                audioSrc.PlayOneShot(audioSrc.clip);
                break;


            case "sfx_MouseClick":
                audioSrc.clip = sfx_MouseClick;
                audioSrc.volume = volumn;
                audioSrc.PlayOneShot(audioSrc.clip);
                break;

            case "sfx_MouseButton":
                audioSrc.clip = sfx_MouseButton;
                audioSrc.volume = volumn;
                audioSrc.PlayOneShot(audioSrc.clip);
                break;

            case "sfx_MouseClickGood":
                audioSrc.clip = sfx_MouseClickGood;
                audioSrc.volume = volumn;
                audioSrc.PlayOneShot(audioSrc.clip);
                break;
        }
    }

}
