using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    public static SoundManager Instance { get; set; }

    public AudioSource dropItemSound;
    public AudioSource craftingSound;
    public AudioSource toolSwingSound;
    public AudioSource chopSound;
    public AudioSource pickupItemSound;
    public AudioSource grassWalkSound;

    public AudioSource wateringChannel;
    public AudioClip wateringCan;

    //music
    public AudioSource startingZoneBGMusic;
    public AudioSource voiceovers;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }


    public void PlaySound(AudioSource soundToPlay)
    {
        if (soundToPlay.isPlaying == false)
        {
            soundToPlay.Play();
        }
    }
    public void PlayVoiceOvers(AudioClip clip)
    {
        voiceovers.clip = clip;
        if (!voiceovers.isPlaying)
        {
            voiceovers.Play();
        }
        else
        {
            voiceovers.Stop();
            voiceovers.Play();
        }
    }

   

}
