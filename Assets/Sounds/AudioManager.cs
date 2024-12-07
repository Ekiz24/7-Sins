using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("------Audio Source------------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;
    // Start is called before the first frame update

    [Header("------Audio Clip-----------")]
    public AudioClip background;
    public AudioClip Slice;
    public AudioClip Chew;
    public AudioClip Coin;
    public AudioClip Coinfloor;
    public AudioClip Shoot;
    public AudioClip Puzzle;
    public static AudioManager instance;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
       
    }
    private void Start()
    {
        musicSource.clip = background;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

    public void Stop(AudioClip clip) 
    {
        musicSource.Stop();
    }
}
